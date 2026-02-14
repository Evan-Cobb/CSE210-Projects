namespace FinalProject.Undo;

public class UndoStack
{
    private readonly Stack<MoveRecord> _stack;

    public UndoStack()
    {
        _stack = new Stack<MoveRecord>();
    }

    public int Count => _stack.Count;

    public void Push(MoveRecord record)
    {
        _stack.Push(record);
    }

    public bool TryPop(out MoveRecord record)
    {
        if (_stack.Count == 0)
        {
            record = null;
            return false;
        }
        record = _stack.Pop();
        return true;
    }
}
