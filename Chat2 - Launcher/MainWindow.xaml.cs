using System;
using System.Collections.Generic;
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

using Chat2___Client;   // Importamos los otros proyectos tal y como se suele hacer con el backend.
using Chat2___Server;   // Revisar las referencias de Chat2 - Launcher para verlo. 

// Por Patricio López J. (pelopez2@uc.cl)

namespace Chat2___Launcher
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ButtonClient.Click += (s, e) =>
            {
                Window clientWindow = new Chat2___Client.MainWindow();
                clientWindow.Show();
            };

            ButtonServer.Click += (s, e) =>
            {
                Window serverWindow = new Chat2___Server.MainWindow();
                serverWindow.Show();

                ButtonServer.IsEnabled = false;

                serverWindow.Closed += OnWindowClose;
            };
        }

        private void OnWindowClose(object sender, EventArgs e)
        {
            // Cerramos todas las ventanas.
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                App.Current.Windows[intCounter].Close();

            // Fuente: 
            // http://social.msdn.microsoft.com/Forums/vstudio/en-US/86e4806f-8f95-49a8-811e-e01514518e16/close-all-open-windows-in-wpf?forum=wpf
        }

        // Bastante simple.
    }
}
