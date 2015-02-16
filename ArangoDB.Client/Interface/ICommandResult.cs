using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface ICommandResult<T>
    {
        BaseResult BaseResult { get; set; }

        T Result { get; set; }
    }
}
