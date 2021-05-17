namespace Jam3
{
    public enum InteractiveType
    {
        Interactive = 0,
        Action = 1
    }

    public enum InteractiveStatus
    {
        None,
        Placement,
        Moving,
        Transform
    }

    public enum Layers
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        Draggable = 3,
        Water = 4,
        UI = 5,
        ARSpace = 6,
        PlayerSpace = 7,
        Effects = 8,
        Surface = 9,
        DraggableAction = 10,
        HandlerX = 11,
        HandlerY = 12,
        HandlerZ = 13,
        BackgroundMesh = 14
    }

    public enum MouseButtons
    {
        Left,
        Right,
        Middle
    }

    public enum Direction
    {
        Left,
        Right,
        Forward,
        Backward,
    }
}
