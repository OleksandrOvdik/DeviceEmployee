namespace DTO.Accounts;

public class UpdateAccountRequest
{
    public UpdateAccountAdminDto AdminPart { get; set; }

    public UpdateViewAccountUserDto UserPart { get; set; }
}