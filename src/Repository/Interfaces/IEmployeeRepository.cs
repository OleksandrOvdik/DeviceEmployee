using Models;

namespace Repository;

public interface IEmployeeRepository
{
    public Task<IEnumerable<Employee>> GetAllEmployees();
    
    public Task<Employee> GetEmployeeById(int id);
}