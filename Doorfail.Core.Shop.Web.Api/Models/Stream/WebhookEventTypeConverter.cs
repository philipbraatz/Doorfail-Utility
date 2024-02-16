using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doorfail.Core.Shop.Web.Api.Models.Stream;

public class WebhookEventTypeConverter :JsonConverter<WebhookEventType>
{
    public override WebhookEventType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        string enumString = reader.GetString()?.Replace('.', '_');
        if(Enum.TryParse(enumString, out WebhookEventType result))
        {
            return result;
        } else
        {
            throw new JsonException($"Unable to parse '{enumString}' to enum type '{typeof(WebhookEventType).FullName}'.");
        }
    }

    public override void Write(Utf8JsonWriter writer, WebhookEventType value, JsonSerializerOptions options)
    {
        string enumString = value.ToString().Replace('_', '.');
        writer.WriteStringValue(enumString);
    }
}
