namespace DisplayProject.Enums;

[Flags]
public enum Direction
{
    Horizontal = 1,
    Vertical = 2,
    Both = Horizontal | Vertical,
}
