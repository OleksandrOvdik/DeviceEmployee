using DTO.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interfaces;

namespace RestApi.Controllers; 

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly PasswordHasher<Account> _passwordHasher = new (); 
        private readonly MasterContext _context;

        public AccountController(MasterContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAcounts();
                return Ok(accounts);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            try
            {
                var account = await _accountService.GetAccountById(id);
                return Ok(account);
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
                    await _accountService.UpdateAccount(id, dto.AdminPart);
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

            if (User.IsInRole("User"))
            {
                var userIdFromTokenString = User.FindFirst("employeeId")?.Value;
                if (string.IsNullOrEmpty(userIdFromTokenString) || 
                    !int.TryParse(userIdFromTokenString, out var userIdFromToken))
                {
                    return Unauthorized("Invalid user ID claim.");
                }

                if (userIdFromToken != id)
                {
                    return Forbid();
                }

                try
                {
                    await _accountService.UpdateUserAccount(id, dto.UserPart);
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


        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("/api/accounts")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Account>> PostAccount(CreateAccountDto newAccount)
        {
            try
            {
                var result = await _accountService.CreateAccount(newAccount);
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

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                await _accountService.DeleteAccount(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
        
        [HttpPost("debug/putAccountRequest")]
        public IActionResult DebugPutAccountRequest([FromBody] UpdateAccountRequest dto)
        {
            if (dto == null)
                return BadRequest("DTO is null");

            // Подивимося, чи AdminPart та UserPart заповнилися:
            return Ok(new 
            {
                adminPartIsNull = dto.AdminPart == null,
                userPartIsNull  = dto.UserPart == null,
                adminPart      = dto.AdminPart,
                userPart       = dto.UserPart
            });
        }

        
    }
