namespace DNS_Simulation
{
    internal static class Program
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

            Client client = new Client();
            Server server = new Server();
            client.Show();
            server.Show();
            Application.Run();
        }
    }
}