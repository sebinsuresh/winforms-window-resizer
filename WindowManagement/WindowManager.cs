using System.Runtime.InteropServices;
using System.Text;
using DisplayProject.Enums;
using DisplayProject.Helpers;
using DisplayProject.WindowManagement.HelperTypes;

namespace DisplayProject.WindowManagement;

public class WindowManager
{
    #region DllImport-ed functions

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("dwmapi.dll", SetLastError = true)]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out Rect pvAttribute, int cbAttribute);

    #endregion

    private readonly IUserMessageHandler _msgHandler;

    public WindowManager(IUserMessageHandler msgHandler)
    {
        _msgHandler = msgHandler;
    }

    public nint GetActiveWindowHandle() => GetForegroundWindow();

    /// <summary>Move and resize a window with given window handle</summary>
    /// <param name="handle">The handle for the window to be moved and resized</param>
    /// <param name="options">Options for move and resize</param>
    /// <returns><see cref="true"/> if move and resize succeeds</returns>
    public bool MoveAndResize(nint handle, MoveAndResizeOptions options)
    {
        // V2
        if (options.MoveType != MoveType.SameMonitor || options.ResizeTo == ResizeTo.Custom)
        {
            throw new NotImplementedException();
        }

        if (options.ResizeTo == ResizeTo.Full)
        {
            return ShowWindow(handle, (int)ShowWindowCommands.SW_MAXIMIZE);
        }

        var screen = Screen.FromHandle(handle);
        var finalStartX = screen.WorkingArea.Left;
        var finalStartY = screen.WorkingArea.Top;
        var finalWidth = screen.WorkingArea.Width;
        var finalHeight = screen.WorkingArea.Height;

        if (options.AlignDirection.HasFlag(Direction.Horizontal))
        {
            finalStartX += ResizeHelpers.GetStartingXOrY(finalWidth, options.ResizeTo, options.HorizontalNth);
            finalWidth = ResizeHelpers.GetResizedDimension(finalWidth, options.ResizeTo);
        }
        if (options.AlignDirection.HasFlag(Direction.Vertical))
        {
            finalStartY += ResizeHelpers.GetStartingXOrY(finalHeight, options.ResizeTo, options.VerticalNth);
            finalHeight = ResizeHelpers.GetResizedDimension(finalHeight, options.ResizeTo);
        }

        // https://stackoverflow.com/a/34143777
        // Windows 10 has thin invisible borders on left, right, and bottom, it is used to grip the mouse for resizing.
        // The borders might look like this: 7,0,7,7 (left, top, right, bottom)
        GetWindowRect(handle, out Rect rectWithoutBorders);
        Rect rectWithBorders;
        if (Environment.OSVersion.Version.Major >= 6)
        {
            int size = Marshal.SizeOf(typeof(Rect));
            _ = DwmGetWindowAttribute(
                    handle, (int)DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, out rectWithBorders, size);
        }
        else
        {
            GetWindowRect(handle, out rectWithBorders);
        }

        var leftBorder = rectWithBorders.Left - rectWithoutBorders.Left;
        var topBorder = rectWithBorders.Top - rectWithoutBorders.Top;
        var rightBorder = rectWithoutBorders.Right - rectWithBorders.Right;
        var bottomBorder = rectWithoutBorders.Bottom - rectWithBorders.Bottom;

        finalStartX -= leftBorder;
        finalStartY -= topBorder;
        finalWidth += leftBorder + rightBorder;     // Adjust for moving left
        finalHeight += topBorder + bottomBorder;    // Adjust for moving up

        _msgHandler.ShowMessage(
            $"Monitor res: {screen.WorkingArea.Width}x{screen.WorkingArea.Height}\n" +
            $"Monitor top-left: {screen.WorkingArea.Left}, {screen.WorkingArea.Top}\n" +
            $"App final dims: {finalWidth}x{finalHeight}px\n" +
            $"App moved to: {finalStartX}, {finalStartY}"
        );

        return
            // Need to restore first
            ShowWindow(handle, (int)ShowWindowCommands.SW_RESTORE) &&
            MoveWindow(handle, finalStartX, finalStartY, finalWidth, finalHeight, true);
    }

    public string GetWindowName(nint handle)
    {
        var strTitle = string.Empty;
        // Obtain the length of the text   
        var intLength = GetWindowTextLength(handle) + 1;
        var stringBuilder = new StringBuilder(intLength);
        if (GetWindowText(handle, stringBuilder, intLength) > 0)
        {
            strTitle = stringBuilder.ToString();
        }
        return strTitle;
    }
}
