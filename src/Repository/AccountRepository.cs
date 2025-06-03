using Microsoft.EntityFrameworkCore;
using Models;
using Repository.Interfaces;

namespace Repository;

public class AccountRepository : IAccountRepository
{
    
    private readonly MasterContext _context;

    public AccountRepository(MasterContext context)
    {
        _context = context;
    }

    public async Task<Account> CreateAccount(Account account)
    {
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<Role?> GetRoleByName(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<Employee?> GetEmployeeByName(string name)
    {
        return await _context.Employees.Include(emp => emp.Person)
            .FirstOrDefaultAsync(emp => emp.Person.FirstName == name);
    }
}