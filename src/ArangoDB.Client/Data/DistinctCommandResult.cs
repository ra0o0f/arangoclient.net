using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class DistinctCommandResult<T> : ICommandResult<T>
    {
        public BaseResult BaseResult { get; set; }

        public T Result { get; set; }
    }
}
