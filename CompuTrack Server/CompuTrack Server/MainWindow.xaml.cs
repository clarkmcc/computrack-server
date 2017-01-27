using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CompuTrack_Server.consoleWriter;
using static CompuTrack_Server.engine;

namespace CompuTrack_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextWriter writer = new TextBoxConsole(tbConsole);
            Console.SetOut(writer);
            engine.alertEngine.engine(323000);
            engine.startTimer(323000);
        }

        private void tbConsole_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbConsole.ScrollToEnd();
        }

        private void tbConsole_MouseMove(object sender, MouseEventArgs e)
        {
            tbConsole.ScrollToEnd();
        }

        private void tbConsole_Initialized(object sender, EventArgs e)
        {
            tbConsole.ScrollToEnd();
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            tbConsole.ScrollToEnd();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            tbConsole.ScrollToEnd();
        }
    }
}
