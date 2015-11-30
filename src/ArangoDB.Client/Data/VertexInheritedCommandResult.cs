using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class VertexInheritedCommandResult<T> : BaseResult, ICommandResult<T>
    {
        [DocumentProperty(IgnoreProperty = true)]
        public BaseResult BaseResult
        {
            get { return this as BaseResult; }
            set { base.SetProperties(value); }
        }

        [DocumentProperty(PropertyName = "vertex")]
        public T Result { get; set; }
    }
}
