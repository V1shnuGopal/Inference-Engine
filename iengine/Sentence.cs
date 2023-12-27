using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iengine
{
    public class Sentence
    {
        public readonly string inputString;


        // Used to store both parts of the sentence
        public readonly string Conclusion;
        public readonly List<string> Premises = new List<string>();

        //Stores boolean values when a sentence contains a negation or does not. 
        public readonly List<bool> Negations = new List<bool>();

        public Sentence(string input)
        {
            inputString = input;

            // Remove all whitespaces characters from input
            string premise = Regex.Replace(input, @"\s+", "");

            // Split sentence by Implication operator if contained

            if (premise.Contains("=>"))
            {
                string[] tokens = premise.Split("=>");
                Conclusion = tokens[1];

                // Further split by Conjunction, Disjunction or Biconditional Operators if contained

                if (tokens[0].Contains("&"))
                {
                    tokens = tokens[0].Split("&");
                    foreach (string symbol in tokens)
                    {
                        
                        if (symbol.StartsWith("~"))
                        {
                            Premises.Add(symbol.Substring(1));
                            Negations.Add(true);
                        }
                        else
                        {
                            Premises.Add(symbol);
                            Negations.Add(false);
                        }
                    }
                }
                else if (tokens[0].Contains("||"))
                {
                    tokens = tokens[0].Split("||");
                    foreach (string symbol in tokens)
                    {
                        if (symbol.StartsWith("~"))
                        {
                            Premises.Add(symbol.Substring(1));
                            Negations.Add(true);
                        }
                        else
                        {
                            Premises.Add(symbol);
                            Negations.Add(false);
                        }
                    }
                }
                else if (tokens[0].Contains("<=>"))
                {
                    tokens = tokens[0].Split("<=>");
                    foreach (string symbol in tokens)
                    {
                        if (symbol.StartsWith("~"))
                        {
                            Premises.Add(symbol.Substring(1));
                            Negations.Add(true);
                        }
                        else
                        {
                            Premises.Add(symbol);
                            Negations.Add(false);
                        }
                    }
                }
                // If sentence includes only negation symbol does not include any operators, add as single premise
                // Add without negation symbol if contained
                else
                {
                    if (tokens[0].StartsWith("~"))
                    {
                        Premises.Add(tokens[0].Substring(1));
                        Negations.Add(true);
                    }
                    else
                    {
                        Premises.Add(tokens[0]);
                        Negations.Add(false);
                    }
                }
            }
            // If implication operator is not contained in premise, it is a single premise
            else
            {
                Conclusion = premise;
            }
        }
    }
}
