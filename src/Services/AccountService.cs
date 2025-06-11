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
        if (result == null) throw new KeyNotFoundException("There are no accounts");
        
        return result.Select(account => new AllAcountsDto
        {
            Id = account.Id,
            Username = account.Username,
        }).ToList();

    }

    public async Task<GetSpecificAccountDto> GetAccountById(int id)
    {
        var account = await _accountRepository.GetAccountById(id);
        if (account == null) throw new KeyNotFoundException("There are no accounts");

        return new GetSpecificAccountDto()
        {
            Username = account.Username,
            Role = account.Role.Name,
        };
    }

    // public async Task<UpdateViewAccountUserDto> ViewAccountUser(int userId)
    // {
    //
    //     var account = await _accountRepository.ViewAccountUser(userId);
    //
    //     if (account == null) throw new KeyNotFoundException($"There are no accounts with id {userId}");
    //     
    //     
    //     var employee = account.Employee!;
    //     var person = employee.Person!;
    //     
    //     return new UpdateViewAccountUserDto()
    //     {
    //         AccountId = account.Id,
    //         Username = account.Username,
    //         Password = account.Password,
    //         RoleName = account.Role.Name,
    //
    //         EmployeeId = employee.Id,
    //         Salary = employee.Salary,
    //         PositionId = employee.PositionId,
    //         HireDate = employee.HireDate,
    //
    //         PersonId = person.Id,
    //         PassportNumber = person.PassportNumber,
    //         FirstName = person.FirstName,
    //         MiddleName = person.MiddleName,
    //         LastName = person.LastName,
    //         Email = person.Email,
    //         PhoneNumber = person.PhoneNumber,
    //     };
    //
    //
    // }

    public async Task<CreateAccountDto> CreateAccount(CreateAccountDto accountDto)
    {

        var account = new Account()
        {
            Username = accountDto.Username,
            Password   = _passwordHasher.HashPassword(null, accountDto.Password),
            EmployeeId = accountDto.EmployeeId,
            RoleId = accountDto.RoleId
        };


        var newAccount = await _accountRepository.CreateAccount(account);
        if (newAccount == null) throw new NullReferenceException("There are no accounts");
        

        return new CreateAccountDto()
        {
            Username = newAccount.Username,
            Password = newAccount.Password,
            EmployeeId = newAccount.EmployeeId,
            RoleId = newAccount.RoleId,
        };

    }

    public async Task UpdateAccount(int id, UpdateAccountAdminDto accountAdminDto)
    {
        var roleName = await _accountRepository.GetRoleByName(accountAdminDto.RoleName); 
        if (roleName == null) throw new KeyNotFoundException($"Role with name {accountAdminDto.RoleName} not found");
        
        var account = await _accountRepository.GetAccountById(id);
        if (account == null) throw new KeyNotFoundException("There is no account");
        
        var employeePassportNumber = await _accountRepository.GetEmployeeByPassportNumber(accountAdminDto.EmployeePassportNumber);
        if (employeePassportNumber == null) throw new KeyNotFoundException($"Employee with name {accountAdminDto.EmployeePassportNumber} not found");
        
        account.Username = accountAdminDto.Username;
        account.Password = _passwordHasher.HashPassword(null, accountAdminDto.Password);
        account.EmployeeId = employeePassportNumber.Id;
        account.RoleId = roleName.Id;
        
        await _accountRepository.UpdateAccount(account);
        
    }

    public async Task UpdateUserAccount(int id, UpdateViewAccountUserDto userAccountDto)
    {
        
        var account = await _accountRepository.GetAccountById(id);
        if (account == null) throw new FileNotFoundException("There is no account");
        
        account.Username = userAccountDto.Username;
        account.Password = _passwordHasher.HashPassword(null, userAccountDto.Password);
        
        account.EmployeeId = userAccountDto.EmployeeId;
        account.Employee.Salary = userAccountDto.Salary;
        account.Employee.PositionId = userAccountDto.PositionId;
        account.Employee.HireDate = userAccountDto.HireDate;
        
        account.Employee.PersonId = userAccountDto.PersonId;
        account.Employee.Person.PassportNumber = userAccountDto.PassportNumber;
        account.Employee.Person.FirstName = userAccountDto.FirstName;
        account.Employee.Person.MiddleName = userAccountDto.MiddleName;
        account.Employee.Person.LastName = userAccountDto.LastName;
        account.Employee.Person.Email = userAccountDto.Email;
        account.Employee.Person.PhoneNumber = userAccountDto.PhoneNumber;
        
        await _accountRepository.UpdateUserAccount(account);
        
    }

    public async Task DeleteAccount(int id)
    {
        
        var result = _accountRepository.DeleteAccount(id);
        if (result == null) throw new KeyNotFoundException($"There is no account with id {id}");
        await result;

    }
}