using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;

namespace SmartifyOS.SaveSystem
{
    public class ColorConverter : JsonConverter<Color?>
    {
        public override void WriteJson(JsonWriter writer, Color? value, JsonSerializer serializer)
        {
            if (value.HasValue)
            {
                var color = value.Value;
                var obj = new JObject
            {
                { "r", color.r },
                { "g", color.g },
                { "b", color.b },
                { "a", color.a }
            };
                obj.WriteTo(writer);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override Color? ReadJson(JsonReader reader, Type objectType, Color? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var obj = JObject.Load(reader);
            return new Color(
                obj["r"].Value<float>(),
                obj["g"].Value<float>(),
                obj["b"].Value<float>(),
                obj["a"].Value<float>()
            );
        }
    }
}
