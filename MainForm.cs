using DisplayProject.Helpers;
using DisplayProject.KeyHandling;

namespace DisplayProject;

public partial class MainForm : Form
{
    private readonly IUserMessageHandler msgHandler;
    private readonly KeyHandler keyHandler;

    public MainForm()
    {
        InitializeComponent();
        msgHandler = new LabelUserMessageHandler(Controls);
        keyHandler = new KeyHandler(this.Handle, msgHandler);
        FormClosing += MainForm_FormClosing;
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        // WndProc may be called before form constructor and
        // keyHandler may not be initialized yet
        keyHandler?.HandleWindowsMessage(m);
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        keyHandler.HandleFormClosing();
    }
}
