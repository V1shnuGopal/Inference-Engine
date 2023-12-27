using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iengine
{
    public class BackwardChainingMethod : IMethod
    {
        public string Solve(List<Sentence> sentences, string query)
        {
            // Create a list of facts from the knowledge base
            List<string> symbols = new List<string>();
            foreach (Sentence sentence in sentences)
            {
                if (sentence.Premises.Count == 0)
                {
                    symbols.Add(sentence.Conclusion);
                }
            }

            // Initialize result list and objects to store the processing status of the symbol
            List<string> result = new List<string>();
            HashSet<string> explored = new HashSet<string>();
            HashSet<string> processing = new HashSet<string>();

            // Call recursive method to check if query can be explored
            if (PL_BCRecursive(sentences, query, symbols, result, explored, processing))
            {
                return "YES: " + string.Join(", ", result);
            }
            else
            {
                return "NO";
            }
        }

        // Recursive method to check if query can be explored using backward chaining
        private bool PL_BCRecursive(List<Sentence> sentences, string query, List<string> symbols, List<string> result, HashSet<string> explored, HashSet<string> processing)
        {
            // If query is an symbol(fact), add it to result and return true
            if (symbols.Contains(query))
            {
                if (!explored.Contains(query))
                {
                    result.Add(query);
                    explored.Add(query);
                }
                return true;
            }

            // If query is already being processed, return false to avoid infinite recursion
            if (processing.Contains(query))
            {
                return false;
            }

            processing.Add(query);

            // Check if any sentence in the knowledge base has the query as its conclusion
            foreach (Sentence sentence in sentences)
            {
                if (sentence.Conclusion == query)
                {
                    bool allPremisesTrue = true;
                    // Check if all premises of the sentence can be inferred
                    foreach (string premise in sentence.Premises)
                    {
                        if (!PL_BCRecursive(sentences, premise, symbols, result, explored, processing))
                        {
                            allPremisesTrue = false;
                            break;
                        }
                    }
                    // If all premises can be inferred, add query to result and return true
                    if (allPremisesTrue)
                    {
                        if (!explored.Contains(query))
                        {
                            result.Add(query);
                            explored.Add(query);
                        }
                        processing.Remove(query);
                        return true;
                    }
                }
            }

            // If query cannot be inferred from any sentence in the knowledge base, return false
            processing.Remove(query);
            return false;
        }
    }
}
