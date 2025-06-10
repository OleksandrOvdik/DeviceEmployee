using Models;

namespace DTO.Accounts;

public class UpdateViewAccountUserDto
{
    //  Account:
    public int    AccountId   { get; set; }
    public string Username    { get; set; } = null!;
    public string Password    { get; set; } = null!;

    //  Role:
    public string RoleName    { get; set; } = null!;

    //  Employee:
    public int    EmployeeId  { get; set; }
    public decimal Salary     { get; set; }
    public int    PositionId  { get; set; }
    public DateTime HireDate  { get; set; }

    //  Person:
    public int    PersonId        { get; set; }
    public string PassportNumber  { get; set; } = null!;
    public string FirstName       { get; set; } = null!;
    public string MiddleName     { get; set; } = null!;
    public string LastName        { get; set; } = null!;
    public string PhoneNumber     { get; set; } = null!;
    public string Email           { get; set; } = null!;
    
    
}