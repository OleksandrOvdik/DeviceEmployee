using System.Security.Claims;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return Ok(devices);
        }
        catch(FileNotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
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
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        if (User.IsInRole("User"))
        {
            var userIdFromTokenString = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(userIdFromTokenString) || 
                !int.TryParse(userIdFromTokenString, out var userIdFromToken))
            {
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
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
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
            var result = await _deviceService.CreateDevice(specificDeviceDto);
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    [HttpPut("deivces/{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> UpdateDevice(int id, PostPutSpecificDeviceDto specificDeviceDto)
    {
        if (User.IsInRole("Admin"))
        {
            try
            {
                await _deviceService.UpdateDevice(id, specificDeviceDto);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        if (User.IsInRole("User"))
        {
            var userIdFromTokenString = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(userIdFromTokenString) || 
                !int.TryParse(userIdFromTokenString, out var userIdFromToken))
            {
                return Unauthorized("Invalid user ID claim.");
            }

            bool isDeviceOwnedByUser = await _deviceService.IsDeviceOwnedByUser(id, userIdFromToken);

            if (!isDeviceOwnedByUser)
            {
                return Forbid();
            }

            try
            {
                await _deviceService.UpdateDevice(id, specificDeviceDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
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
            await _deviceService.DeleteDevice(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
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
            var employees = await _employeeService.GetAllEmployees();
            return Ok(employees);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    
    
    [HttpGet]
    [Route("employees/{id}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetEmployeeById(int id)   
    {
        try
        {
            var userIdFromTokenString = User.FindFirst("employeeId")?.Value;
            if (string.IsNullOrEmpty(userIdFromTokenString) || 
                !int.TryParse(userIdFromTokenString, out var userIdFromToken))
            {
                return Unauthorized("Invalid user ID claim.");
            }

            if (userIdFromToken != id)
                return Forbid();     

            var employeeDto = await _accountService.ViewAccountUser(id);
            return Ok(employeeDto);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("debug/claims")]
    [Authorize]
    public IActionResult DebugClaims()
    {
        var claimsList = User.Claims
            .Select(c => new { c.Type, c.Value })
            .ToList();
        return Ok(claimsList);
    }

    [HttpGet("positions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPositions()
    {
        try
        {
            var result = _employeeService.GetAllPositions();
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            var result = _employeeService.GetAllPositions();
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}

