using DTO;
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

    public async Task<List<AllAcountsDto>> GetAllAcounts()
    {

        var result = await _accountRepository.GetAllAccounts();
        if (result == null) throw new FileNotFoundException("There are no accounts");
        
        return result.Select(account => new AllAcountsDto
        {
            Id = account.Id,
            Username = account.Username,
            Password = account.Password,
        }).ToList();

    }

    public async Task<AccountByIdDto> GetAccountById(int id)
    {
        var account = await _accountRepository.GetAccountById(id);
        if (account == null) throw new FileNotFoundException("There are no accounts");

        return new AccountByIdDto()
        {
            Password = account.Password,
            Username = account.Username,
        };
    }

    public async Task<ViewAccountUserDto> ViewAccountUser(int userId)
    {

        var account = await _accountRepository.ViewAccountUser(userId);

        if (account == null) throw new KeyNotFoundException($"There are no accounts with id {userId}");
        
        
        var employee = account.Employee!;
        var person = employee.Person!;

        return new ViewAccountUserDto()
        {
            AccountId = account.Id,
            Username = account.Username,
            Password = account.Password,
            RoleName = account.Role.Name,

            EmployeeId = employee.Id,
            Salary = employee.Salary,
            PositionId = employee.PositionId,
            HireDate = employee.HireDate,

            PersonId = person.Id,
            PassportNumber = person.PassportNumber,
            FirstName = person.FirstName,
            MiddleName = person.MiddleName,
            LastName = person.LastName,
            Email = person.Email,
            PhoneNumber = person.PhoneNumber,
        };


    }

    public async Task<CreateAccountDto> CreateAccount(CreateAccountDto accountDto)
    {
        
        var roleName = await _accountRepository.GetRoleByName(accountDto.RoleName); 
        if (roleName == null) throw new KeyNotFoundException($"Role with name {accountDto.RoleName} not found");
        
        var employeePassportNumber = await _accountRepository.GetEmployeeByPassportNumber(accountDto.EmployeePassportNumber);
        if (employeePassportNumber == null) throw new KeyNotFoundException($"Employee with name {accountDto.EmployeePassportNumber} not found");

        var account = new Account()
        {
            Username = accountDto.Username,
            // Password = accountDto.Password,
            Password   = _passwordHasher.HashPassword(null, accountDto.Password),
            EmployeeId = employeePassportNumber.Id,
            RoleId = roleName.Id,
        };


        var newAccount = await _accountRepository.CreateAccount(account);
        

        return new CreateAccountDto()
        {
            Username = accountDto.Username,
            // Password = accountDto.Password,
            EmployeePassportNumber = newAccount.Employee.Person.PassportNumber,
            RoleName = newAccount.Role.Name,
        };

    }

    public async Task UpdateAccount(int id, UpdateAccountAdminDto accountAdminDto)
    {
        var roleName = await _accountRepository.GetRoleByName(accountAdminDto.RoleName); 
        if (roleName == null) throw new KeyNotFoundException($"Role with name {accountAdminDto.RoleName} not found");
        
        var account = await _accountRepository.GetAccountById(id);
        if (account == null) throw new FileNotFoundException("There is no account");
        
        var employeePassportNumber = await _accountRepository.GetEmployeeByPassportNumber(accountAdminDto.EmployeePassportNumber);
        if (employeePassportNumber == null) throw new KeyNotFoundException($"Employee with name {accountAdminDto.EmployeePassportNumber} not found");
        
        account.Username = accountAdminDto.Username;
        account.Password = _passwordHasher.HashPassword(null, accountAdminDto.Password);
        account.EmployeeId = employeePassportNumber.Id;
        account.RoleId = roleName.Id;
        
        await _accountRepository.UpdateAccount(account);
        
    }

    public async Task DeleteAccount(int id)
    {
        
        var result = _accountRepository.DeleteAccount(id);
        if (result == null) throw new KeyNotFoundException($"There is no account with id {id}");
        await result;

    }
}