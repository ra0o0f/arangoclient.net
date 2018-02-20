using System;

namespace ArangoDB.Client.Test.Model
{
    public class DateModel
    {
        public string IsoDateAsString { get; set; }
        public DateTime IsoDateAsDateTime { get; set; }
        public string IsoDateTimeAsString { get; set; }
        public DateTime IsoDateTimeAsDateTime { get; set; }
        public DateTimeOffset IsoDateTimeAsDateTimeOffset { get; set; }
        public DateTimeOffset IsoDateTimeWithOffsetAsDateTimeOffset { get; set; }
    }
}