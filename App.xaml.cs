using DQB2IslandEditor.DataPK;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DQB2IslandEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Error" + Environment.NewLine + e.Exception.Message, "Error");
            if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "ErrorLog"))) Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "ErrorLog"));
            string path = Path.Combine(Directory.GetCurrentDirectory(), "ErrorLog\\" + DateTime.Now.ToString("yyyy-MM-dd_HH;mm") + "-CRASH.txt");
            System.IO.File.WriteAllText(path, 
                "TIME:" + DateTime.Now.ToString("yyyy-MM-dd_HH;mm") + "\nError" + Environment.NewLine + e.Exception.Message);
            //e.Handled = true;
        }
    }

}
