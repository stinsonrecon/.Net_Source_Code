using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epayment.Models
{
    public class JsonDateConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(String.IsNullOrEmpty(reader.GetString()))
            {
                return DateTime.MinValue;
            }
            else
                return DateTime.ParseExact(reader.GetString(),"dd/MM/yyyy", CultureInfo.InvariantCulture);
        }


        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
       => writer.WriteStringValue(value.ToString(
                    "dd/MM/yyyy", CultureInfo.InvariantCulture));
    }

    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var x = reader.GetString();
            if(!String.IsNullOrEmpty(reader.GetString()))
            {
                return DateTime.ParseExact(reader.GetString(),
                    "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
                return DateTime.MinValue;
        }


        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
       => writer.WriteStringValue(value.ToString(
                    "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
    }
}