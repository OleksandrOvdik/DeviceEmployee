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

    public async Task<GetSpecificDeviceDto> GetDevicesById(int id)
    {
    
        var device = await _deviceRepository.GetDevicesById(id);
        
        if(device == null) throw new KeyNotFoundException($"Device with id {id} not found");

        var currentEmployee = device.DeviceEmployees.FirstOrDefault(emp => emp.ReturnDate == null);

        return new GetSpecificDeviceDto()
        {
            DeviceTypeName = device.DeviceType?.Name ?? "Хуй зна",
            IsEnabled = device.IsEnabled,
            AdditionalProperties = JsonDocument.Parse(device.AdditionalProperties).RootElement,
            DeviceName = device.Name
            // CurrentEmployee = currentEmployee is null
            //     ? null
            //     : new EmployeeDto()
            //     {
            //         Id = currentEmployee.EmployeeId,
            //         Name =
            //             $"{currentEmployee.Employee.Person.FirstName} {currentEmployee.Employee.Person.MiddleName} {currentEmployee.Employee.Person.LastName}"
            //     }

        };

    }

    public async Task<PostPutSpecificDeviceDto> CreateDevice(PostPutSpecificDeviceDto specificDeviceDto)
    {
        var device = new Device()
        {
            Name = specificDeviceDto.DeviceName,
            DeviceTypeId = specificDeviceDto.DeviceTypeId,   
            IsEnabled = specificDeviceDto.IsEnabled,
            AdditionalProperties = JsonSerializer.Serialize(specificDeviceDto.AdditionalProperties)
        };
        
        var newDevice = await _deviceRepository.CreateDevice(device);

        return new PostPutSpecificDeviceDto()
        {
            DeviceName = newDevice.Name,
            DeviceTypeId = newDevice.Id,
            IsEnabled = newDevice.IsEnabled,
            AdditionalProperties = JsonDocument.Parse(newDevice.AdditionalProperties).RootElement,
        };

    }

    public async Task UpdateDevice(int id, PostPutSpecificDeviceDto specificDeviceDto)
    {
        var device = await _deviceRepository.GetDevicesById(id);
        if (device == null) throw new KeyNotFoundException($"Device with id {id} not found");
        
        device.Name = specificDeviceDto.DeviceName;
        device.DeviceTypeId = specificDeviceDto.DeviceTypeId;
        device.IsEnabled = specificDeviceDto.IsEnabled;
        device.AdditionalProperties = JsonSerializer.Serialize(specificDeviceDto.AdditionalProperties);
        
        await _deviceRepository.UpdateDevice(device);
    }

    public async Task DeleteDevice(int id)
    {
        var result =  _deviceRepository.DeleteDevice(id);
        if (result == null) throw new KeyNotFoundException($"Device with id {id} not found");
        await result;
    }

    public async Task<bool> IsDeviceOwnedByUser(int deviceId, int employeeId)
    {
        return await _deviceRepository.IsDeviceOwnedByUser(deviceId, employeeId);
    }
}