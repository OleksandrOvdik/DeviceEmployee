using DTO.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interfaces;

namespace RestApi.Controllers; 

    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly PasswordHasher<Account> _passwordHasher = new (); 
        private readonly MasterContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(MasterContext context, IAccountService accountService, ILogger<AccountController> logger)
        {
            _context = context;
            _accountService = accountService;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            try
            {
                _logger.LogInformation("Getting all accounts in AccountController");
                var accounts = await _accountService.GetAllAcounts();
                return Ok(accounts);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Account not found in AccountController");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while getting all accounts in AccountController");
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            if (User.IsInRole("Admin"))
            {
                try
                {
                    _logger.LogInformation("Getting account in AccountController as Admin");
                    var result = await _accountService.GetAccountById(id);
                    return Ok(result);
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogWarning("Account not found in AccountController");
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error occured while getting account in AccountController");
                    return BadRequest(ex.Message);
                }
            }

            if (User.IsInRole("User"))
            {
                var userIdFromTokenString = User.FindFirst("accountId")?.Value;
                if (string.IsNullOrEmpty(userIdFromTokenString) || 
                    !int.TryParse(userIdFromTokenString, out var userIdFromToken))
                {
                    _logger.LogWarning("Error occured while getting device as User with id: {0}", userIdFromTokenString);
                    return Unauthorized("Invalid user ID claim.");
                }

                if (userIdFromToken != id)
                {
                    return Forbid();
                }

                try
                {
                    _logger.LogInformation("Getting account in AccountController as User");
                    var result = await _accountService.GetAccountById(id);
                    return Ok(result);
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogWarning("Account not found in AccountController");
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error occured while getting account in AccountController");
                    return BadRequest(ex.Message);
                }
            }
            return Forbid();
        }
        

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> PutAccount(
            int id,
            [FromBody] UpdateAccountRequest dto)
        {
            if (User.IsInRole("Admin"))
            {
                try
                {
                    _logger.LogInformation("Updating account in AccountController as Admin");   
                    await _accountService.UpdateAccount(id, dto.AdminPart);
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogWarning("Account or role not found in AccountController");
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error occured while updating account in AccountController");
                    return BadRequest(ex.Message);
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

                if (userIdFromToken != id)
                {
                    return Forbid();
                }

                try
                {
                    _logger.LogInformation("Updating account in AccountController as User");
                    await _accountService.UpdateUserAccount(id, dto.UserPart);
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogWarning("Account or role not found in AccountController");
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error occured while updating account in AccountController");
                    return BadRequest(ex.Message);
                }
            }
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dto));
            return Forbid();
        }


        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("/api/accounts")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Account>> PostAccount(CreateAccountDto newAccount)
        {
            try
            {
                _logger.LogInformation("Creating new account in AccountController");
                var result = await _accountService.CreateAccount(newAccount);
                return Ok(result);
            }
            catch (NullReferenceException e)
            {
                _logger.LogWarning("Account has null");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured while creating new account");
                return BadRequest();
            }
            
            
            
            
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                _logger.LogInformation("Deleting account in AccountController");
                await _accountService.DeleteAccount(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Account not found in AccountController");
                return NotFound(ex.Message);
            }
        }

        // private bool AccountExists(int id)
        // {
        //     return _context.Accounts.Any(e => e.Id == id);
        // }
        
        // [HttpPost("debug/putAccountRequest")]
        // public IActionResult DebugPutAccountRequest([FromBody] UpdateAccountRequest dto)
        // {
        //     if (dto == null)
        //         return BadRequest("DTO is null");
        //
        //     return Ok(new 
        //     {
        //         adminPartIsNull = dto.AdminPart == null,
        //         userPartIsNull  = dto.UserPart == null,
        //         adminPart      = dto.AdminPart,
        //         userPart       = dto.UserPart
        //     });
        // }

        
    }
