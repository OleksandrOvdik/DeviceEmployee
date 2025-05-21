using DTO;
namespace Services.Interfaces;

public interface IDeviceService
{
    
    Task<List<AllDevicesDto>> GetDevices();
    
    Task<DeviceDto> GetDevicesById(int id);
    
    Task<CreateUpdateDeviceDto> CreateDevice(CreateUpdateDeviceDto deviceDto);
    
    Task UpdateDevice(int id, CreateUpdateDeviceDto deviceDto);
    
    Task DeleteDevice(int id);
    
}