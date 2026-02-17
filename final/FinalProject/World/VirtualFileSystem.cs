namespace FinalProject.World;

public class VirtualFileSystem
{
    private readonly Dictionary<string, List<VirtualFileItem>> _folders;
    private readonly List<string> _folderOrder;

    public VirtualFileSystem(IEnumerable<string> folderNames)
    {
        _folders = new Dictionary<string, List<VirtualFileItem>>(StringComparer.OrdinalIgnoreCase);
        _folderOrder = new List<string>();
        foreach (string name in folderNames)
        {
            AddFolderInternal(name);
        }
    }

    private void AddFolderInternal(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Folder name cannot be empty.");
        }
        if (_folders.ContainsKey(name))
        {
            throw new InvalidOperationException($"Duplicate folder: {name}");
        }
        _folders[name] = new List<VirtualFileItem>();
        _folderOrder.Add(name);
    }

    public IReadOnlyList<string> FolderNames => _folderOrder.AsReadOnly();

    public IReadOnlyList<VirtualFileItem> GetFolderItems(string folderName)
    {
        if (!_folders.TryGetValue(folderName, out List<VirtualFileItem> items))
        {
            throw new InvalidOperationException($"Folder not found: {folderName}");
        }
        return items.AsReadOnly();
    }

    public void AddToFolder(string folderName, VirtualFileItem item)
    {
        if (!_folders.TryGetValue(folderName, out List<VirtualFileItem> items))
        {
            throw new InvalidOperationException($"Folder not found: {folderName}");
        }
        items.Add(item);
    }

    public void MoveItem(VirtualFileItem item, string fromFolder, string toFolder)
    {
        if (!_folders.TryGetValue(fromFolder, out List<VirtualFileItem> fromItems))
        {
            throw new InvalidOperationException($"Folder not found: {fromFolder}");
        }
        if (!_folders.TryGetValue(toFolder, out List<VirtualFileItem> toItems))
        {
            throw new InvalidOperationException($"Folder not found: {toFolder}");
        }
        if (!fromItems.Remove(item))
        {
            throw new InvalidOperationException("Item not found in source folder.");
        }
        toItems.Add(item);
    }
}
