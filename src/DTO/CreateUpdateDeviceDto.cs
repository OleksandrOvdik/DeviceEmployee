using Models;

namespace DTO;

public class CreateUpdateDeviceDto
{
    public string DeviceName { get; set; } = null!;

    public string DeviceTypeName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public string AdditionalProperties { get; set; } = null!;
}