using Models;

namespace DTO;

public class CreateUpdateDeviceDto
{
    public string DeviceName { get; set; }
    
    public string DeviceTypeName { get; set; }
    public bool IsEnabled { get; set; }
    public string AdditionalProperties { get; set; }
}