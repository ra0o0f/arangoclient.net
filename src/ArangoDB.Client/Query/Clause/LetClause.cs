using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query.Clause
{
    public sealed class LetClause : IBodyClause, IQuerySource
    {
        private readonly string _itemName;

        private Expression _letExpression;

        public LetClause(string itemName, Expression letExpression)
        {
            LinqUtility.CheckNotNull("itemName", itemName);
            LinqUtility.CheckNotNull("letExpression", letExpression);

            _itemName = itemName;
            _letExpression = letExpression;
        }

        public LetClause(string itemName, Expression letExpression, Expression subQueryexpression)
            : this(itemName, letExpression)
        {
            SubqueryExpression = subQueryexpression;
        }

        public string ItemName
        {
            get { return _itemName; }
        }

        public Type ItemType
        {
            get { return LetExpression.Type; }
        }

        public Expression LetExpression
        {
            get { return _letExpression; }
            set { _letExpression = LinqUtility.CheckNotNull("value", value); }
        }

        public Expression SubqueryExpression { get; set; }

        public void TransformExpressions(Func<Expression, Expression> transformation)
        {
            _letExpression = transformation(_letExpression);
        }

        public void Accept(IQueryModelVisitor visitor, QueryModel queryModel, int index)
        {
            var arangoVisotor = visitor as ArangoModelVisitor;
            if (arangoVisotor == null)
                throw new Exception("QueryModelVisitor should be type of ArangoModelVisitor");

            arangoVisotor.VisitLetClause(this, queryModel, typeof(object));
        }

        public IBodyClause Clone(CloneContext cloneContext)
        {
            return new LetClause(ItemName, LetExpression);
        }

        public override string ToString()
        {
            return string.Format("let {0} = {1}", ItemName, LetExpression.ToString());
        }
    }
}
