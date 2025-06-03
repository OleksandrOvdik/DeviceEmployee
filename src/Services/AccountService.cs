using DTO.Accounts;
using Models;
using Repository.Interfaces;
using Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Services;

public class AccountService : IAccountService
{
    
    private readonly IAccountRepository _accountRepository;
    private readonly PasswordHasher<Account> _passwordHasher = new ();


    public AccountService(IAccountRepository accountRepository)
    {
        this._accountRepository = accountRepository;
    }

    public async Task<CreateAccountDto> CreateAccount(CreateAccountDto accountDto)
    {
        
        var roleName = await _accountRepository.GetRoleByName(accountDto.RoleName); 
        if (roleName == null) throw new KeyNotFoundException($"Role with name {accountDto.RoleName} not found");
        
        var employeeName = await _accountRepository.GetEmployeeByName(accountDto.EmployeeName);
        if (employeeName == null) throw new KeyNotFoundException($"Employee with name {accountDto.EmployeeName} not found");

        var account = new Account()
        {
            Username = accountDto.Username,
            // Password = accountDto.Password,
            Password   = _passwordHasher.HashPassword(null, accountDto.Password),
            EmployeeId = employeeName.Id,
            RoleId = roleName.Id,
        };


        var newAccount = await _accountRepository.CreateAccount(account);
        

        return new CreateAccountDto()
        {
            Username = accountDto.Username,
            // Password = accountDto.Password,
            EmployeeName = newAccount.Employee.Person.FirstName,
            RoleName = newAccount.Role.Name,
        };

    }
}