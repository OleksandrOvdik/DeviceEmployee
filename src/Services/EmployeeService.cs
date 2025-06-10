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
        var employee = await _employeeRepository.GetEmployeeById(id);
        if (employee == null) throw new KeyNotFoundException($"Employee with id {id} not found");
        return new EmployeeByIdDto()
        {
            Person = new PersonEmployeeDto()
            {
                Id = employee.Person.Id,
                FirstName = employee.Person.FirstName,
                MiddleName = employee.Person.MiddleName,
                LastName = employee.Person.LastName,
                PhoneNumber = employee.Person.PhoneNumber,
                Email = employee.Person.Email,
            },
            Salary = employee.Salary,
            Position = new Position()
            {
                Id = employee.Position.Id,
                Name = employee.Position.Name
            },
            HireDate = employee.HireDate,
        };
    }

    public async Task<List<Position>> GetAllPositions()
    {
        var result = await _employeeRepository.GetAllPositions();
        if (result == null) throw new KeyNotFoundException("No positions found");

        return result.Select(r => new Position()
        {
            Id = r.Id,
            Name = r.Name,
        }).ToList();

    }

    public async Task<List<Role>> GetAllRoles()
    {
        var result = await _employeeRepository.GetAllRoles();
        if (result == null) throw new KeyNotFoundException("No roles found");
        return result.Select(r => new Role()
        {
            Id = r.Id,
            Name = r.Name,
        }).ToList();
    }
}