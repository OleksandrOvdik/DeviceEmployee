using DTO;
using Models;

namespace Repository;

public interface IDeviceRepository
{
    Task<List<Device>> GetDevices();
    
    Task<Device> GetDevicesById(int id);
    
    Task<Device> CreateDevice(Device device);
    
    Task UpdateDevice(CreateUpdateDeviceDto device);
    
    Task DeleteDevice(int id);
    
    
}