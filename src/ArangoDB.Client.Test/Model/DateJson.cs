namespace ArangoDB.Client.Test.Model
{
    public class DateJson
    {
        public static string SingleDateModelResult => @"
{
'IsoDateAsString':'1983-10-20',
'IsoDateAsDateTime':'1983-10-20',
'IsoDateTimeAsString':'2017-11-16T00:00:00Z',
'IsoDateTimeAsDateTime':'2017-11-16T02:30:15Z',
'IsoDateTimeAsDateTimeOffset':'2017-11-16T01:00:00Z',
'IsoDateTimeWithOffsetAsDateTimeOffset':'2018-02-14T13:43:12+02:30',
'_id':'DateModel/KEY',
'_rev':'REV',
'_key':'KEY'
}
";
    }
}
        