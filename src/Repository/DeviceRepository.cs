using DTO;
using Microsoft.EntityFrameworkCore;
using Models;
using Repository.Interfaces;

namespace Repository;

public class DeviceRepository : IDeviceRepository
{
    private readonly MasterContext _context;

    public DeviceRepository(MasterContext context)
            {
                _context = context;
            }

    


    public async Task<List<Device>> GetDevices()
    {
        
        var deviceInfo = _context.Devices.Select(d => new Device
        {
            Id = d.Id,
            Name = d.Name,
        });
        return await deviceInfo.ToListAsync();

    }
    
    public async Task<Device> GetDevicesById(int deviceId)
    {
        return (await _context.Devices
            .Include(x => x.DeviceType)
            .Include(emp => emp.DeviceEmployees)
            .ThenInclude(emp => emp.Employee)
            .ThenInclude(p => p.Person)
            .FirstOrDefaultAsync(e => e.Id == deviceId))!;
    }

    public async Task<Device> CreateDevice(Device device)
    {
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        return device;
        
    }

    public async Task UpdateDevice(Device device)
    {
        
        _context.Entry(device).State = EntityState.Modified;
        await _context.SaveChangesAsync();  
    }

    public async Task DeleteDevice(int id)
    {
        
        var device = await _context.Devices.FindAsync(id);
        if (device != null)
        {
            _context.Devices.Remove(device);   
            await _context.SaveChangesAsync();
        }
        
    }

    public async Task<DeviceType?> GetDeviceTypeByName(string name)
    {
        return await _context.DeviceTypes.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<List<GetAllDeviceTypesDto>> GetDeviceTypes()
    {
        
        var types =  _context.DeviceTypes.Select(x => new GetAllDeviceTypesDto()
        {
            Id = x.Id,
            Name = x.Name,
        });

        return await types.ToListAsync();

    }
    public async Task<bool> IsDeviceOwnedByUser(int deviceId, int employeeId)
    {
        return await _context.DeviceEmployees
            .AnyAsync(de => de.DeviceId == deviceId && de.EmployeeId == employeeId);
    }
    
}