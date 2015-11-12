using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// // Por Patricio López J. (pelopez2@uc.cl)

namespace Backend
{
    public class Client
    {
        // Campos importantes.
        private Socket SocketCliente { get; set; }
        private Thread EscucharServidorThread { get; set; }
        public string UserName;
        public string color;
        public string lastMessage = "";

        // Eventos.
        public event Action<String> MensajeRecibido;
        public event Action<string> MoverGrilla;
        public event Action Accion;
        public event Action<bool> Boom; //true si hay que aumentar el nivel de peligro

        // Constructor
        public Client(string username)
        {
            UserName = username;
            color = getRandColor();
        }

        // Nos intentamos conectar a una IP
        public bool Conectar(String IP, String Port)
        {
            IPEndPoint Ep = null;
            int Puerto = Int32.Parse(Port);
            try
            {
                Ep = new IPEndPoint(IPAddress.Parse(IP), Puerto);
                SocketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SocketCliente.Connect(Ep);
            }
            catch (ArgumentOutOfRangeException)
            {
                // El número del puerto está fuera de los límites.
                return false;
            }
            catch (FormatException)
            {
                // El formato de la IP no es válido.
                return false;
            }
            catch (SocketException)
            {
                // Error de conexión.
                return false;
            }

            // Conexión satisfactoria.

            // Creamos un thread para escuchar al servidor.
            EscucharServidorThread = new Thread(EscucharServidor);
            EscucharServidorThread.IsBackground = true;
            EscucharServidorThread.Start();

            return true;
        }

        private void EscucharServidor()
        {
            while (SocketCliente != null)
            {
                string texto;
                byte[] dataBuffer;
                int largo;

                try
                {
                    dataBuffer = new byte[256];
                    // No queremos bloquear el thread principal, por eso esta linea se ejecuta en un thread separado.
                    largo = SocketCliente.Receive(dataBuffer);
                    texto = Encoding.UTF8.GetString(dataBuffer, 0, largo);
                    char[] splitchar = { '|' };
                    var mensaje = texto.Split(splitchar);
                    // El servidor nos mandó un mensaje.

                    if (MensajeRecibido != null && mensaje[2].Length != 0)
                        MensajeRecibido(texto);
                    EsComando(texto);

                }
                catch (SocketException)
                {
                    
                }
            }
        }

        public void EsComando(string lista)
        {
            char[] splitchar = { '|' };
            var mensaje = lista.Split(splitchar)[2];
            if (mensaje == "Right")
            {
                if (MoverGrilla != null)
                {
                    MoverGrilla("Right");
                }
            }
            else if (mensaje == "Left")
            {
                if (MoverGrilla != null)
                {
                    MoverGrilla("Left");
                }
            }
            else if (mensaje == "Up")
            {
                if (MoverGrilla != null)
                {
                    MoverGrilla("Up");
                }
            }
            else if (mensaje == "Down")
            {
                if (MoverGrilla != null)
                {
                    MoverGrilla("Down");
                }
            }
            else if (mensaje == "A")
            {
                if (Accion != null)
                {
                    Accion();
                }
            }
            else if (mensaje == "Boom")
            {
                if (Boom != null)
                {
                    Boom(true);
                }
            }
            else if (mensaje == "Antiboom")
            {
                if (Boom != null)
                {
                    Boom(false);
                }
            }




        }

        private string getRandColor()
        {
            Random rnd = new Random();
            string hexOutput = String.Format("{0:X}", rnd.Next(0, 0xFFFFFF));
            while (hexOutput.Length < 6)
                hexOutput = "0" + hexOutput;
            return "#" + hexOutput;
        }

        public void EnviarMensaje(String s)
        {
            if (s == "")
                s = lastMessage;
            lastMessage = s;
            if (s != "")
            {
                string mensaje = color + "|" + UserName + "|" + s;

                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(mensaje);
                    SocketCliente.Send(data);
                }
                catch (SocketException)
                {
                    // No se pudo enviar el mensaje.
                }
            }
        }
    }
}
