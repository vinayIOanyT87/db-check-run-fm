using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FMBusinessServices.InternalClasses
{
    internal class DoubleJsonConverter : JsonConverter<double>
    {
        public override double Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
                double.Parse(reader.GetString());

        // rounds the decimal representation of the double, which can look strange to users
        public override void Write(
            Utf8JsonWriter writer,
            double doubleValue,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(doubleValue.ToString("N3", CultureInfo.InvariantCulture));
    }
}
