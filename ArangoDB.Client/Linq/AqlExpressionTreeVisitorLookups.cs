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

        static AqlExpressionTreeVisitor()
        {
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

                /*array*/
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
                {"Paths","paths"}
            };
        }
    }
}
