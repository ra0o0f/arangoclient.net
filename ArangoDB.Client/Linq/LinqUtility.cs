using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using ArangoDB.Client.Linq.Clause;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class LinqUtility
    {
        public static string ResolveCollectionName(IArangoDatabase db,Type itemType)
        {
            string collectionName = db.SharedSetting.Collection.ResolveCollectionName(itemType);
            return AddBacktickToName(collectionName);
        }

        public static string ResolveMemberName(IArangoDatabase db,MemberInfo memberInfo)
        {
            return ResolvePropertyName(db.SharedSetting.Collection.ResolvePropertyName(memberInfo.DeclaringType, memberInfo.Name));
        }

        public static string ResolvePropertyName(string name)
        {
            return AddBacktickToName(name.Replace("<", "").Replace(">", ""));
        }

        public static string AddBacktickToName(string name)
        {
            return string.Format("`{0}`", name);
        }

        public static IQueryParser CreateQueryParser()
        {
            //Create Custom node registry
            var customNodeTypeRegistry = new MethodInfoBasedNodeTypeRegistry();

            //Register new clause type
            customNodeTypeRegistry.Register(FilterExpressionNode.SupportedMethods, typeof(FilterExpressionNode));
            customNodeTypeRegistry.Register(LetSelectExpressionNode.SupportedMethods, typeof(LetSelectExpressionNode));
            customNodeTypeRegistry.Register(LetLambdaExpressionNode.SupportedMethods, typeof(LetLambdaExpressionNode));
            customNodeTypeRegistry.Register(TakeExpressionNode.SupportedMethods, typeof(TakeExpressionNode));
            customNodeTypeRegistry.Register(SkipExpressionNode.SupportedMethods, typeof(SkipExpressionNode));
            customNodeTypeRegistry.Register(GroupByExpressionNode.GroupBySupportedMethods, typeof(GroupByExpressionNode));
            customNodeTypeRegistry.Register(LimitExpressionNode.SupportedMethods, typeof(LimitExpressionNode));
            customNodeTypeRegistry.Register(UpdateAndReturnExpressionNode.SupportedMethods, typeof(UpdateAndReturnExpressionNode));
            customNodeTypeRegistry.Register(RemoveAndReturnExpressionNode.SupportedMethods, typeof(RemoveAndReturnExpressionNode));
            customNodeTypeRegistry.Register(InsertAndReturnExpressionNode.SupportedMethods, typeof(InsertAndReturnExpressionNode));
            customNodeTypeRegistry.Register(InModifyExpressionNode.SupportedMethods, typeof(InModifyExpressionNode));
            customNodeTypeRegistry.Register(ReturnResultModifyExpressionNode.SupportedMethods, typeof(ReturnResultModifyExpressionNode));
            customNodeTypeRegistry.Register(QueryableExtensions.OrderBySupportedMethods, typeof(ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel.OrderByExpressionNode));
            customNodeTypeRegistry.Register(QueryableExtensions.OrderByDescendingSupportedMethods, typeof(ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel.OrderByDescendingExpressionNode));
            customNodeTypeRegistry.Register(QueryableExtensions.SelectManySupportedMethods, typeof(ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel.SelectManyExpressionNode));

            //This creates all the default node types
            var nodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider();

            //add custom node provider to the providers
            //nodeTypeProvider.InnerProviders.Add(customNodeTypeRegistry);

            nodeTypeProvider.InnerProviders.Insert(0, customNodeTypeRegistry);

            var transformerRegistry = ExpressionTransformerRegistry.CreateDefault();
            var processor = ExpressionTreeParser.CreateDefaultProcessor(transformerRegistry);
            var expressionTreeParser = new ExpressionTreeParser(nodeTypeProvider, processor);
            var queryParser = new QueryParser(expressionTreeParser);

            return queryParser;
        }
        

        public static AqlModelVisitor FindParentModelVisitor(AqlModelVisitor modelVisitor)
        {
            AqlModelVisitor parentModelVisitor = modelVisitor;
            do
            {
                if (parentModelVisitor.ParnetModelVisitor == null)
                    return parentModelVisitor;

                parentModelVisitor = parentModelVisitor.ParnetModelVisitor;
            }
            while (true);
        }

        public static List<GroupByClause> PriorGroupBy(AqlModelVisitor modelVisitor)
        {
            List<GroupByClause> clauses = new List<GroupByClause>();

            FindGroupByRecursive(modelVisitor, clauses);

            return clauses;
        }

        static void FindGroupByRecursive(AqlModelVisitor modelVisitor,List<GroupByClause> clauses)
        {
            if (modelVisitor == null)
                return;

            var groupByClauses = modelVisitor.QueryModel.BodyClauses.Where(x => x is GroupByClause).Select(x=>x as GroupByClause).Where(x=>x.Visited);

            foreach(var g in groupByClauses)
            {
                clauses.Add(g);
                if (!g.GroupOnLastGroup)
                    return;
            }

            FindGroupByRecursive(modelVisitor.ParnetModelVisitor, clauses);
        }
    }
}
