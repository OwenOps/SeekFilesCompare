
namespace SeekFilesCompare.environment
{
    internal class SettingsExample
    {
        public static string PathResult { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results.xlsx");
        public static string Server = "smtp.server"; // Example
        public static int Port { get; } = 1234; // Example
        public static string UserName { get; } = "your_mail";
        public static string AppPassword = "your_app_password_mail";
        public static string Name { get; } = "your_name";
    }
}
