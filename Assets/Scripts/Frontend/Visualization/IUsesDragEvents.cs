// Interface for extending the input detection system to detect start of an object being dragged and clicks that are
// not drags. Requries the ObjDrag monobehavior to properly use.
public interface IUsesDragEvents
{
    // Method called at the start of a proper dragging operation
    public void OnDragStart();
    // Method called at the end of a proper dragging operation
    public void OnDragFinish();
    // Method called when use clicks the object but doesn't drag
    public void OnMouseDownNonDrag();
}
