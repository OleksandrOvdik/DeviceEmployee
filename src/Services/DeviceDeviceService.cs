using System.Text.Json;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;
using Models; // Ensures the Employee class is included.
using Repository;

namespace Services;

public class DeviceDeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    

    public DeviceDeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<List<AllDevicesDto>> GetDevices()
    {
        
        var result =  await _deviceRepository.GetDevices();
        return result.Select(device => new AllDevicesDto
        {
            Id = device.Id,
            Name = device.Name
        }).ToList();
    }

    public async Task<DeviceDto> GetDevicesById(int id)
    {
    
        var device = await _deviceRepository.GetDevicesById(id);
        
        if(device == null) throw new KeyNotFoundException($"Device with id {id} not found");

        var currentEmployee = device.DeviceEmployees.FirstOrDefault(emp => emp.ReturnDate == null);

        return new DeviceDto()
        {
            DeviceTypeName = device.DeviceType?.Name ?? "Хуй зна",
            IsEnabled = device.IsEnabled,
            AdditionalProperties = JsonDocument.Parse(device.AdditionalProperties).RootElement,
            CurrentEmployee = currentEmployee is null
                ? null
                : new EmployeeDto()
                {
                    Id = currentEmployee.EmployeeId,
                    Name =
                        $"{currentEmployee.Employee.Person.FirstName} {currentEmployee.Employee.Person.MiddleName} {currentEmployee.Employee.Person.LastName}"
                }

        };

    }

    public async Task<CreateUpdateDeviceDto> CreateDevice(CreateUpdateDeviceDto deviceDto)
    {
        
        var deviceTypeName = await _deviceRepository.GetDeviceTypeByName(deviceDto.DeviceTypeName);
        if (deviceTypeName == null) throw new KeyNotFoundException($"Device type with name {deviceDto.DeviceTypeName} not found");

        var device = new Device()
        {
            Name = deviceDto.DeviceName,
            DeviceTypeId = deviceTypeName.Id,
            IsEnabled = deviceDto.IsEnabled,
            AdditionalProperties = deviceDto.AdditionalProperties,
        };
        
        var newDevice = await _deviceRepository.CreateDevice(device);

        return new CreateUpdateDeviceDto()
        {
            DeviceName = newDevice.Name,
            DeviceTypeName = newDevice.DeviceType.Name,
            IsEnabled = newDevice.IsEnabled,
            AdditionalProperties = newDevice.AdditionalProperties,
        };

    }

    public async Task UpdateDevice(int id, CreateUpdateDeviceDto deviceDto)
    {
        var device = await _deviceRepository.GetDevicesById(id);
        if (device == null) throw new KeyNotFoundException($"Device with id {id} not found");
        
        var deviceTypeName = await _deviceRepository.GetDeviceTypeByName(deviceDto.DeviceTypeName);
        if (device == null) throw new KeyNotFoundException($"Device with id {id} not found");
        
        device.Name = deviceDto.DeviceName;
        device.DeviceType = deviceTypeName;
        device.IsEnabled = deviceDto.IsEnabled;
        device.AdditionalProperties = deviceDto.AdditionalProperties;
        
        await _deviceRepository.UpdateDevice(device);
    }

    public async Task DeleteDevice(int id)
    {
        await _deviceRepository.DeleteDevice(id);
    }
    

    private async Task<string> GenerateId()
    {
        
        MasterContext _context = new MasterContext();
        var lastId = await _context.Devices
            .MaxAsync(device => (int?)device.Id) ?? 0; 
        return (lastId + 1).ToString();
    }
    
}