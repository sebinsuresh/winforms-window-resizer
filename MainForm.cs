using System.ComponentModel;
using System.Runtime.InteropServices;
using DisplayProject.Enums;
using DisplayProject.Helpers;

namespace DisplayProject;

public partial class MainForm : Form
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

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
        msgHandler.ShowMessage($"Pressed key: {key} with modifier: {modifier}");
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
