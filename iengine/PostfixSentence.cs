using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iengine
{
    public static class PostfixSentence
    {

        // Iterates through each character in the input infix expression and separates operands and operators.
        public static Queue<string> InfixToPostfix(string infix)
        {
            // Initialize queue for output expression and stack for operators
            Queue<string> output = new Queue<string>();
            Stack<string> stack = new Stack<string>();

            // Accumulate characters that represent an operand
            string token = "";

            // Iterate through infix sentences
            for (int i = 0; i < infix.Length; i++)
            {
                char c = infix[i];
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }
                if (char.IsLetterOrDigit(c))
                {
                    token += c;
                }
                else
                {
                    if (token.Length > 0)
                    {
                        output.Enqueue(token);
                        token = "";
                    }

                    // Push open parentheses onto the stack
                    // If closing parenthesis is found, pop operands from the stack until an opening parenthesis is found

                    if (c == '(')
                    {
                        stack.Push(c.ToString());
                    }
                    else if (c == ')')
                    {
                        while (stack.Count > 0 && stack.Peek() != "(")
                        {
                            output.Enqueue(stack.Pop());
                        }

                        if (stack.Count > 0 && stack.Peek() == "(")
                        {
                            stack.Pop();
                        }
                    }

                    //Parse operators, add to output queue according to operator precedence
                    else
                    {
                        string op = c.ToString();
                        if (i + 2 < infix.Length && c == '<' && infix[i + 1] == '=' && infix[i + 2] == '>')
                        {
                            op = "<=>";
                            i += 2;
                        }
                        else if (i + 1 < infix.Length && (c == '=' || c == '|' || c == '>'))
                        {
                            op += infix[i + 1];
                            i++;
                        }

                        while (stack.Count > 0 && stack.Peek() != "(" && GetPrecedence(stack.Peek()) >= GetPrecedence(op))
                        {
                            output.Enqueue(stack.Pop());
                        }

                        stack.Push(op);
                    }
                }
            }

            if (token.Length > 0)
            {
                output.Enqueue(token);
            }

            while (stack.Count > 0)
            {
                output.Enqueue(stack.Pop());
            }

            return output;
        }

        private static int GetPrecedence(string op)
        {
            switch (op)
            {
                case "~":
                    return 4;
                case "&":
                    return 3;
                case "||":
                    return 2;
                case "=>":
                    return 1;
                case "<=>":
                    return 0;
                default:
                    // Invalid operator
                    throw new ArgumentException("Invalid operator: " + op);
            }
        }


        public static bool EvaluatePostfix(Queue<string> postfix, Dictionary<string, bool> model)
        {
            Stack<bool> stack = new Stack<bool>();

            while (postfix.Count > 0)
            {
                // Initialize token string by first token in Queue
                string token = postfix.Dequeue();

                if (Regex.IsMatch(token, "^[a-zA-Z0-9]+$"))
                {
                    // If token is an operand, push value from model onto stack
                    stack.Push(model[token]);
                }
                // If the token is an operator, pop the necessary number of operands from the stack,
                // apply the operator to them and push the result back onto the stack 
                else
                {
                    bool RHS = stack.Pop();
                    bool LHS;

                    switch (token)
                    {
                        case "~":
                            stack.Push(!RHS);
                            break;
                        case "&":
                            LHS = stack.Pop();
                            stack.Push(LHS && RHS);
                            break;
                        case "||":
                            LHS = stack.Pop();
                            stack.Push(LHS || RHS);
                            break;
                        case "=>":
                            LHS = stack.Pop();
                            stack.Push(!LHS || RHS);
                            break;
                        case "<=>":
                            LHS = stack.Pop();
                            stack.Push(LHS == RHS);
                            break;
                        default:
                            // Invalid operator
                            throw new ArgumentException("Invalid operator: " + token);
                    }
                }
            }
            // Once all tokens have been processed, pop the final result from the stack and return it
            return stack.Pop();
        }
    }
}
