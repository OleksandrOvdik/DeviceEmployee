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

        return new GetSpecificDeviceDto()
        {
            Name = device.Name,
            IsEnabled = device.IsEnabled,
            AdditionalProperties = JsonDocument.Parse(device.AdditionalProperties).RootElement,
            Type = device.DeviceType?.Name ?? "Хуй зна",
        };
    }

    public async Task<List<GetAllDeviceTypesDto>> GetAllDeviceTypes()
    {
        var result = await _deviceRepository.GetDeviceTypes();
        if (result == null) throw new KeyNotFoundException("No devices found");
        return result.Select(x => new GetAllDeviceTypesDto()
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
        
    }

    public async Task<PostPutSpecificDeviceDto> CreateDevice(PostPutSpecificDeviceDto specificDeviceDto)
    {
        var device = new Device()
        {
            Name = specificDeviceDto.Name,
            DeviceTypeId = specificDeviceDto.TypeId,   
            IsEnabled = specificDeviceDto.IsEnabled,
            AdditionalProperties = JsonSerializer.Serialize(specificDeviceDto.AdditionalProperties)
        };
        
        var newDevice = await _deviceRepository.CreateDevice(device);

        return new PostPutSpecificDeviceDto()
        {
            Name = newDevice.Name
        };

    }

    public async Task UpdateDevice(int id, PostPutSpecificDeviceDto specificDeviceDto)
    {
        var device = await _deviceRepository.GetDevicesById(id);
        if (device == null) throw new KeyNotFoundException($"Device with id {id} not found");
        
        device.Name = specificDeviceDto.Name;
        device.DeviceTypeId = specificDeviceDto.TypeId;
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