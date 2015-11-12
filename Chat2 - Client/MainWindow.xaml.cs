using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Backend;

// Por Patricio López J. (pelopez2@uc.cl)

namespace Chat2___Client
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ButtonConnect.Click += ButtonConnect_Click;

            // Con esto nos aseguramos que el TextBox esté listo para escribir.
            TextBoxUserName.Focus();
            ButtonConnect.IsEnabled = false;
            TextBoxIP.Text = System.Windows.Forms.Clipboard.GetText();

            TextBoxUserName.KeyDown += (s, e) =>
            {
                ButtonConnect.IsEnabled = true;
                if (e.Key == Key.Enter)
                {
                    Conectar();
                }
            };
        }

        void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            Conectar();
        }

        private void Conectar()
        {
            Client cliente = null;
            Button button = ButtonConnect;
            String IP = TextBoxIP.Text;
            String UserName = TextBoxUserName.Text;
            String Port = puerto.Text;

            TextBoxIP.IsEnabled = button.IsEnabled = false;

            // Hacemos que la tarea de intentar conectarse se haga en un proceso por separado.
            // Esto para no bloquear el thread principal.
            // Una manera más correcta de hacer esto es utilizando Task:
            // http://msdn.microsoft.com/es-es/library/system.threading.tasks.task(v=vs.110).aspx
            //
            // Pero no nos compliquemos más, usemos solo un Thread:
            new Thread(() =>
            {
                // Tareas en backbround.
                cliente = new Client(UserName);
                bool exito = cliente.Conectar(IP, Port);

                // Una vez terminada las tareas volvemos al thread principal. 
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (exito)
                    {
                        Chat chat = new Chat(cliente, new Mapa("mapa"));
                        chat.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo conectar");
                        TextBoxIP.IsEnabled = button.IsEnabled = true;
                    }
                }));
            }).Start();
        }
    }
}
