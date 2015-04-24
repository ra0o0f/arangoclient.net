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
        public static string Substitute(string value,string search,string replace,int limit) { throw e; }
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
        public static string[] Split(string value,string separator, int limit) { throw e; }
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
        public static long Floor(double value) { throw e; }
        public static long Ceil(double value) { throw e; }
        public static long Round(double value) { throw e; }
        public static double Abs(double value) { throw e; }
        public static double Sqrt(double value) { throw e; }
        public static double Rand(double value) { throw e; }

        /*date*/
        public static long DateTimestamp(string date) { throw e; }
        public static long DateTimestamp(long date) { throw e; }
        public static long DateTimestamp(DateTime date) { throw e; }
        public static long DateTimestamp(DateTimeOffset date) { throw e; }
        public static long DateTimestamp(int year, int month, int day) { throw e; }
        public static long DateTimestamp(int year, int month, int day, int hour) { throw e; }
        public static long DateTimestamp(int year, int month, int day, int hour, int minute) { throw e; }
        public static long DateTimestamp(int year, int month, int day, int hour, int minute, int second) { throw e; }
        public static long DateTimestamp(int year, int month, int day, int hour, int minute, int second, int millisecond) { throw e; }
        
        public static string DateIso8601(string date) { throw e; } 
        public static string DateIso8601(long date) { throw e; }
        public static string DateIso8601(DateTime date) { throw e; }
        public static string DateIso8601(DateTimeOffset date) { throw e; }
        public static string DateIso8601(int year, int month, int day) { throw e; }
        public static string DateIso8601(int year, int month, int day, int hour) { throw e; }
        public static string DateIso8601(int year, int month, int day, int hour, int minute) { throw e; }
        public static string DateIso8601(int year, int month, int day, int hour, int minute, int second) { throw e; }
        public static string DateIso8601(int year, int month, int day, int hour, int minute, int second, int millisecond) { throw e; }
        public static int DateDayOfWeek(string date) { throw e; }
        public static int DateDayOfWeek(long date) { throw e; }
        public static int DateDayOfWeek(DateTime date) { throw e; }
        public static int DateDayOfWeek(DateTimeOffset date) { throw e; }
        public static int DateYear(string date) { throw e; }
        public static int DateYear(long date) { throw e; }
        public static int DateYear(DateTime date) { throw e; }
        public static int DateYear(DateTimeOffset date) { throw e; }
        public static int DateMonth(string date) { throw e; }
        public static int DateMonth(long date) { throw e; }
        public static int DateMonth(DateTime date) { throw e; }
        public static int DateMonth(DateTimeOffset date) { throw e; }
        public static int DateDay(string date) { throw e; }
        public static int DateDay(long date) { throw e; }
        public static int DateDay(DateTime date) { throw e; }
        public static int DateDay(DateTimeOffset date) { throw e; }
        public static int DateHour(string date) { throw e; }
        public static int DateHour(long date) { throw e; }
        public static int DateHour(DateTime date) { throw e; }
        public static int DateHour(DateTimeOffset date) { throw e; }
        public static int DateMinute(string date) { throw e; }
        public static int DateMinute(long date) { throw e; }
        public static int DateMinute(DateTime date) { throw e; }
        public static int DateMinute(DateTimeOffset date) { throw e; }
        public static int DateSecond(string date) { throw e; }
        public static int DateSecond(long date) { throw e; }
        public static int DateSecond(DateTime date) { throw e; }
        public static int DateSecond(DateTimeOffset date) { throw e; }
        public static int DateMilliSecond(string date) { throw e; }
        public static int DateMilliSecond(long date) { throw e; }
        public static int DateMilliSecond(DateTime date) { throw e; }
        public static int DateMilliSecond(DateTimeOffset date) { throw e; }
        public static long DateNow() { throw e; }

        /*array*/
        public static int Length(object array) { throw e; }
        public static IList<T> Flatten<T>(object array) { throw e; }
        public static IList<T> Flatten<T>(object array, int depth) { throw e; }
        public static T Min<T>(IList<T> array) { throw e; }
        public static T Max<T>(IList<T> array) { throw e; }
        public static T Average<T>(IList<T> array) { throw e; }
        public static T Sum<T>(IList<T> array) { throw e; }
        public static T Median<T>(IList<T> array) { throw e; }
        public static T Percentile<T>(IList<T> array, int n) { throw e; }
        public static T Percentile<T>(IList<T> array, int n, string method) { throw e; }
        public static T VariancePopulation<T>(IList<T> array) { throw e; }
        public static T VarianceSample<T>(IList<T> array) { throw e; }
        public static T StdDevPopulation<T>(IList<T> array) { throw e; }
        public static IList<T> Reverse<T>(IList<T> array) { throw e; }
        public static T First<T>(IList<T> array) { throw e; }
        public static T Last<T>(IList<T> array) { throw e; }
        public static T Nth<T>(IList<T> array, int position) { throw e; }
        public static bool Position<T>(IList<T> array, T search) { throw e; }
        public static object Position<T>(IList<T> array, T search, bool returnIndex) { throw e; }
        public static IList<T> Slice<T>(IList<T> array, int start) { throw e; }
        public static IList<T> Slice<T>(IList<T> array, int start, int length) { throw e; }
        public static IList<T> Unique<T>(IList<T> array) { throw e; }
        public static IList<T> Union<T>(IList<T> array1, IList<T> array2) { throw e; }
        public static IList<T> Union<T>(IList<T> array1, IList<T> array2, IList<T> array3) { throw e; }
        public static IList<T> Union<T>(IList<T> array1, IList<T> array2, IList<T> array3, IList<T> array4) { throw e; }
        public static IList<T> Union<T>(IList<T> array1, IList<T> array2, IList<T> array3, IList<T> array4, IList<T> array5) { throw e; }
        public static IList<T> UnionDistinct<T>(IList<T> array1, IList<T> array2) { throw e; }
        public static IList<T> UnionDistinct<T>(IList<T> array1, IList<T> array2, IList<T> array3) { throw e; }
        public static IList<T> UnionDistinct<T>(IList<T> array1, IList<T> array2, IList<T> array3, IList<T> array4) { throw e; }
        public static IList<T> UnionDistinct<T>(IList<T> array1, IList<T> array2, IList<T> array3, IList<T> array4, IList<T> array5) { throw e; }
        public static IList<T> Minus<T>(IList<T> array1, IList<T> array2) { throw e; }
        public static IList<T> Minus<T>(IList<T> array1, IList<T> array2, IList<T> array3) { throw e; }
        public static IList<T> Minus<T>(IList<T> array1, IList<T> array2, IList<T> array3, IList<T> array4) { throw e; }
        public static IList<T> Minus<T>(IList<T> array1, IList<T> array2, IList<T> array3, IList<T> array4, IList<T> array5) { throw e; }
        public static IList<T> Intersection<T>(IList<T> array1, IList<T> array2) { throw e; }
        public static IList<T> Intersection<T>(IList<T> array1, IList<T> array2, IList<T> array3) { throw e; }
        public static IList<T> Intersection<T>(IList<T> array1, IList<T> array2, IList<T> array3, IList<T> array4) { throw e; }
        public static IList<T> Intersection<T>(IList<T> array1, IList<T> array2, IList<T> array3, IList<T> array4, IList<T> array5) { throw e; }
        public static IList<T> Append<T>(IList<T> array, IList<T> values) { throw e; }
        public static IList<T> Append<T>(IList<T> array, IList<T> values, bool unique) { throw e; }
        public static IList<T> Push<T>(IList<T> array, T value) { throw e; }
        public static IList<T> Push<T>(IList<T> array, T value, bool unique) { throw e; }
        public static IList<T> UnShift<T>(IList<T> array, T value) { throw e; }
        public static IList<T> UnShift<T>(IList<T> array, T value, bool unique) { throw e; }
        public static IList<T> Pop<T>(IList<T> array) { throw e; }
        public static IList<T> Shift<T>(IList<T> array) { throw e; }
        public static IList<T> RemoveValue<T>(IList<T> array, T value) { throw e; }
        public static IList<T> RemoveValue<T>(IList<T> array, T value, int limit) { throw e; }
        public static IList<T> RemoveValues<T>(IList<T> array, IList<T> values) { throw e; }
        public static IList<T> RemoveNth<T>(IList<T> array, int position) { throw e; }

        /*object-document*/
        public static T Merge<T>(object document1, object document2) { throw e; }
        public static T Merge<T>(object document1, object document2, object document3) { throw e; }
        public static T Merge<T>(object document1, object document2, object document3, object document4) { throw e; }
        public static T Merge<T>(object document1, object document2, object document3, object document4, object document5) { throw e; }
        public static bool Matches(object document, IList<object> examples) { throw e; }
        public static object Matches(object document, IList<object> examples, bool returnIndex) { throw e; }
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
        public static T Zip<T>(IList<string> attributes, IList<object> values) { throw e; }
        public static T Unset<T>(object document, params string[] attributeNames) { throw e; }
        public static T Keep<T>(object document, params string[] attributeNames) { throw e; }
        public static ParseIdentifierResult ParseIdentifier(string documentHandle) { throw e; }

        /*geo*/
        public static IList<T> Near<T>(double latitude, double longitude) { throw e; }
        public static IList<T> Near<T>(double latitude, double longitude, int limit) { throw e; }
        //TODO: add distance overload
        //public static IList<T> Near<T>(double latitude, double longitude, int limit, Expression<Func<T, object>> distance) { throw e; }
        public static IList<T> Within<T>(double latitude, double longitude, double radius) { throw e; }
        //TODO: add distance overload
        //public static IList<T> Within<T>(double latitude, double longitude, double radius, Expression<Func<T, object>> distance) { throw e; }
        public static IList<T> WithinRectangle<T>(double latitude1, double longitude1, double latitude2, double longitude2) { throw e; }
        public static bool IsInPolygon(double[][] polygon, double latitude, double longitude) { throw e; }
        public static bool IsInPolygon(double[][] polygon, double[] coordinates) { throw e; }
        public static bool IsInPolygon(double[][] polygon, double[] coordinates, bool reverseCoordinateOrder) { throw e; }

        /*edge*/
        public static IList<TEdgeCollection> Edges<TEdgeCollection>(string startVertex, EdgeDirection direction) { throw e; }
        public static IList<TEdgeCollection> Edges<TEdgeCollection>(string startVertex, EdgeDirection direction, object edgeExample) { throw e; }
        public static IList<EdgeVertexResult<TVertexCollection, TEdgeCollection>> Neighbors<TVertexCollection, TEdgeCollection>(string startVertex, EdgeDirection direction) { throw e; }
        public static IList<EdgeVertexResult<TVertexCollection, TEdgeCollection>> Neighbors<TVertexCollection, TEdgeCollection>(string startVertex, EdgeDirection direction, object edgeExample) { throw e; }
        public static IList<EdgeVertexResult<TVertexResult, TEdgeCollection>> Neighbors<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction) { throw e; }
        public static IList<EdgeVertexResult<TVertexResult, TEdgeCollection>> Neighbors<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction, object edgeExample) { throw e; }
        public static IList<VertexResult<TVertexCollection>> Traversal<TVertexCollection, TEdgeCollection>(string startVertex, EdgeDirection direction) { throw e; }
        public static IList<VertexResult<TVertexCollection>> Traversal<TVertexCollection, TEdgeCollection>(string startVertex, EdgeDirection direction, object options) { throw e; }
        public static IList<VertexResult<TVertexResult>> Traversal<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction) { throw e; }
        public static IList<VertexResult<TVertexResult>> Traversal<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction, object options) { throw e; }
        public static IList<TVertexResult> TraversalTree<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction, string connectName) { throw e; }
        public static IList<TVertexResult> TraversalTree<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, EdgeDirection direction, string connectName, object options) { throw e; }
        public static IList<VertexResult<TVertexCollection>> ShortestPath<TVertexCollection, TEdgeCollection>(string startVertex, string endVertex, EdgeDirection direction) { throw e; }
        public static IList<VertexResult<TVertexCollection>> ShortestPath<TVertexCollection, TEdgeCollection>(string startVertex, string endVertex, EdgeDirection direction, object options) { throw e; }
        public static IList<VertexResult<TVertexResult>> ShortestPath<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, string endVertex, EdgeDirection direction) { throw e; }
        public static IList<VertexResult<TVertexResult>> ShortestPath<TVertexCollection, TEdgeCollection, TVertexResult>(string startVertex, string endVertex, EdgeDirection direction, object options) { throw e; }
        public static IList<PathResult<TVertexCollection, TEdgeCollection>> Paths<TVertexCollection, TEdgeCollection>(EdgeDirection direction) { throw e; }
        public static IList<PathResult<TVertexCollection, TEdgeCollection>> Paths<TVertexCollection, TEdgeCollection>(EdgeDirection direction, object options) { throw e; }
        public static IList<PathResult<TVertexResult, TEdgeCollection>> Paths<TVertexCollection, TEdgeCollection, TVertexResult>(EdgeDirection direction) { throw e; }
        public static IList<PathResult<TVertexResult, TEdgeCollection>> Paths<TVertexCollection, TEdgeCollection, TVertexResult>(EdgeDirection direction, object options) { throw e; }
    }
}
