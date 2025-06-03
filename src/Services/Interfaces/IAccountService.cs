using DTO.Accounts;

namespace Services.Interfaces;

public interface IAccountService
{
    
    Task<CreateAccountDto> CreateAccount(CreateAccountDto accountDto);
    
}