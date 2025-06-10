using DTO;
namespace Services.Interfaces;

public interface IDeviceService
{
    
    Task<List<AllDevicesDto>> GetDevices();
    
    Task<GetSpecificDeviceDto> GetDevicesById(int id);
    
    Task<PostPutSpecificDeviceDto> CreateDevice(PostPutSpecificDeviceDto specificDeviceDto);
    
    Task UpdateDevice(int id, PostPutSpecificDeviceDto specificDeviceDto);
    
    Task DeleteDevice(int id);
    
    Task<bool> IsDeviceOwnedByUser(int deviceId, int employeeId);
    
    
    
}