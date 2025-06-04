using System.Text.Json;
using Models;

namespace DTO;

public class CreateUpdateDeviceDto
{
    public string DeviceName { get; set; } = null!;

    public string DeviceTypeName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public JsonElement AdditionalProperties { get; set; }
}