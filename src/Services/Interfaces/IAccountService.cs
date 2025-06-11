using DTO;
using DTO.Accounts;
using Models;

namespace Services.Interfaces;

public interface IAccountService
{
    
    Task<List<AllAcountsDto>> GetAllAcounts();
    
    Task<GetSpecificAccountDto> GetAccountById(int id);
    
    
    Task<CreateAccountDto> CreateAccount(CreateAccountDto accountDto);
    
    Task UpdateAccount(int id, UpdateAccountDto accountDto);
    
    Task UpdateUserAccount(int id, UpdateAccountDto accountDto);
    
    Task DeleteAccount(int id);
    
}