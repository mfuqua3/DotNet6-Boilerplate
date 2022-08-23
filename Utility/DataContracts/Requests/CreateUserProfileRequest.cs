namespace Utility.DataContracts.Requests;

public class CreateUserProfileRequest
{
    public string ActiveDirectoryId { get; set; }
    public bool Onboarded { get; set; }
    
}