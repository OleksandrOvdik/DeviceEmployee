using Models;

namespace Repository.Interfaces;

public interface IDeviceRepository
{
    Task<List<Device>> GetDevices();
    
    Task<Device> GetDevicesById(int id);
    
    Task<Device> CreateDevice(Device device);
    
    Task UpdateDevice(Device device);
    
    Task DeleteDevice(int id);
    
    Task<DeviceType?> GetDeviceTypeByName(string name);
    
    
}