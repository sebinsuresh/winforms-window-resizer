namespace DisplayProject.Helpers;

public class LabelUserMessageHandler : IUserMessageHandler
{
    private readonly Label messageLabel;

    public LabelUserMessageHandler(Control.ControlCollection Parent)
    {
        messageLabel = new Label
        {
            Text = "No presses detected",
            AutoSize = true,
        };
        Parent.Add(messageLabel);
    }

    public void ShowMessage(string message)
    {
        messageLabel.Text = message;
    }
}
