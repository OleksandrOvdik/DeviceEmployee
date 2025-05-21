using System.Text.Json;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;

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
    
    public async Task<Device?> GetDevicesById(int deviceId)
    {
        return await _context.Devices
            .Include(x => x.DeviceType)
            .Include(emp => emp.DeviceEmployees)
            .ThenInclude(emp => emp.Employee)
            .ThenInclude(p => p.Person)
            .FirstOrDefaultAsync(e => e.Id == deviceId);

        
    }

    public async Task<Device> CreateDevice(Device device)
    {
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        return device;
        
    }

    public async Task UpdateDevice(CreateUpdateDeviceDto device)
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
    
    
}