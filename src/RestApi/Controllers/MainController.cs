using System.Security.Claims;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Interfaces;
namespace RestApi.Controllers;

[ApiController]
[Route("/api")]
public class MainController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly IEmployeeService _employeeService;
    private readonly IAccountService _accountService;
    private readonly ILogger<MainController> _logger;
    
    public MainController(IDeviceService deviceService, ILogger<MainController> logger, IEmployeeService employeeService, IAccountService accountService)
    {
        _deviceService = deviceService;
        _logger = logger;
        _employeeService = employeeService;
        _accountService = accountService;
    }
    
    [HttpGet]
    [Route("devices")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<AllDevicesDto>>> GetDevices()
    {
        try
        {
            var devices = await _deviceService.GetDevices();
            _logger.LogInformation("Called Get devices endpoint in MainController");
            return Ok(devices);
        }
        catch(FileNotFoundException ex)
        {
            _logger.LogWarning(ex, "User do not get an information: {0},", ex.Message);
            return NotFound();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting devices endpoint: {0}", e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Route("devices/{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<DeviceDto>> GetDeviceById(int id)
    {

        if (User.IsInRole("Admin"))
        {
            try
            {
                var result = await _deviceService.GetDevicesById(id);
                _logger.LogInformation("Called Get devices endpoint in MainController as Admin");
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning(e, "Device with id {id} not found", id);
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while getting device as Admin with id: {0}", e.Message);
                return BadRequest(e.Message);
            }
        }

        if (User.IsInRole("User"))
        {
            var userIdFromTokenString = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(userIdFromTokenString) || 
                !int.TryParse(userIdFromTokenString, out var userIdFromToken))
            {
                _logger.LogWarning("Error occured while getting device as User with id: {0}", userIdFromTokenString);
                return Unauthorized("Invalid user ID claim.");
            }

            bool isDeviceOwnedByUser = await _deviceService.IsDeviceOwnedByUser(id, userIdFromToken);

            if (!isDeviceOwnedByUser)
            {
                return Forbid();
            }

            try
            {
                var result = await _deviceService.GetDevicesById(id);
                _logger.LogInformation("Called Get devices by id endpoint in MainController as User");
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Device with id {id} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting device as User with id: {0}", id);
                return BadRequest(ex.Message);
            }
        }
        return Forbid();
    }
    
    
    [HttpPost]
    [Route("devices")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PostPutSpecificDeviceDto>> CreateDevice(PostPutSpecificDeviceDto specificDeviceDto)
    {
        try
        {
            _logger.LogInformation("Called CreateDevice endpoint in MainController");
            await _deviceService.CreateDevice(specificDeviceDto);
            return NoContent();
        }
        catch (NullReferenceException e)
        {
            _logger.LogError("Error occured, because of null in device {specificDeviceDto}", specificDeviceDto);
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while adding new device to MainController");
            return BadRequest(e.Message);
        }
    }
    
    
    [HttpPut("devices/{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> UpdateDevice(int id, PostPutSpecificDeviceDto specificDeviceDto)
    {
        if (User.IsInRole("Admin"))
        {
            try
            {
                _logger.LogInformation("Called Update device endpoint in MainController as Admin");
                await _deviceService.UpdateDevice(id, specificDeviceDto);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning(e, "Device with id {id} not found", id);
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while updating new device to MainController");
                return BadRequest(e.Message);
            }
        }

        if (User.IsInRole("User"))
        {
            var userIdFromTokenString = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(userIdFromTokenString) || 
                !int.TryParse(userIdFromTokenString, out var userIdFromToken))
            {
                _logger.LogWarning("Error occured while getting device as User with id: {0}", userIdFromTokenString);
                return Unauthorized("Invalid user ID claim.");
            }

            bool isDeviceOwnedByUser = await _deviceService.IsDeviceOwnedByUser(id, userIdFromToken);

            if (!isDeviceOwnedByUser)
            {
                return Forbid();
            }

            try
            {
                _logger.LogInformation("Called Update device endpoint in MainController as User");
                await _deviceService.UpdateDevice(id, specificDeviceDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Device with id {id} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while updating device to MainController");
                return BadRequest(ex.Message);
            }
        }
        return Forbid();
    }

    [HttpDelete("devices/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        try
        {
            _logger.LogInformation("Called Delete device endpoint in MainController");
            await _deviceService.DeleteDevice(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Device with id {id} not found", id);
            return NotFound();
        }
    }


    [HttpGet]
    [Route("employees")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetEmployees()
    {
        try
        {
            _logger.LogInformation("Called GetEmployees endpoint in MainController");
            var employees = await _employeeService.GetAllEmployees();
            return Ok(employees);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning(e, "Employee not found in MainController");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting all employees");
            return NotFound(e.Message);
        }
    }
    
    
    
    [HttpGet]
    [Route("employees/{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetEmployeeById(int id)   
    {

        if (User.IsInRole("Admin"))
        {
            try
            {
                _logger.LogInformation("Called GetEmployee endpoint in MainController as Admin");
                var result = await _employeeService.GetEmployeeById(id);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning(e, "Employee not found in MainController");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while getting employee from MainController");
                return BadRequest(e.Message);
            }
            
        }

        if (User.IsInRole("User"))
        {
            try
            {
                var userIdFromTokenString = User.FindFirst("employeeId")?.Value;
                if (string.IsNullOrEmpty(userIdFromTokenString) || 
                    !int.TryParse(userIdFromTokenString, out var userIdFromToken))
                {
                    _logger.LogWarning("Error occured while getting device as User with id: {0}", userIdFromTokenString);
                    return Unauthorized("Invalid user ID claim.");
                }

                if (userIdFromToken != id)
                    return Forbid();     
                
                _logger.LogInformation("Called GetEmployee endpoint in MainController as User");
                var employeeDto = await _employeeService.GetEmployeeById(id);
                return Ok(employeeDto);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning(e, "Employee not found in MainController");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while getting employee from MainController");
                return BadRequest(e.Message);
            }
        }
       return Forbid();
    }

    [HttpPost("employees")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Employee>> PostEmployee(PostPutSpecificEmployee newEmployee)
    {
        try
        {
            _logger.LogInformation("Called PostEmployee endpoint in MainController");
            var result = await _employeeService.CreateEmployee(newEmployee);
            return Ok(result);
        }
        catch (NullReferenceException e)
        {
            _logger.LogWarning(e, "Employee null in MainController");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while posting employee to MainController");
            return BadRequest();
        }
    }
    
    
    // [HttpGet("debug/claims")]
    // [Authorize]
    // public IActionResult DebugClaims()
    // {
    //     var claimsList = User.Claims
    //         .Select(c => new { c.Type, c.Value })
    //         .ToList();
    //     return Ok(claimsList);
    // }

    [HttpGet("positions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPositions()
    {
        try
        {
            _logger.LogInformation("Called GetPositions endpoint in MainController");
            var result = await _employeeService.GetAllPositions();
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning(e, "Employee not found in MainController");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting all positions");
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            _logger.LogInformation("Called GetRoles endpoint in MainController");
            var result = await _employeeService.GetAllRoles();
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning(e, "Employee not found in MainController");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting all roles");
            return BadRequest(e.Message);
        }
    }

    [HttpGet("devices/types")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDeviceTypes()
    {
        try
        {
            _logger.LogInformation("Called GetDeviceTypes endpoint in MainController");
            var result = await _deviceService.GetAllDeviceTypes();
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning(e, "Employee not found in MainController");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting all device");
            return BadRequest(e.Message);
        }
    }
    
}

