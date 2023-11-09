namespace DisplayProject.Helpers;

public class LabelUserMessageHandler : IUserMessageHandler
{
    private const double ClearLabelTimeInSeconds = 0.1;
    private readonly Label _messageLabel;
    private DateTime _lastMessageReceived = DateTime.Now;

    public LabelUserMessageHandler(Control.ControlCollection Parent)
    {
        _messageLabel = new Label
        {
            Text = "No presses detected",
            AutoSize = true,
        };
        Parent.Add(_messageLabel);
    }

    public void ShowMessage(string message)
    {
        if ((DateTime.Now - _lastMessageReceived).Seconds > ClearLabelTimeInSeconds)
        {
            _messageLabel.Text = message;
        }
        else
        {
            _messageLabel.Text += "\n" + message;
        }
        _lastMessageReceived = DateTime.Now;
    }
}
