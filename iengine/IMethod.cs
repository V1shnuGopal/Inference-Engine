using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iengine
{

    // Method used to solve the query within a given knowledge base
    // sentences = The knowledge base (a list of sentence objects)
    // query = The query which follows the ASK keyword
    // Returns "YES" or "NO" with information about the solution
    public interface IMethod
    {
        string Solve(List<Sentence> sentences, string query);
    }
}
