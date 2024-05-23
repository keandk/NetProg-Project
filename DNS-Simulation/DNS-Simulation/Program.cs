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

            Client client1 = new Client();
            client1.Show();
            //Client client2 = new Client();
            //client2.Show();
            Server server = new();
            server.Show();
            Application.Run();
        }
    }
}