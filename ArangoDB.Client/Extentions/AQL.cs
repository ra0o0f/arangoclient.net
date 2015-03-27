using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public class AQL
    {
        static Exception e = new InvalidOperationException("Aql Functions should only be used in ArangoDatabase.Query");

        /*type cast*/
        public static bool ToBool(object value) { throw e; }
        public static int ToNumber(object value) { throw e; }
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
        public static string Concat(string str1,string str2) { throw e; }
        public static string Concat(string str1, string str2, string str3) { throw e; }
        public static string Concat(string str1, string str2, string str3, string str4) { throw e; }
        public static string Concat(string str1, string str2, string str3, string str4, string str5) { throw e; }
        public static string ConcatSeparator(string separator,string str1,string str2) { throw e; }
        public static string ConcatSeparator(string separator,string str1, string str2, string str3) { throw e; }
        public static string ConcatSeparator(string separator,string str1, string str2, string str3, string str4) { throw e; }
        public static string ConcatSeparator(string separator,string str1, string str2, string str3, string str4, string str5) { throw e; }
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
        public static int Floor(double value) { throw e; }
        public static int Ceil(double value) { throw e; }
        public static int Round(double value) { throw e; }
        public static double Abs(double value) { throw e; }
        public static double Sqrt(double value) { throw e; }
        public static double Rand(double value) { throw e; }

        /*object-document*/
        public static T Merge<T>(object document1, object document2) { throw e; }
    }
}
