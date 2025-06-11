using System.Text.Json;

namespace DTO;

public class PostPutSpecificDeviceDto
{
    public string Name { get; set; } = null!;
    public int TypeId { get; set; }
    public bool IsEnabled { get; set; }
    public JsonElement AdditionalProperties { get; set; }
}