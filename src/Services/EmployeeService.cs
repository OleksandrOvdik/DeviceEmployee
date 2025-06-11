using DTO;
using Models;
using Repository;
using Repository.Interfaces;
using Services.Interfaces;

namespace Services;

public class EmployeeService : IEmployeeService
{
    
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }
    

    public async Task<List<EmployeeDto>> GetAllEmployees()
    {
        
        var employees = await _employeeRepository.GetAllEmployees();
        if (employees == null) throw new KeyNotFoundException("No employees found");
        var obj = new List<EmployeeDto>();

        foreach (var employee in employees)
        {
            obj.Add(new EmployeeDto()
            {
                Id = employee.Id,
                Fullname = $"{employee.Person.FirstName} {employee.Person.MiddleName} {employee.Person.LastName}"
            });
        }
        
        return obj;
        
    }

    public async Task<EmployeeByIdDto> GetEmployeeById(int id)
    {
        // var PositionName = await _employeeRepository.GetPositionById(id);
        // if (PositionName == null) throw new KeyNotFoundException("No position found");
        
        var employee = await _employeeRepository.GetEmployeeById(id);
        if (employee == null) throw new KeyNotFoundException($"Employee with id {id} not found");
        return new EmployeeByIdDto()
        {
            Person = new PersonEmployeeDto()
            {
                PassportNumber = employee.Person.PassportNumber,
                FirstName = employee.Person.FirstName,
                MiddleName = employee.Person.MiddleName,
                LastName = employee.Person.LastName,
                PhoneNumber = employee.Person.PhoneNumber,
                Email = employee.Person.Email,
            },
            Salary = employee.Salary,
            Position = employee.Position.Name,
            HireDate = employee.HireDate,
        };
    }

    public async Task<PostPutSpecificEmployee> CreateEmployee(PostPutSpecificEmployee employeeDto)
    {

        var employee = new Employee()
        {
            Salary = employeeDto.Salary,
            PositionId = employeeDto.PositionId,
            Person = new Person()
            {
                PassportNumber = employeeDto.Person.PassportNumber,
                FirstName = employeeDto.Person.FirstName,
                MiddleName = employeeDto.Person.MiddleName!,
                LastName = employeeDto.Person.LastName,
                PhoneNumber = employeeDto.Person.PhoneNumber,
                Email = employeeDto.Person.Email,
            }
        };
        
        var newEmployee = await _employeeRepository.CreateEmployee(employee);
        if (newEmployee == null) throw new NullReferenceException("Null in newEmployee");

        return new PostPutSpecificEmployee()
        {
            PositionId = newEmployee.PositionId,
        };

    }

    public async Task<List<GetAllPositionsDto>> GetAllPositions()
    {
        var result = await _employeeRepository.GetAllPositions();
        if (result == null) throw new KeyNotFoundException("No positions found");

        return result.Select(r => new GetAllPositionsDto()
        {
            Id = r.Id,
            Name = r.Name,
        }).ToList();

    }

    public async Task<List<GetAllRolesDto>> GetAllRoles()
    {
        var result = await _employeeRepository.GetAllRoles();
        if (result == null) throw new KeyNotFoundException("No roles found");
        return result.Select(r => new GetAllRolesDto()
        {
            Id = r.Id,
            Name = r.Name,
        }).ToList();
    }
}