
using System.ComponentModel.DataAnnotations;

namespace Models;

public partial class Role
{
    public int Id { get; set; }

    [Length(1,25)]
    public string Name { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
