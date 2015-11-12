using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public class AQL
    {
        static Exception e = new InvalidOperationException("Aql functions should only be used in ArangoDatabase.Query");

        public static T As<T>(object any) { throw e; }

        /*type cast*/
        public static bool ToBool(object value) { throw e; }
        public static double ToNumber(object value) { throw e; }
        public static string ToString(object value) { throw e; }
        public static IList<T> ToArray<T>(T value) { throw e; }
        public static IList<T> ToList<T>(T value) { throw e; }
        public static bool IsNull(object value) { throw e; }
        public static bool IsBool(object value) { throw e; }
        public static bool IsNumber(object value) { throw e; }
        public static bool IsString(object value) { throw e; }
        public static bool IsArray(object value) { throw e; }
        public static bool IsList(object value) { throw e; }
        public static bool IsObject(object value) { throw e; }
        public static bool IsDocument(object value) { throw e; }

        /*string*/
        public static string Concat(params string[] str) { throw e; }
        public static string ConcatSeparator(string separator, params string[] str) { throw e; }
        public static int CharLength(string value) { throw e; }
        public static string Lower(string value) { throw e; }
        public static string Upper(string value) { throw e; }
        public static string Substitute(string value, string search, string replace) { throw e; }
        public static string Substitute(string value, string search, string replace, int limit) { throw e; }
        public static string Substring(string value, int offset) { throw e; }
        public static string Substring(string value, int offset, int length) { throw e; }
        public static string Left(string value, int length) { throw e; }
        public static string Right(string value, int length) { throw e; }
        public static string Trim(string value) { throw e; }
        public static string Trim(string value, int type) { throw e; }
        public static string Trim(string value, string chars) { throw e; }
        public static string LTrim(string value) { throw e; }
        public static string LTrim(string value, string chars) { throw e; }
        public static string RTrim(string value) { throw e; }
        public static string RTrim(string value, string chars) { throw e; }
        public static string[] Split(string value) { throw e; }
        public static string[] Split(string value, string separator) { throw e; }
        public static string[] Split(string value, string separator, int limit) { throw e; }
        public static string Reverse(string value) { throw e; }
        public static bool Contains(string text, string search) { throw e; }
        public static int Contains(string text, string search, bool returnIndex) { throw e; }
        public static int FindFirst(string text, string search) { throw e; }
        public static int FindFirst(string text, string search, int start, int end) { throw e; }
        public static int FindLast(string text, string search) { throw e; }
        public static int FindLast(string text, string search, int start, int end) { throw e; }
        public static bool Like(string text, string search) { throw e; }
        public static bool Like(string text, string search, bool caseInsensitive) { throw e; }

        /*numeric*/
        public static double Floor(double value) { throw e; }
        public static double Ceil(double value) { throw e; }
        public static double Round(double value) { throw e; }
        public static double Abs(double value) { throw e; }
        public static double Sqrt(double value) { throw e; }
        public static double Rand() { throw e; }

        /*date*/
        public static double DateTimestamp(string date) { throw e; }
        public static double DateTimestamp(double date) { throw e; }
        public static double DateTimestamp(DateTime date) { throw e; }
        public static double DateTimestamp(DateTimeOffset date) { throw e; }
        public static double DateTimestamp(int year, int month, int day) { throw e; }
        public static double DateTimestamp(int year, int month, int day, int hour) { throw e; }
        public static double DateTimestamp(int year, int month, int day, int hour, int minute) { throw e; }
        public static double DateTimestamp(int year, int month, int day, int hour, int minute, int second) { throw e; }
        public static double DateTimestamp(int year, int month, int day, int hour, int minute, int second, int millisecond) { throw e; }

        public static string DateIso8601(string date) { throw e; }
        public static string DateIso8601(double date) { throw e; }
        public static string DateIso8601(DateTime date) { throw e; }
        public static string DateIso8601(DateTimeOffset date) { throw e; }
        public static string DateIso8601(int year, int month, int day) { throw e; }
        public static string DateIso8601(int year, int month, int day, int hour) { throw e; }
        public static string DateIso8601(int year, int month, int day, int hour, int minute) { throw e; }
        public static string DateIso8601(int year, int month, int day, int hour, int minute, int second) { throw e; }
        public static string DateIso8601(int year, int month, int day, int hour, int minute, int second, int millisecond) { throw e; }

        public static int DateDayOfWeek(string date) { throw e; }
        public static int DateDayOfWeek(double date) { throw e; }
        public static int DateDayOfWeek(DateTime date) { throw e; }
        public static int DateDayOfWeek(DateTimeOffset date) { throw e; }

        public static int DateYear(string date) { throw e; }
        public static int DateYear(double date) { throw e; }
        public static int DateYear(DateTime date) { throw e; }
        public static int DateYear(DateTimeOffset date) { throw e; }

        public static int DateMonth(string date) { throw e; }
        public static int DateMonth(double date) { throw e; }
        public static int DateMonth(DateTime date) { throw e; }
        public static int DateMonth(DateTimeOffset date) { throw e; }

        public static int DateDay(string date) { throw e; }
        public static int DateDay(double date) { throw e; }
        public static int DateDay(DateTime date) { throw e; }
        public static int DateDay(DateTimeOffset date) { throw e; }

        public static int DateHour(string date) { throw e; }
        public static int DateHour(double date) { throw e; }
        public static int DateHour(DateTime date) { throw e; }
        public static int DateHour(DateTimeOffset date) { throw e; }

        public static int DateMinute(string date) { throw e; }
        public static int DateMinute(double date) { throw e; }
        public static int DateMinute(DateTime date) { throw e; }
        public static int DateMinute(DateTimeOffset date) { throw e; }

        public static int DateSecond(string date) { throw e; }
        public static int DateSecond(double date) { throw e; }
        public static int DateSecond(DateTime date) { throw e; }
        public static int DateSecond(DateTimeOffset date) { throw e; }

        public static int DateMilliSecond(string date) { throw e; }
        public static int DateMilliSecond(double date) { throw e; }
        public static int DateMilliSecond(DateTime date) { throw e; }
        public static int DateMilliSecond(DateTimeOffset date) { throw e; }

        public static int DateIsoWeek(string date) { throw e; }
        public static int DateIsoWeek(double date) { throw e; }
        public static int DateIsoWeek(DateTime date) { throw e; }
        public static int DateIsoWeek(DateTimeOffset date) { throw e; }

        public static bool DateLeapYear(string date) { throw e; }
        public static bool DateLeapYear(double date) { throw e; }
        public static bool DateLeapYear(DateTime date) { throw e; }
        public static bool DateLeapYear(DateTimeOffset date) { throw e; }

        public static int DateQuarter(string date) { throw e; }
        public static int DateQuarter(double date) { throw e; }
        public static int DateQuarter(DateTime date) { throw e; }
        public static int DateQuarter(DateTimeOffset date) { throw e; }

        public static int DateDaysInMonth(string date) { throw e; }
        public static int DateDaysInMonth(double date) { throw e; }
        public static int DateDaysInMonth(DateTime date) { throw e; }
        public static int DateDaysInMonth(DateTimeOffset date) { throw e; }

        public static string DateAdd(string date,string duration) { throw e; }
        public static string DateAdd(double date, string duration) { throw e; }
        public static string DateAdd(DateTime date, string duration) { throw e; }
        public static string DateAdd(DateTimeOffset date, string duration) { throw e; }
        public static string DateAdd(string date,int amount, string unit) { throw e; }
        public static string DateAdd(double date,int amount, string unit) { throw e; }
        public static string DateAdd(DateTime date, int amount, string unit) { throw e; }
        public static string DateAdd(DateTimeOffset date, int amount, string unit) { throw e; }

        public static string DateSubtract(string date, string duration) { throw e; }
        public static string DateSubtract(double date, string duration) { throw e; }
        public static string DateSubtract(DateTime date, string duration) { throw e; }
        public static string DateSubtract(DateTimeOffset date, string duration) { throw e; }
        public static string DateSubtract(string date, int amount, string unit) { throw e; }
        public static string DateSubtract(double date, int amount, string unit) { throw e; }
        public static string DateSubtract(DateTime date, int amount, string unit) { throw e; }
        public static string DateSubtract(DateTimeOffset date, int amount, string unit) { throw e; }

        public static double DateDiff(string date1,string date2,string unit) { throw e; }
        public static double DateDiff(double date1, double date2, string unit) { throw e; }
        public static double DateDiff(DateTime date1, DateTime date2, string unit) { throw e; }
        public static double DateDiff(DateTimeOffset date1, DateTimeOffset date2, string unit) { throw e; }
        public static double DateDiff(string date1, string date2, string unit, bool asFloat) { throw e; }
        public static double DateDiff(double date1, double date2, string unit, bool asFloat) { throw e; }
        public static double DateDiff(DateTime date1, DateTime date2, string unit, bool asFloat) { throw e; }
        public static double DateDiff(DateTimeOffset date1, DateTimeOffset date2, string unit, bool asFloat) { throw e; }

        public static bool DateCompare(string date1, string date2, string unitRangeStart, string unitRangeEnd) { throw e; }
        public static bool DateCompare(double date1, double date2, string unitRangeStart, string unitRangeEnd) { throw e; }
        public static bool DateCompare(DateTime date1, DateTime date2, string unitRangeStart, string unitRangeEnd) { throw e; }
        public static bool DateCompare(DateTimeOffset date1, DateTimeOffset date2, string unitRangeStart, string unitRangeEnd) { throw e; }
        public static bool DateCompare(string date1, string date2, string unitRangeStart) { throw e; }
        public static bool DateCompare(double date1, double date2, string unitRangeStart) { throw e; }
        public static bool DateCompare(DateTime date1, DateTime date2, string unitRangeStart) { throw e; }
        public static bool DateCompare(DateTimeOffset date1, DateTimeOffset date2, string unitRangeStart) { throw e; }

        public static string DateFormat(string date,string format) { throw e; }
        public static string DateFormat(double date, string format) { throw e; }
        public static string DateFormat(DateTime date, string format) { throw e; }
        public static string DateFormat(DateTimeOffset date, string format) { throw e; }

        public static double DateNow() { throw e; }

        /*array*/
        public static bool In<T>(T member, IEnumerable<T> array) { throw e; }
        public static int Length(object array) { throw e; }
        public static IList<T> Flatten<T>(object array) { throw e; }
        public static IList<T> Flatten<T>(object array, int depth) { throw e; }
        public static T Min<T>(IEnumerable<T> array) { throw e; }
        public static T Max<T>(IEnumerable<T> array) { throw e; }
        public static T Average<T>(IEnumerable<T> array) { throw e; }
        public static T Sum<T>(IEnumerable<T> array) { throw e; }
        public static T Median<T>(IEnumerable<T> array) { throw e; }
        public static T Percentile<T>(IEnumerable<T> array, int n) { throw e; }
        public static T Percentile<T>(IEnumerable<T> array, int n, string method) { throw e; }
        public static T VariancePopulation<T>(IEnumerable<T> array) { throw e; }
        public static T VarianceSample<T>(IEnumerable<T> array) { throw e; }
        public static T StdDevPopulation<T>(IEnumerable<T> array) { throw e; }
        public static IList<T> Reverse<T>(IEnumerable<T> array) { throw e; }
        public static T First<T>(IEnumerable<T> array) { throw e; }
        public static T Last<T>(IEnumerable<T> array) { throw e; }
        public static T Nth<T>(IEnumerable<T> array, int position) { throw e; }
        public static bool Position<T>(IEnumerable<T> array, T search) { throw e; }
        public static object Position<T>(IEnumerable<T> array, T search, bool returnIndex) { throw e; }
        public static IList<T> Slice<T>(IEnumerable<T> array, int start) { throw e; }
        public static IList<T> Slice<T>(IEnumerable<T> array, int start, int length) { throw e; }
        public static IList<T> Unique<T>(IEnumerable<T> array) { throw e; }
        public static IList<T> Union<T>(IEnumerable<T> array1, IEnumerable<T> array2) { throw e; }
        public static IList<T> Union<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3) { throw e; }
        public static IList<T> Union<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3, IEnumerable<T> array4) { throw e; }
        public static IList<T> Union<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3, IEnumerable<T> array4, IEnumerable<T> array5) { throw e; }
        public static IList<T> UnionDistinct<T>(IEnumerable<T> array1, IEnumerable<T> array2) { throw e; }
        public static IList<T> UnionDistinct<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3) { throw e; }
        public static IList<T> UnionDistinct<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3, IEnumerable<T> array4) { throw e; }
        public static IList<T> UnionDistinct<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3, IEnumerable<T> array4, IEnumerable<T> array5) { throw e; }
        public static IList<T> Minus<T>(IEnumerable<T> array1, IEnumerable<T> array2) { throw e; }
        public static IList<T> Minus<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3) { throw e; }
        public static IList<T> Minus<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3, IEnumerable<T> array4) { throw e; }
        public static IList<T> Minus<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3, IEnumerable<T> array4, IEnumerable<T> array5) { throw e; }
        public static IList<T> Intersection<T>(IEnumerable<T> array1, IEnumerable<T> array2) { throw e; }
        public static IList<T> Intersection<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3) { throw e; }
        public static IList<T> Intersection<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3, IEnumerable<T> array4) { throw e; }
        public static IList<T> Intersection<T>(IEnumerable<T> array1, IEnumerable<T> array2, IEnumerable<T> array3, IEnumerable<T> array4, IEnumerable<T> array5) { throw e; }
        public static IList<T> Append<T>(IEnumerable<T> array, IEnumerable<T> values) { throw e; }
        public static IList<T> Append<T>(IEnumerable<T> array, IEnumerable<T> values, bool unique) { throw e; }
        public static IList<T> Push<T>(IEnumerable<T> array, T value) { throw e; }
        public static IList<T> Push<T>(IEnumerable<T> array, T value, bool unique) { throw e; }
        public static IList<T> UnShift<T>(IEnumerable<T> array, T value) { throw e; }
        public static IList<T> UnShift<T>(IEnumerable<T> array, T value, bool unique) { throw e; }
        public static IList<T> Pop<T>(IEnumerable<T> array) { throw e; }
        public static IList<T> Shift<T>(IEnumerable<T> array) { throw e; }
        public static IList<T> RemoveValue<T>(IEnumerable<T> array, T value) { throw e; }
        public static IList<T> RemoveValue<T>(IEnumerable<T> array, T value, int limit) { throw e; }
        public static IList<T> RemoveValues<T>(IEnumerable<T> array, IEnumerable<T> values) { throw e; }
        public static IList<T> RemoveNth<T>(IEnumerable<T> array, int position) { throw e; }

        /*object-document*/
        public static T Merge<T>(object document1, object document2) { throw e; }
        public static T Merge<T>(object document1, object document2, object document3) { throw e; }
        public static T Merge<T>(object document1, object document2, object document3, object document4) { throw e; }
        public static T Merge<T>(object document1, object document2, object document3, object document4, object document5) { throw e; }
        public static bool Matches(object document, IEnumerable<object> examples) { throw e; }
        public static object Matches(object document, IEnumerable<object> examples, bool returnIndex) { throw e; }
        public static T MergeRecursive<T>(object document1, object document2) { throw e; }
        public static T MergeRecursive<T>(object document1, object document2, object document3) { throw e; }
        public static T MergeRecursive<T>(object document1, object document2, object document3, object document4) { throw e; }
        public static T MergeRecursive<T>(object document1, object document2, object document3, object document4, object document5) { throw e; }
        public static T Translate<T>(string value, object lookup) { throw e; }
        public static T Translate<T>(string value, object lookup, object defaultValue) { throw e; }
        public static bool Has(object document, string attributeName) { throw e; }
        public static IList<string> Attributes(object document) { throw e; }
        public static IList<string> Attributes(object document, bool removeInternal) { throw e; }
        public static IList<string> Attributes(object document, bool removeInternal, bool sort) { throw e; }
        public static IList<object> Values(object document) { throw e; }
        public static IList<object> Values(object document, bool removeInternal) { throw e; }
        public static T Zip<T>(IEnumerable<string> attributes, IEnumerable<object> values) { throw e; }
        public static T Unset<T>(object document, params string[] attributeNames) { throw e; }
        public static T Keep<T>(object document, params string[] attributeNames) { throw e; }
        public static ParseIdentifierResult ParseIdentifier(string documentHandle) { throw e; }

        /*geo*/
        public static IList<T> Near<T>(double latitude, double longitude) { throw e; }
        public static IList<T> Near<T>(double latitude, double longitude, int limit) { throw e; }
        public static IList<T> Near<T>(double latitude, double longitude, int limit, string distance) { throw e; }
        //TODO: add distance overload
        //public static IList<T> Near<T>(double latitude, double longitude, int limit, Expression<Func<T, object>> distance) { throw e; }
        public static IList<T> Within<T>(double latitude, double longitude, double radius) { throw e; }
        public static IList<T> Within<T>(double latitude, double longitude, double radius, string distance) { throw e; }
        //TODO: add distance overload
        //public static IList<T> Within<T>(double latitude, double longitude, double radius, Expression<Func<T, object>> distance) { throw e; }
        public static IList<T> WithinRectangle<T>(double latitude1, double longitude1, double latitude2, double longitude2) { throw e; }
        public static bool IsInPolygon(double[][] polygon, double latitude, double longitude) { throw e; }
        public static bool IsInPolygon(double[][] polygon, double[] coordinates) { throw e; }
        public static bool IsInPolygon(double[][] polygon, double[] coordinates, bool reverseCoordinateOrder) { throw e; }

        /*edge*/
        public static IList<TEdgeCollection> Edges<TEdgeCollection>(string startVertex, EdgeDirection direction) { throw e; }
        public static IList<TEdgeCollection> Edges<TEdgeCollection>(string startVertex, EdgeDirection direction, object edgeExample) { throw e; }
        public static IList<TEdgeCollection> Edges<TEdgeCollection>(string startVertex, EdgeDirection direction, object edgeExample, object options) { throw e; }
        public static IList<EdgeVertexResult<TVertexResult, TEdgeCollection>> Edges<TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction, object edgeExample, object options) { throw e; }

        public static IList<string> Neighbors<TVertexCollection, TEdgeCollection>(string startVertex, EdgeDirection direction) { throw e; }
        public static IList<string> Neighbors<TVertexCollection, TEdgeCollection>(string startVertex, EdgeDirection direction, object edgeExample) { throw e; }
        public static IList<string> Neighbors<TVertexCollection, TEdgeCollection>(string startVertex, EdgeDirection direction, object edgeExample, object options) { throw e; }
        public static IList<TVertexResult> Neighbors<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction, object edgeExample, object options) { throw e; }

        public static IList<AQLTraversalResult<TVertexCollection, TEdgeCollection>> Traversal<TVertexCollection, TEdgeCollection>(string startVertex, EdgeDirection direction) { throw e; }
        public static IList<AQLTraversalResult<TVertexCollection, TEdgeCollection>> Traversal<TVertexCollection, TEdgeCollection>(string startVertex, EdgeDirection direction, object options) { throw e; }
        public static IList<AQLTraversalResult<TVertexResult, TEdgeCollection>> Traversal<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction) { throw e; }
        public static IList<AQLTraversalResult<TVertexResult, TEdgeCollection>> Traversal<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction, object options) { throw e; }

        public static IList<TVertexResult> TraversalTree<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction, string connectName) { throw e; }
        public static IList<TVertexResult> TraversalTree<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction, string connectName, object options) { throw e; }

        public static ShortestPathResult<string, string> ShortestPath<TVertexCollection, TEdgeCollection>(string startVertex, string endVertex, EdgeDirection direction) { throw e; }
        public static ShortestPathResult<TVertexResult, TEdgeResult> ShortestPath<TVertexCollection, TEdgeCollection, TVertexResult, TEdgeResult>(string startVertex, string endVertex, EdgeDirection direction, object options) { throw e; }

        public static IList<PathResult<TVertexCollection, TEdgeCollection>> Paths<TVertexCollection, TEdgeCollection>(EdgeDirection direction) { throw e; }
        public static IList<PathResult<TVertexCollection, TEdgeCollection>> Paths<TVertexCollection, TEdgeCollection>(EdgeDirection direction, object options) { throw e; }
        public static IList<PathResult<TVertexResult, TEdgeCollection>> Paths<TVertexCollection, TEdgeCollection, TVertexResult>(EdgeDirection direction) { throw e; }
        public static IList<PathResult<TVertexResult, TEdgeCollection>> Paths<TVertexCollection, TEdgeCollection, TVertexResult>(EdgeDirection direction, object options) { throw e; }

        /*graph*/
        public static IList<string> GraphEdges(string graphName, object vertexExample) { throw e; }
        public static IList<TEdgeResult> GraphEdges<TEdgeResult>(string graphName, object vertexExample, object options) { throw e; }

        public static IList<TVertexResult> GraphVertices<TVertexResult>(string graphName, object vertexExample) { throw e; }
        public static IList<TVertexResult> GraphVertices<TVertexResult>(string graphName, object vertexExample, object options) { throw e; }

        public static IList<string> GraphNeighbors(string graphName, object vertexExample) { throw e; }
        public static IList<TVertexResult> GraphNeighbors<TVertexResult>(string graphName, object vertexExample, object options) { throw e; }

        public static IList<GraphCommonNeighborsResult<string>> GraphCommonNeighbors(string graphName, object vertex1Example, object vertex2Example) { throw e; }
        public static IList<GraphCommonNeighborsResult<TVertexResult>> GraphCommonNeighbors<TVertexResult>(string graphName, object vertex1Example, object vertex2Example, object optionsVertex1, object optionsVertex2) { throw e; }

        public static IList<Dictionary<string, IList<TShareResult>>> GraphCommonProperties<TShareResult>(string graphName, object vertex1Example, object vertex2Example) { throw e; }
        public static IList<Dictionary<string, IList<TShareResult>>> GraphCommonProperties<TShareResult>(string graphName, object vertex1Example, object vertex2Example, object options) { throw e; }

        public static IList<PathResult<TVertexResult, TEdgeResult>> GraphPaths<TVertexResult, TEdgeResult>(string graphName) { throw e; }
        public static IList<PathResult<TVertexResult, TEdgeResult>> GraphPaths<TVertexResult, TEdgeResult>(string graphName, object options) { throw e; }

        public static IList<ShortestPathResult<string, string>> GraphShortestPath(string graphName, object startVertexExample, object endVertexExample) { throw e; }
        public static IList<ShortestPathResult<TVertexResult, TEdgeResult>> GraphShortestPath<TVertexResult, TEdgeResult>(string graphName, object startVertexExample, object endVertexExample, object options) { throw e; }

        public static IList<IList<AQLTraversalResult<TVertexResult, object>>> GraphTraversal<TVertexResult>(string graphName, object startVertexExample, EdgeDirection direction) { throw e; }
        public static IList<IList<AQLTraversalResult<TVertexResult, TEdgeResult>>> GraphTraversal<TVertexResult, TEdgeResult>(string graphName, object startVertexExample, EdgeDirection direction, object options) { throw e; }

        public static IList<TVertexResult> GraphTraversalTree<TVertexResult>(string graphName, object startVertexExample, EdgeDirection direction, string connectName) { throw e; }
        public static IList<TVertexResult> GraphTraversalTree<TVertexResult>(string graphName, object startVertexExample, EdgeDirection direction, string connectName, object options) { throw e; }

        public static IList<GraphDistanceToResult> GraphDistanceTo(string graphName, object startVertexExample, object endVertexExample) { throw e; }
        public static IList<GraphDistanceToResult> GraphDistanceTo(string graphName, object startVertexExample, object endVertexExample, object options) { throw e; }

        public static Dictionary<string, double> GraphAbsoluteEccentricity(string graphName, object vertexExample) { throw e; }
        public static Dictionary<string, double> GraphAbsoluteEccentricity(string graphName, object vertexExample, object options) { throw e; }

        public static Dictionary<string, double> GraphEccentricity(string graphName) { throw e; }
        public static Dictionary<string, double> GraphEccentricity(string graphName, object options) { throw e; }

        public static Dictionary<string, double> GraphAbsoluteCloseness(string graphName, object vertexExample) { throw e; }
        public static Dictionary<string, double> GraphAbsoluteCloseness(string graphName, object vertexExample, object options) { throw e; }

        public static Dictionary<string, double> GraphCloseness(string graphName) { throw e; }
        public static Dictionary<string, double> GraphCloseness(string graphName, object options) { throw e; }

        public static Dictionary<string, double> GraphAbsoluteBetweenness(string graphName, object vertexExample) { throw e; }
        public static Dictionary<string, double> GraphAbsoluteBetweenness(string graphName, object vertexExample, object options) { throw e; }

        public static Dictionary<string, double> GraphBetweenness(string graphName) { throw e; }
        public static Dictionary<string, double> GraphBetweenness(string graphName, object options) { throw e; }

        public static double GraphRadius(string graphName) { throw e; }
        public static double GraphRadius(string graphName, object options) { throw e; }

        public static double GraphDiameter(string graphName) { throw e; }
        public static double GraphDiameter(string graphName, object options) { throw e; }
    }
}
