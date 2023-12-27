using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace iengine
{
    public class Program
    {
        static void Main(string[] args)
        {

            // Check number of arguments
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid number of arguments");
                Console.WriteLine("Usage: iengine <method> <filename>");
                return;
            }

            // Check if file exists
            if (!File.Exists(args[1]))
            {
                Console.WriteLine("Cannot find file: " + args[1]);
                return;
            }
            try
            {
                // Initialize arguments as method and filename
                string method = args[0];
                string fileName = args[1];

                // Initialize Sentence Knowledge Base and query
                List<Sentence> sentences = new List<Sentence>();
                string query = "";

                // Read data from file
                string[] lines = File.ReadAllLines(fileName);

                int parseState = 0;

                // Iterate through each line and parse it based on parseState 
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();

                    // If the line is empty or whitespace, skip it
                    if (string.IsNullOrEmpty(line.Trim()))
                    {
                        continue;
                    }

                    // Set parse state according to "TELL" or "ASK" keywords
                    switch (line)
                    {
                        case "TELL":
                            parseState = 1;
                            continue;
                        case "ASK":
                            parseState = 2;
                            continue;
                    }

                    // If parseState keyword is "TELL",
                    // parse each consecutive line as a Sentence object and add it to the knowledge base
                    if (parseState == 1)
                    {
                        string[] tokens = line.Split(';');

                        for (int j = 0; j < tokens.Length; j++)
                        {
                            string token = tokens[j].Trim();
                            if (string.IsNullOrWhiteSpace(token))
                            {
                                continue;
                            }

                            Sentence sentence = new Sentence(token);
                            sentences.Add(sentence);
                        }
                    }

                    // If parseState keyword is "ASK", the line is a query to solve
                    else if (parseState == 2)
                    {
                        query = line;
                        break;
                    }
                }


                // Initialize IMethod and determine which method the user wants to use to solve the query
                IMethod solveMethod;

                switch (method)
                {
                    case "TT":
                        solveMethod = new TruthTableMethod();
                        break;
                    case "FC":
                        solveMethod = new ForwardChainingMethod();
                        break;
                    case "BC":
                        solveMethod = new BackwardChainingMethod();
                        break;
                    default:
                        solveMethod = null;
                        break;
                }

                // Check if method is valid
                if (solveMethod == null)
                {
                    Console.WriteLine("Invalid Method. Please use TT, FC or BC");
                    return;
                }
                // Call the solve method to solve the query and store the result in output string
                string output = solveMethod.Solve(sentences, query);

                // Check if solution is found
                if (output == null)
                {
                    Console.WriteLine("Unable to find a solution!");
                    return;
                }

                Console.WriteLine(output);
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
