using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using DisplayProject.Enums;
using DisplayProject.Helpers;

namespace DisplayProject;

public partial class MainForm : Form
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

    // 0x0312 Windows message processor's message number for hot key registered by the RegisterHotKey function
    // https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-hotkey
    private const int WM_HOTKEY = 0x0312;
    private const int HOT_KEY_MODIFIER = (int)(KeyModifier.Control | KeyModifier.Alt | KeyModifier.Shift);
    private const int HOT_KEY_ID = 0;

    private readonly IUserMessageHandler msgHandler;

    public MainForm()
    {
        InitializeComponent();
        msgHandler = new LabelUserMessageHandler(Controls);
        RegisterKeys();
        FormClosing += MainForm_FormClosing;
    }

    private void RegisterKeys()
    {
        var keys = new[] { Keys.G, Keys.H };
        foreach (var key in keys)
        {
            if (!RegisterHotKey(this.Handle, HOT_KEY_ID, HOT_KEY_MODIFIER, key.GetHashCode()))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (m.Msg == WM_HOTKEY)
        {
            m = HandleRegisteredKeyMessage(m);
        }
    }

    private Message HandleRegisteredKeyMessage(Message m)
    {
        /* 
         * Note that the three lines below are not needed if you only want to register one hotkey.
         * The below lines are useful in case you want to register multiple keys, which you can use a switch with
         * the id as argument, or if you want to know which key/modifier was pressed for some particular reason.
        */

        Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);              // The key of the hotkey that was pressed.
        KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);   // The modifier of the hotkey that was pressed.
        int id = m.WParam.ToInt32();                                    // The id of the hotkey that was pressed.

        HandleRegisteredKeyPress(key, modifier, id);
        return m;
    }

    private void HandleRegisteredKeyPress(Keys key, KeyModifier modifier, int hotKeyId)
    {
        var handle = GetForegroundWindow();
        // TODO: Replace with the new MoveWindow overload
        var (x, y, width, height) = key switch
        {
            Keys.G => (100, 100, 800, 600),
            Keys.H => (400, 400, 600, 600),
            _ => (0, 0, 800, 600),
        };
        MoveWindow(handle, x, y, width, height, true);
        msgHandler.ShowMessage(
            $"Pressed key: {key} with modifier: {modifier}\n" +
            $"Moving window: {GetCaptionOfActiveWindow(handle)}");
    }

    private static string GetCaptionOfActiveWindow(nint handle)
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

    /// <summary>
    /// Move and resize a window with given window handle
    /// </summary>
    /// <param name="handle">The handle for the window to be moved and resized</param>
    /// <param name="alignDirection">Direction to align to when moving & resizing</param>
    /// <param name="resizeTo">What slice of the monitor to resize the window to</param>
    /// <param name="horizontalNth">
    ///     Which slice of the horizontal monitor divisions (form the left) to place window in. Starts at 0.
    ///     e.g. To place the window in the "third horizontal third-th" of the monitor, pass in 2.
    /// </param>
    /// <param name="verticalNth">
    ///     Which slice of the vertical monitor divisions (from the top) to place window in. Starts at 0.
    ///     e.g. To place the window in the "second vertical half" aka "bottom half" of the monitor, pass in 1.
    /// </param>
    /// <param name="customPercentage">
    ///     Custom percentage of the monitor width to resize to. Use when resizeTo=<see cref="ResizeTo.Custom"/>
    /// </param>
    /// <param name="moveType">Which monitor to move the window to. Defaults to same monitor</param>
    /// <returns><see cref="true"/> if move and resize succeeds</returns>
    private static bool MoveAndResizeWindow(
        nint handle, Direction alignDirection, ResizeTo resizeTo, int horizontalNth, int verticalNth,
        float customPercentage = 100, MoveType moveType = MoveType.SameMonitor)
    {
        // V2
        if (moveType != MoveType.SameMonitor) throw new NotImplementedException("Not supported yet");

        // TODO
        throw new NotImplementedException();
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // TODO: Remove
        // MessageBox.Show($"Reason = {e.CloseReason}\nCancel = {e.Cancel}", "FormClosing Event");

        // Unregister hotkey with id `hotKeyId` before closing the form.
        // You might want to call this more than once with different id values if you are planning
        // to register more than one hotkey.
        UnregisterHotKey(this.Handle, HOT_KEY_ID);
    }
}
