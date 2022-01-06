﻿// Interface for extending the input detection system to detect start of an object being dragged and clicks that are
// not drags. Requries the ObjDrag monobehavior to properly use.
public interface IUsesDragEvents
{
    public void OnDragStart();
    public void OnMouseDownNonDrag();
}
