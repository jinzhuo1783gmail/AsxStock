using System.Formats.Asn1;
using System.Globalization;
using Newtonsoft.Json;

namespace Stock.DataCenter.Others.Convertor
{
    public class YahooDateTimeConverter : JsonConverter
    {
        private const string DateFormat = "dd-MM-yyyy";
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dateStr = reader.Value.ToString();
            return DateTime.ParseExact(dateStr, DateFormat, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var date = (DateTime)value;
            writer.WriteValue(date.ToString(DateFormat));
        }
    }
}
