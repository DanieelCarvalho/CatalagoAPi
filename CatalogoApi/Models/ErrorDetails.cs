using System.Text.Json;

namespace CatalogoApi.Models;

public class ErrorDetails
{

    public string? StatusCode { get; set; }
    public string? Message { get; set; }

    public string? Trace { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}


