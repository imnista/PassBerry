namespace PassBerry
{
    using PassBerry.Forms;
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while installize the program.\nDetails:\n{ex}");
                Application.Exit();
            }
        }
    }
}