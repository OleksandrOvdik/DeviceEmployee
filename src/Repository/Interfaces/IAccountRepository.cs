using DTO.Accounts;
using Models;

namespace Repository.Interfaces;

public interface IAccountRepository
{
    
    Task<List<Account>> GetAllAccounts();
    
    Task<Account> GetAccountById(int id);
    
    Task<Account> ViewAccountUser(int passportNumber);
    
    Task<Account> CreateAccount(Account account);
    
    Task UpdateUserAccount(Account account);
    
    Task UpdateAccount(Account account);
    
    Task DeleteAccount(int id);
    
    Task<Role?> GetRoleByName(string name);
    
    Task<Employee?> GetEmployeeByPassportNumber(string name);
    
}