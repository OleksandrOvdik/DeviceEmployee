using DTO;
using Models;

namespace Services.Interfaces;

public interface IEmployeeService
{

    public Task<List<EmployeeDto>> GetAllEmployees();
    
    public Task<EmployeeByIdDto> GetEmployeeById(int id);
    
    Task<List<Position>> GetAllPositions();
    Task<List<Role>> GetAllRoles();


}