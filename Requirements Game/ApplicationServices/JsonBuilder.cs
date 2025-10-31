using System.Collections.Generic;
using System.Text.Json;

/// <summary>
/// A simple utility class for building and serializing key-value pairs into JSON format
/// </summary>
public class JsonBuilder {

    public Dictionary<string, object> Items { get; }

    public JsonBuilder() {

        Items = new Dictionary<string, object>();

    }

    public override string ToString() {

        return JsonSerializer.Serialize(Items);

    }

}

