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

    public async Task<List<Account>> GetAllAccounts()
    {

        var accountsInfo = _context.Accounts.Select(a => new Account
        {
            Id = a.Id,
            Username = a.Username,
            Password = a.Password,
        });

        return await accountsInfo.ToListAsync();
    }

    public async Task<Account> GetAccountById(int id)
    {
        
        return ( _context.Accounts
            .Include(acc => acc.Employee)
            .ThenInclude(emp => emp.Person)
            .FirstOrDefault(acc => acc.Id == id));
        
    }

    public async Task<Account> ViewAccountUser(int userId)
    {
        return await _context.Accounts
            .Include(a => a.Role)
            .Include(a => a.Employee)
            .ThenInclude(e => e.Person)
            .Where(a => a.Id == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<Account> CreateAccount(Account account)
    {
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task UpdateUserAccount(Account account)
    {
        _context.Entry(account).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAccount(Account account)
    {
        
        _context.Entry(account).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
    }

    public async Task DeleteAccount(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account != null)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Role?> GetRoleByName(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<Employee?> GetEmployeeByPassportNumber(string number)
    {
        return await _context.Employees.Include(emp => emp.Person)
            .FirstOrDefaultAsync(emp => emp.Person.PassportNumber == number);
    }
}