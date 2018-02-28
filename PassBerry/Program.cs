namespace PassBerry
{
    using PassBerry.Forms;
    using System;
    using System.Drawing;
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

        private static void Debug()
        {
            var p = Handler.ResourceProcessor.GetInstance();
            var id = Guid.NewGuid();
            p.Save(new Model.RecordForDisplay
            {
                Id = id,
                Name = "Baidu",
                PictureForLogo = Image.FromFile(@"C:\Users\Hendry\Desktop\1.jpg"),
                Keywords = "1;2;3;4",
                Password = Library.SecureStringHelper.GetSecureStringFromString("1qaz2wsxE"),
                Remarks = "test123"
            });

            var all = p.GetAll();
        }
    }
}