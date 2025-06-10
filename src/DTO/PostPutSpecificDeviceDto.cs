using System.Text.Json;

namespace DTO;

public class PostPutSpecificDeviceDto
{
    public string DeviceName { get; set; } = null!;
    public int DeviceTypeId { get; set; }
    public bool IsEnabled { get; set; }
    public JsonElement AdditionalProperties { get; set; }
}