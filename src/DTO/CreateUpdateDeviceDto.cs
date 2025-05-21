using Models;

namespace DTO;

public class CreateUpdateDeviceDto
{
    
    public int Id { get; set; }
    public string DeviceTypeName { get; set; }
    public bool IsEnabled { get; set; }
    public string AdditionalProperties { get; set; }
    public int? DeviceTypeId { get; set; }
    public string DeviceName { get; set; }
}