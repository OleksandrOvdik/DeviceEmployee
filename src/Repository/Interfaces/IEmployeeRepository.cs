using Models;

namespace Repository.Interfaces;

public interface IEmployeeRepository
{
    public Task<IEnumerable<Employee>> GetAllEmployees();
    
    public Task<Employee> GetEmployeeById(int id);
}