namespace Student_Performance
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
        public static class AppInfo
        {
            // Фиксируем время старта приложения
            public static readonly DateTime StartTime = DateTime.Now;

            // Переменная для хранения даты последнего успешного бэкапа
            public static DateTime? LastBackupDate { get; set; }
        }
    }
}