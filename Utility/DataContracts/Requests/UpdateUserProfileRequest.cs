using System.ComponentModel.DataAnnotations;
using Utility.DataContracts.Interfaces;

namespace Utility.DataContracts.Requests;

public class UpdateUserProfileRequest : CreateUserProfileRequest, IUnique<int>
{
    [Range(1, int.MaxValue, ErrorMessage = "Must provide a valid ID.")]
    public int Id { get; set; }
}