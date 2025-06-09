namespace DTO.Accounts;

public class UpdateAccountAdminDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string EmployeePassportNumber {get; set;} = null!;
    public string RoleName { get; set; } = null!;
    
}