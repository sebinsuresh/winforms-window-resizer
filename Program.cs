namespace DisplayProject;

/*
* Sources referenced for things:
* - https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/overview-of-using-controls-in-windows-forms?view=netframeworkdesktop-4.8
* - https://www.fluxbytes.com/csharp/how-to-register-a-global-hotkey-for-your-application-in-c/
* - https://stackoverflow.com/questions/24371440/how-to-run-a-method-after-a-specific-time-interval
* - https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-hotkey
*/

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
