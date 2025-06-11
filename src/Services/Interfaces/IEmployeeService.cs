using DTO;
using Models;

namespace Services.Interfaces;

public interface IEmployeeService
{

    public Task<List<EmployeeDto>> GetAllEmployees();
    
    public Task<EmployeeByIdDto> GetEmployeeById(int id);
    
    Task<PostPutSpecificEmployee> CreateEmployee(PostPutSpecificEmployee employeeDto);
    
    Task<List<GetAllPositionsDto>> GetAllPositions();
    Task<List<GetAllRolesDto>> GetAllRoles();


}