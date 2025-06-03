using Models;

namespace Repository.Interfaces;

public interface IAccountRepository
{
    Task<Account> CreateAccount(Account account);
    
    Task<Role?> GetRoleByName(string name);
    
    Task<Employee?> GetEmployeeByName(string name);
    
}