using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class InheritedCommandResult<T> : BaseResult, ICommandResult<T>
    {
        [DocumentProperty(IgnoreProperty = true)]
        public BaseResult BaseResult
        {
            get { return this as BaseResult; }
            set { base.SetProperties(value); }
        }

        public T Result { get; set; }
    }
}
