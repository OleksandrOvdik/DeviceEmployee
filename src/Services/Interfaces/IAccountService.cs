using DTO;
using DTO.Accounts;
using Models;

namespace Services.Interfaces;

public interface IAccountService
{
    
    Task<List<AllAcountsDto>> GetAllAcounts();
    
    Task<AccountByIdDto> GetAccountById(int id);
    
    Task<ViewAccountUserDto> ViewAccountUser(int userId);
    
    
    Task<CreateAccountDto> CreateAccount(CreateAccountDto accountDto);
    
    Task UpdateAccount(int id, UpdateAccountAdminDto accountAdminDto);
    
    Task DeleteAccount(int id);
    
}