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

    public async Task UpdateAccount(int id, UpdateAccountDto accountDto)
    {
        
        var account = await _accountRepository.GetAccountById(id);
        if (account == null) throw new KeyNotFoundException("There is no account");
        
        
        account.Username = accountDto.Username;
        account.Password = _passwordHasher.HashPassword(null, accountDto.Password);
        account.EmployeeId = accountDto.EmployeeId;
        account.RoleId = accountDto.RoleId;
        
        await _accountRepository.UpdateAccount(account);
        
    }

    public async Task UpdateUserAccount(int id, UpdateAccountDto accountDto)
    {
        
        var account = await _accountRepository.GetAccountById(id);
        if (account == null) throw new KeyNotFoundException("There is no account");
        
        account.Username = accountDto.Username;
        account.Password = _passwordHasher.HashPassword(null, accountDto.Password);
        
        await _accountRepository.UpdateUserAccount(account);
        
    }

    public async Task DeleteAccount(int id)
    {
        
        var result = _accountRepository.DeleteAccount(id);
        if (result == null) throw new KeyNotFoundException($"There is no account with id {id}");
        await result;

    }
}