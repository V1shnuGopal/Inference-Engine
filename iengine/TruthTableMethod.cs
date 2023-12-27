using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iengine
{
    public class TruthTableMethod : IMethod
    {
        // Count for number of models entailed 
        private int entailedCount;
        
        public string Solve(List<Sentence> sentences, string query)
        {
            // Initialize symbols from Knowledge Base as a List
            List<string> symbols = GetSymbols(sentences, query);

            // Initialize a model to hold truth values for each symbol 
            Dictionary<string, bool> model = new Dictionary<string, bool>();

            foreach(string symbol in symbols)
            {
                model.Add(symbol, false);
            }

            // Use truth table method to check if query is entailed in the knowledge base
            bool output = TTCheckAll(sentences, query, symbols, model);

            if (output)
            {
                return $"YES: {entailedCount}";
            }
            else
            {
                return "NO";
            }

        }

        // Return all symbols used in Knowledge Base
        private List<string> GetSymbols(List<Sentence> sentences, string query)
        {
            List<string> output = new List<string>();

            // Iterate through each sentence in the knowledge base
            foreach (Sentence sentence in sentences)
            {
                Queue<string> postfix = PostfixSentence.InfixToPostfix(sentence.inputString);
                while (postfix.Count > 0)
                {
                    string token = postfix.Dequeue();
                    if (Regex.IsMatch(token, "^[a-zA-Z0-9]+$") && !output.Contains(token))
                    {
                        output.Add(token);
                    }
                }
            }

            // Include symbols from query
            Queue<string> postfixQuery = PostfixSentence.InfixToPostfix(query);
            while (postfixQuery.Count > 0)
            {
                string token1 = postfixQuery.Dequeue();
                if (Regex.IsMatch(token1, "^[a-zA-Z0-9]+$") && !output.Contains(token1))
                {
                    output.Add(token1);
                }
            }
            return output;
        }

        // Check if model satisfies Knowledge Base
        private bool PLTrue(List<Sentence> sentences, Dictionary<string, bool> model)
        {
            // Loop through each sentence in Knowledge Base
            foreach(Sentence sentence in sentences)
            {
                Queue<string> postfix = PostfixSentence.InfixToPostfix(sentence.inputString);
                bool output = PostfixSentence.EvaluatePostfix(postfix, model);

                if (!output)
                {
                    return false;
                }
            }

            // If all sentences are true, return true
            return true;
        }

        // Using Truth Table method to enumerate all models
        private bool TTCheckAll(List<Sentence> sentences, string query, List<string> symbols, Dictionary<string,bool> model)
        {
            // Check if all symbols have been assigned a value in model
            if(symbols.Count == 0)
            {
                // If model satisfies all sentences, convert query to postfix and return true if model satisfies the query and increment count.
                if(PLTrue(sentences, model))
                {
                    entailedCount++;
                    Queue<string> postfixQuery = PostfixSentence.InfixToPostfix(query);
                    return PostfixSentence.EvaluatePostfix(postfixQuery, model);
                }

                return true;
            }

            // Generate models using Recursion
            string symbol = symbols[0];
            symbols = symbols.GetRange(1, symbols.Count - 1);

            model[symbol] = true;
            bool LHS = TTCheckAll(sentences, query, symbols, model);

            model[symbol] = false;
            bool RHS = TTCheckAll(sentences, query, symbols, model);

            return LHS && RHS;
        }
    }
}
