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
using System.Threading;
using Backend;

// Por Patricio López J. (pelopez2@uc.cl)

namespace Chat2___Server
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Server Server { get; set; }
        public List<string> comandos = new List<string>(new string[] { "Right", "Left", "Up", "Down", "A", "Democracy", "Anarchy", "Boom", "Antiboom" });


        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;

            

            Server = new Server();

            Server.MensajeRecibido += (texto) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    char[] splitchar = { '|' };
                    var mensaje = texto.Split(splitchar);
                    StackPanelLog.Children.Add(new TextBlock
                    {
                        Text = mensaje[1] + " -> " + mensaje[2],
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(mensaje[0])),
                        FontWeight = (comandos.Contains(mensaje[2]) ? FontWeights.Bold : FontWeights.Normal)
                    });
                }));
            };

            Server.UsuarioConectado += () =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    StackPanelLog.Children.Add(new TextBlock { Text = "Usuario conectado", Foreground = Brushes.Blue });
                }));
            };
            IPTextBox.Text = Server.GetLocalIP();
            PortTextBox.Text = Server.PUERTO.ToString();
            // #YoPrometíNuncaBloquearElThreadPrincipal
            ConectarButton.Click += ConectarButton_Click;
            
        }

        void ConectarButton_Click(object sender, RoutedEventArgs e)
        {
            String ip = IPTextBox.Text;
            int port = Int32.Parse(PortTextBox.Text);
            IniciarServidor(ip, port);
            IPTextBox.IsReadOnly = true;
            PortTextBox.IsReadOnly = true;
        }

        private void IniciarServidor(String ip, int port)
        {
            

            new Thread(() =>
            {
                bool exito = Server.StartServer();
                if (exito)
                {
                    if (ip != null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            IPTextBox.Text = ip;
                            PortTextBox.Text = "" + port;
                            Clipboard.SetText(ip);
                            StackPanelLog.Children.Add(new TextBlock { Text = "Se ha copiado la dirección IP al portapapeles." });
                        }));
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Aquí ocurre un error cuando no se puede volver a utilizar la IPEndPoint del server.
                        // Bastante extraño.
                        MessageBox.Show("No se pudo crear un servidor.");
                    }));
                }
            }).Start();
        }
    }
}
