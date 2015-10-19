using ArangoDB.Client.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public partial class AqlExpressionTreeVisitor
    {
        static Dictionary<ExpressionType, string> expressionTypes;
        static Dictionary<string, string> aqlMethods;

        static HashSet<string> methodsWithFirstGenericArgument;
        static HashSet<string> methodsWithSecondGenericArgument;
        static Dictionary<string, string> methodsWithNoParenthesis;

        static Dictionary<Type, Func<object, string>> enumToStrings = new Dictionary<Type, Func<object, string>>
        {
            //[typeof(EdgeDirection)] = (v) => Utils.EdgeDirectionToString((EdgeDirection)v)
        };

        static AqlExpressionTreeVisitor()
        {
            methodsWithFirstGenericArgument = new HashSet<string>
            {
                "near","within","within_rectangle","edges","neighbors","traversal","traversal_tree"
                ,"shortest_path","paths"
            };

            methodsWithSecondGenericArgument = new HashSet<string>
            {
                "neighbors","traversal","traversal_tree","shortest_path","paths"
            };

            methodsWithNoParenthesis = new Dictionary<string, string>
            {
                ["in"] = "in"
            };

            expressionTypes = new Dictionary<ExpressionType, string>()
            {
                {ExpressionType.Equal, " == "},
                {ExpressionType.NotEqual, " != "},
                {ExpressionType.LessThan, " < "},
                {ExpressionType.LessThanOrEqual, " <= "},
                {ExpressionType.GreaterThan, " > "},
                {ExpressionType.GreaterThanOrEqual, " >= "},
                {ExpressionType.And, " and "},
                {ExpressionType.AndAlso, " and "},
                {ExpressionType.Or, " or "},
                {ExpressionType.OrElse, " or "},
                {ExpressionType.Not, " not "},
                {ExpressionType.Add, " + "},
                {ExpressionType.Subtract, " - "},
                {ExpressionType.Multiply, " * "},
                {ExpressionType.Divide, " / "},
                {ExpressionType.Modulo, " % "}
            };

            aqlMethods = new Dictionary<string, string>()
            {
                /*type cast*/
                {"ToBool","to_bool"},
                {"ToNumber","to_number"},
                {"ToString","to_string"},
                {"ToArray","to_array"},
                {"IsNull","is_null"},
                {"IsBool","is_bool"},
                {"IsNumber","is_number"},
                {"IsString","is_string"},
                {"IsArray","is_array"},
                {"IsList","is_list"},
                {"IsObject","is_object"},
                {"IsDocument","is_document"},

                /*string*/
                {"Concat","concat"},
                {"ConcatSeparator","concat_separator"},
                {"CharLength","char_length"},
                {"Lower","lower"},
                {"Upper","upper"},
                {"Substitute","substitute"},
                {"Substring","substring"},
                {"Left","left"},
                {"Right","right"},
                {"Trim","trim"},
                {"LTrim","ltrim"},
                {"RTrim","rtrim"},
                {"Split","split"},
                {"Reverse","reverse"},
                {"Contains","contains"},
                {"FindFirst","find_first"},
                {"FindLast","find_last"},
                {"Like","like"},
                
                /*numeric*/
                {"Floor","floor"},
                {"Ceil","ceil"},
                {"Round","round"},
                {"Abs","abs"},
                {"Sqrt","sqrt"},
                {"Rand","rand"},
                
                /*date*/
                {"DateTimestamp","date_timestamp"},
                {"DateIso8601","date_iso8601"},
                {"DateDayOfWeek","date_dayofweek"},
                {"DateYear","date_year"},
                {"DateMonth","date_month"},
                {"DateDay","date_day"},
                {"DateHour","date_hour"},
                {"DateMinute","date_minute"},
                {"DateSecond","date_second"},
                {"DateMilliSecond","date_millisecond"},
                {"DateNow","date_now"},
                {"DateIsoWeek","date_isoweek"},
                {"DateLeapYear","date_leapyear"},
                {"DateQuarter","date_quarter"},
                {"DateDaysInMonth","date_days_in_month"},
                {"DateAdd","date_add"},
                {"DateSubtract","date_subtract"},
                {"DateDiff","date_diff"},
                {"DateCompare","date_compare"},
                {"DateFormat","date_format"},

                /*array*/
                {"In","in"},
                {"Length","length"},
                {"Flatten","flatten"},
                {"Min","min"},
                {"Max","max"},
                {"Average","average"},
                {"Sum","sum"},
                {"Median","median"},
                {"Percentile","percentile"},
                {"VariancePopulation","variance_population"},
                {"VarianceSample","variance_sample"},
                {"StdDevPopulation","stddev_population"},
                {"First","first"},
                {"Last","last"},
                {"Nth","nth"},
                {"Position","position"},
                {"Slice","slice"},
                {"Unique","unique"},
                {"Union","Union"},
                {"UnionDistinct","union_distinct"},
                {"Minus","minus"},
                {"Intersection","intersection"},
                {"Append","append"},
                {"Push","push"},
                {"UnShift","unshift"},
                {"Pop","pop"},
                {"Shift","shift"},
                {"RemoveValue","remove_value"},
                {"RemoveValues","remove_values"},
                {"RemoveNth","remove_nth"},

                /*object-document*/
                {"Merge","merge"},
                {"Matches","matches"},
                {"MergeRecursive","merge_recursive"},
                {"Translate","translate"},
                {"Has","has"},
                {"Attributes","attributes"},
                {"Values","values"},
                {"Zip","zip"},
                {"Unset","unset"},
                {"Keep","keep"},
                {"ParseIdentifier","parse_identifier"},

                /*geo*/
                {"Near","near"},
                {"Within","within"},
                {"WithinRectangle","within_rectangle"},
                {"IsInPolygon","is_in_polygon"},

                /*edge*/
                {"Edges","edges"},
                {"Neighbors","neighbors"},
                {"Traversal","traversal"},
                {"TraversalTree","traversal_tree"},
                {"ShortestPath","shortest_path"},
                {"Paths","paths"},

                /*graph*/
                { "GraphEdges","graph_edges" },
                { "GraphVertices","graph_vertices" },
                { "GraphNeighbors","graph_neighbors" },
                { "GraphCommonNeighbors","graph_common_neighbors" },
                { "GraphCommonProperties","graph_common_properties" },
                { "GraphPaths","graph_paths" },
                { "GraphShortestPath","graph_shortest_path" },
                { "GraphTraversal","graph_traversal" },
                { "GraphTraversalTree","graph_traversal_tree" },
                { "GraphDistanceTo","graph_distance_to" },
                { "GraphAbsoluteEccentricity","graph_absolute_eccentricity" },
                { "GraphEccentricity","graph_eccentricity" },
                { "GraphAbsoluteCloseness","graph_absolute_closeness" },
                { "GraphCloseness","graph_closeness" },
                { "GraphAbsoluteBetweenness","graph_absolute_betweenness" },
                { "GraphBetweenness","graph_betweenness" },
                { "GraphRadius","graph_radius" },
                { "GraphDiameter","graph_diameter" }
            };
        }
    }
}
