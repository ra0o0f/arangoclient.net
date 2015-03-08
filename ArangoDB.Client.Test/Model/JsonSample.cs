using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Model
{
    public class JsonSample
    {
        public static string SingleResult
        {
            get
            {
                return @"
{
'Fullname':'raoof hojat',
'Age':27,
'Height':172,
'_id':'Person/KEY',
'_rev':'REV',
'_key':'KEY'
}
"; ;
            }
        }

        // return code changed to 202 to test if its get parsed in success result
        public static string MergeResult
        {
            get
            {
                return @"
{
'error':false,
'code':202,
'Airline':'AIRLINE',
'FlightNumber':'10012314',
'_id':'Flight/KEY',
'_rev':'REV',
'_key':'KEY'
}
";
            }
        }

        public static string NestedSingleResult
        {
            get
            {
                return @"
{
'error':false,
'code':200,
'document':
{
'Fullname':'raoof hojat',
'Age':27,
'Height':172,
'_id':'Person/KEY',
'_rev':'REV',
'_key':'KEY'
}
}
"; ;
            }
        }

        public static string Error
        {
            get
            {
                return @"
{
'error' : true, 
'errorMessage' : 'ERROR', 
'code' : 400, 
'errorNum' : 1202 
}";
            }
        }

        public static string ListResult
        {
            get
            {
                return @"
{
'error':false,
'code':200,
'document':
[{
'Fullname':'raoof hojat',
'Age':27,
'Height':172,
'_id':'Person/KEY1',
'_rev':'REV1',
'_key':'KEY1'
},
{
'Fullname':'hojat raoof',
'Age':7,
'Height':721,
'_id':'Person/KEY2',
'_rev':'REV2',
'_key':'KEY2'
}]
}
"; ;
            }
        }

        public static string CursorFirstResult_WithStatsComesFirst
        {
            get
            {
                return @"
{
'error':false,
'code':202,
'id':'26011191',
'count':2,
'hasMore':true,
'extra':{'stats':{
'writesExecuted' : 1,
'writesIgnored' : 2,
'scannedFull' : 3,
'scannedIndex' : 4,
'fullCount':6
}},
'result':
[{
'Fullname':'raoof hojat2',
'Age':27,
'Height':172,
'_id':'Person/KEY3',
'_rev':'REV3',
'_key':'KEY3'
},
{
'Fullname':'hojat raoof2',
'Age':7,
'Height':721,
'_id':'Person/KEY4',
'_rev':'REV4',
'_key':'KEY4'
}]
}
";
            }
        }

        public static string CursorLastResult_WithStatsComesFirst
        {
            get
            {
                return @"
{
'error':false,
'code':201,
'id':'26011191',
'count':2,
'hasMore':false,
'extra':{'stats':{
'writesExecuted' : 1,
'writesIgnored' : 2,
'scannedFull' : 3,
'scannedIndex' : 4,
'fullCount':6
}},
'result':
[{
'Fullname':'raoof hojat',
'Age':27,
'Height':172,
'_id':'Person/KEY1',
'_rev':'REV1',
'_key':'KEY1'
},
{
'Fullname':'hojat raoof',
'Age':7,
'Height':721,
'_id':'Person/KEY2',
'_rev':'REV2',
'_key':'KEY2'
}]
}
";
            }
        }

        public static string CursorLastResult_WithStatsInTheMiddle
        {
            get
            {
                return @"
{
'error':false,
'code':201,
'id':'26011191',
'result':
[{
'Fullname':'raoof hojat',
'Age':27,
'Height':172,
'_id':'Person/KEY1',
'_rev':'REV1',
'_key':'KEY1'
},
{
'Fullname':'hojat raoof',
'Age':7,
'Height':721,
'_id':'Person/KEY2',
'_rev':'REV2',
'_key':'KEY2'
}],
'count':2,
'hasMore':false,
'extra':{'stats':{
'writesExecuted' : 1,
'writesIgnored' : 2,
'scannedFull' : 3,
'scannedIndex' : 4,
'fullCount':6
}}
}
";
            }
        }

        public static string CursorLastResult_WithStatsComesLast
        {
            get
            {
                return @"
{
'result':
[{
'Fullname':'raoof hojat',
'Age':27,
'Height':172,
'_id':'Person/KEY1',
'_rev':'REV1',
'_key':'KEY1'
},
{
'Fullname':'hojat raoof',
'Age':7,
'Height':721,
'_id':'Person/KEY2',
'_rev':'REV2',
'_key':'KEY2'
}],
'error':false,
'code':201,
'id':'26011191',
'count':2,
'hasMore':false,
'extra':{'stats':{
'writesExecuted' : 1,
'writesIgnored' : 2,
'scannedFull' : 3,
'scannedIndex' : 4,
'fullCount':6
}}
}
";
            }
        }
    }
}
