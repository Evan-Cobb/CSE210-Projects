namespace FinalProject.World;

public class VirtualFileItem
{
    public Guid Id { get; }
    public string Name { get; }
    public string Extension { get; }
    public DateTime CreatedUtc { get; }
    public DateTime ModifiedUtc { get; }

    public VirtualFileItem(string name, DateTime createdUtc, DateTime modifiedUtc)
    {
        Id = Guid.NewGuid();
        Name = name;
        Extension = Path.GetExtension(name);
        CreatedUtc = createdUtc;
        ModifiedUtc = modifiedUtc;
    }
}
