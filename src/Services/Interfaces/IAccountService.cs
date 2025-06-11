using DTO;
using DTO.Accounts;
using Models;

namespace Services.Interfaces;

public interface IAccountService
{
    
    Task<List<AllAcountsDto>> GetAllAcounts();
    
    Task<GetSpecificAccountDto> GetAccountById(int id);
    
    Task<UpdateViewAccountUserDto> ViewAccountUser(int userId);
    
    
    Task<CreateAccountDto> CreateAccount(CreateAccountDto accountDto);
    
    Task UpdateAccount(int id, UpdateAccountAdminDto accountAdminDto);
    
    Task UpdateUserAccount(int id, UpdateViewAccountUserDto userAccountDto);
    
    Task DeleteAccount(int id);
    
}