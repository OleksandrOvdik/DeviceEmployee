using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Tokens;

namespace RestApi.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        private readonly MasterContext _context;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<Account> _passwordHasher = new ();

        public AuthController(MasterContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }


        [HttpPost]
        [Route("/api/auth")]
        public async Task<IActionResult> Auth(LoginUserDto user, CancellationToken cancellationToken)
        {
            var foundUser = await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.Employee)
                .ThenInclude(emp => emp.Person)
                .FirstOrDefaultAsync(a => string.Equals(a.Username, user.Username), 
                cancellationToken);

            if (foundUser == null)
            {
                return Unauthorized();
            }
            
            var verificationResult = _passwordHasher.VerifyHashedPassword(foundUser, foundUser.Password, user.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized();
            }

            var token = new
            {
                AccessToken = _tokenService.GenerateToken(foundUser.Username, foundUser.Role.Name, foundUser.Id),
            };
            
            return Ok(new { AccessToken = token });

        }
        
        
    }
