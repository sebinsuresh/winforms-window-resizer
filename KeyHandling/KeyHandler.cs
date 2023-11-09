using System.ComponentModel;
using System.Runtime.InteropServices;
using DisplayProject.Enums;
using DisplayProject.Helpers;
using DisplayProject.WindowManagement;
using DisplayProject.WindowManagement.HelperTypes;

namespace DisplayProject.KeyHandling;

public class KeyHandler
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    // 0x0312 is Windows message processor's message number for hot key registered by the RegisterHotKey function
    // https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-hotkey
    private const int RegisteredHotKeyMessage = 0x0312;
    private const KeyModifier DefaultHotKeyModifier = KeyModifier.Control | KeyModifier.Alt | KeyModifier.Shift;
    private const int HotKeyId = 0;

    // TODO: Make it configurable
    private static readonly Dictionary<(KeyModifier modifier, Keys key), MoveAndResizeOptions> KeyMappings = new()
    {
        [new(DefaultHotKeyModifier, Keys.G)] = new(Direction.Horizontal, ResizeTo.Half, 0, 0),
        [new(DefaultHotKeyModifier, Keys.H)] = new(Direction.Horizontal, ResizeTo.Half, 1, 0),
        [new(DefaultHotKeyModifier, Keys.N)] = new(0, ResizeTo.Full, 0, 0),
    };

    private WindowManager _windowManager;
    private IUserMessageHandler _msgHandler;
    private readonly nint _formHandle;

    public KeyHandler(nint formHandle, IUserMessageHandler msgHandler)
    {
        _formHandle = formHandle;
        _msgHandler = msgHandler;
        _windowManager = new WindowManager(msgHandler);

        RegisterKeys();
    }

    private void RegisterKeys()
    {
        foreach (var modifierAndKey in KeyMappings.Keys)
        {
            var registerSuccess = RegisterHotKey(
                _formHandle, HotKeyId, (int)modifierAndKey.modifier, modifierAndKey.key.GetHashCode());

            if (!registerSuccess) throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    /// <summary>
    /// Call this from overridden <see cref="Form.WndProc(ref Message)"/> method in form
    /// </summary>
    /// <param name="message">Windows message</param>
    /// <returns>Whether handling succeeded</returns>
    public bool HandleWindowsMessage(Message message)
    {
        if (message.Msg != RegisteredHotKeyMessage) return true;

        Keys key = (Keys)(((int)message.LParam >> 16) & 0xFFFF);
        KeyModifier modifier = (KeyModifier)((int)message.LParam & 0xFFFF);
        return Handle(modifier, key);
    }

    private bool Handle(KeyModifier modifier, Keys key)
    {
        var handle = _windowManager.GetActiveWindowHandle();
        if (!KeyMappings.TryGetValue((modifier, key), out var resizeOptions))
        {
            throw new InvalidOperationException();
        }
        _msgHandler.ShowMessage(
            $"Pressed key: {key} with modifier: {modifier}\n" +
            $"Moving window: {_windowManager.GetWindowName(handle)}");
        return _windowManager.MoveAndResize(handle, resizeOptions);
    }

    /// <summary>
    /// Call this from the FormClosing handler
    /// </summary>
    public void HandleFormClosing()
    {
        UnregisterHotKey(_formHandle, HotKeyId);
    }
}
