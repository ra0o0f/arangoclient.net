using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IDocumentIdentifierResult
    {
        string Id { get; set; }
        
        string Rev { get; set; }
        
        string Key { get; set; }
    }
}
