
using System.ComponentModel.DataAnnotations;

namespace Models;

public partial class Account
{
    public int Id { get; set; }

    [Length(3,60)]
    public string Username { get; set; } = null!;

    [Length(12,25)]
    public string Password { get; set; } = null!;

    public int EmployeeId { get; set; }

    public int RoleId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    
    public virtual Role Role { get; set; } = null!;
}
