using System.Text.Json;
using DTO;
using Models;
using Repository.Interfaces;
using Services.Interfaces;

namespace Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<List<AllDevicesDto>> GetDevices()
    {
        
        var result =  await _deviceRepository.GetDevices();
        if (result == null) throw new FileNotFoundException("No devices found");
        
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
            AdditionalProperties = JsonSerializer.Serialize(deviceDto.AdditionalProperties)
        };
        
        var newDevice = await _deviceRepository.CreateDevice(device);

        return new CreateUpdateDeviceDto()
        {
            DeviceName = newDevice.Name,
            DeviceTypeName = newDevice.DeviceType?.Name!,
            IsEnabled = newDevice.IsEnabled,
            AdditionalProperties = JsonDocument.Parse(newDevice.AdditionalProperties).RootElement,
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
        device.AdditionalProperties = JsonSerializer.Serialize(deviceDto.AdditionalProperties);
        
        await _deviceRepository.UpdateDevice(device);
    }

    public async Task DeleteDevice(int id)
    {
        var result =  _deviceRepository.DeleteDevice(id);
        if (result == null) throw new KeyNotFoundException($"Device with id {id} not found");
        await result;
    }
}