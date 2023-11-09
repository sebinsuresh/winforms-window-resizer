using DisplayProject.Enums;

namespace DisplayProject.WindowManagement.HelperTypes;

public struct MoveAndResizeOptions
{
    /// <summary>
    /// Direction to align to when moving and resizing
    /// </summary>
    public Direction AlignDirection { get; set; }
    /// <summary>
    /// What horizontal/vertical/both slice of the monitor to resize the window to
    /// </summary> // TODO: Separate for h and v
    public ResizeTo ResizeTo { get; set; }
    /// <summary>
    /// Which slice of the horizontal monitor divisions (form the left) to place window in. Starts at 0.
    /// e.g. To place the window in the "third horizontal third-th" of the monitor, pass in 2.
    /// </summary>
    public int HorizontalNth { get; set; }
    /// <summary>
    /// Which slice of the vertical monitor divisions (from the top) to place window in. Starts at 0.
    /// e.g. To place the window in the "second vertical half" aka "bottom half" of the monitor, pass in 1.
    /// </summary>
    public int VerticalNth { get; set; }
    /// <summary>
    /// Custom percentage of the monitor width to resize to. Use when resizeTo=<see cref="ResizeTo.Custom"/>
    /// </summary>
    public float CustomPercentage { get; set; } = 100;
    /// <summary>
    /// Which monitor to move the window to. Defaults to same monitor
    /// </summary>
    public MoveType MoveType { get; set; } = MoveType.SameMonitor;

    public MoveAndResizeOptions() { }

    public MoveAndResizeOptions(
        Direction alignDirection, ResizeTo resizeTo, int horizontalNth, int verticalNth,
        float customPercentage = 100, MoveType moveType = MoveType.SameMonitor)
    {
        this.AlignDirection = alignDirection;
        this.ResizeTo = resizeTo;
        this.HorizontalNth = horizontalNth;
        this.VerticalNth = verticalNth;
        this.CustomPercentage = customPercentage;
        this.MoveType = moveType;
    }
}
