using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iengine
{
    public class ForwardChainingMethod : IMethod
    {
        public string Solve(List<Sentence> sentences, string query)
        {
            // Count of premises in each sentence
            Dictionary<Sentence, int> count = new Dictionary<Sentence, int>();
            // Inferred symbols
            List<string> inferred = new List<string>();
            // Unprocessed symbols
            Queue<string> queue = new Queue<string>();

            // Initialize the count and enqueue sentences with no premises
            foreach (Sentence sentence in sentences)
            {
                count.Add(sentence, sentence.Premises.Count);

                if(sentence.Premises.Count == 0)
                {
                    queue.Enqueue(sentence.Conclusion);
                }
            }

            while (queue.Count > 0)
            {
                // Dequeue a symbol from the queue
                string symbol = queue.Dequeue();

                // If symbol is equal to query, generate the output string
                if(symbol == query)
                {
                    string output;
                    if(inferred.Count > 0) 
                    {
                        output = "YES: " + string.Join(", ", inferred) + ", " + query;
                    }
                    else
                    {
                        output = "YES: " + query;
                    }
                    return output;
                }

                // Add symbol to inferred list
                if (!inferred.Contains(symbol))
                {
                    inferred.Add(symbol);
                }

                // For each sentence, check if all premises are inferred
                // If they are inferred, decrease the count and enqueue the conclusion
                foreach (Sentence sentence in sentences)
                {
                    bool premisesInferred = true;

                    foreach (string premise in sentence.Premises)
                    {
                        if (!inferred.Contains(premise))
                        {
                            premisesInferred = false;
                            break;
                        }
                    }

                    if (premisesInferred)
                    {
                        foreach (string premise in sentence.Premises)
                        {
                            count[sentence]--;
                            if (count[sentence] == 0)
                            {
                                queue.Enqueue(sentence.Conclusion);
                            }
                        }
                    }
                }
            }
            // If query cannot be entailed, return NO
            return $"NO";
        }
    }
}
