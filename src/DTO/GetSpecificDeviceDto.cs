using System.Text.Json;

namespace DTO;

public class GetSpecificDeviceDto
{
    public string DeviceName { get; set; } = null!;
    public string DeviceTypeName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public JsonElement AdditionalProperties { get; set; }
    
}