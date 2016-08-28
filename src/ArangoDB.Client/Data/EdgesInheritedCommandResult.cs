using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class EdgesInheritedCommandResult<T> : BaseResult, ICommandResult<T>
    {
        [DocumentProperty(IgnoreProperty = true)]
        public BaseResult BaseResult
        {
            get { return this as BaseResult; }
            set { SetProperties(value); }
        }

        [DocumentProperty(PropertyName="edges")]
        public T Result { get; set; }
    }
}
