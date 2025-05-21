using System.Text.Json;
using Models;

namespace DTO;

public class DeviceDto
{
    public string DeviceTypeName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public JsonElement AdditionalProperties { get; set; }
    public EmployeeDto? CurrentEmployee { get; set; } = null!;
}