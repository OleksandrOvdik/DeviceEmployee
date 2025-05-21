using DTO;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using Services.Interfaces;

namespace RestApi.Controllers;

[ApiController]
[Route("/api")]
public class MainController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<MainController> _logger;
    
    public MainController(IDeviceService deviceService, ILogger<MainController> logger, IEmployeeService employeeService)
    {
        _deviceService = deviceService;
        _logger = logger;
        _employeeService = employeeService;
        
    }
    
    

    [HttpGet]
    [Route("devices")]
    public async Task<ActionResult<List<AllDevicesDto>>> GetDevices()
    {
        var devices = await _deviceService.GetDevices();
        return Ok(devices);
    }

    [HttpGet]
    [Route("devices/{id}")]
    public async Task<ActionResult<DeviceDto>> GetDeviceById(int id)
    {
        try
        {
            var result = await _deviceService.GetDevicesById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    [HttpPost]
    public async Task<ActionResult<CreateUpdateDeviceDto>> CreateDevice(CreateUpdateDeviceDto deviceDto)
    {
        var createdDevice = await _deviceService.CreateDevice(deviceDto);
        return CreatedAtAction(nameof(GetDevices), new {id = createdDevice.Id}, createdDevice);
    }
    
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDevice(int id, CreateUpdateDeviceDto deviceDto)
    {
        try
        {
            await _deviceService.UpdateDevice(id, deviceDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        await _deviceService.DeleteDevice(id);
        return NoContent();
    }


    [HttpGet]
    [Route("employees")]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _employeeService.GetAllEmployees();
        return Ok(employees);
    }
    
}

