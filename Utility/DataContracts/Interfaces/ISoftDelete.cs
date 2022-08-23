namespace Utility.DataContracts.Interfaces;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }

    public DateTime? Deleted { get; set; }
}