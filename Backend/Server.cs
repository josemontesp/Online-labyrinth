using System;
using System.Collections.Generic;
using System.Linq;
using System.Net; // Importado
using System.Net.Sockets; // Importado
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Por Patricio López J. (pelopez2@uc.cl)
// Forked by Jose Montes

namespace Backend
{
    public class Server
    {
        // Constantes
        private const int CONEXIONESMAXIMAS = Int32.MaxValue;   // El servidor soportará 2.147.483.647 conexiones.
        private const int SLEEPTIME = 10;                       // Tiempo de sleep para el thread que envía mensajes para no saturarlo.
        public const int PUERTO = 8000;                         // Puerto que usaremos para nuestro programa. Puede ser un número hasta el 65.535

        
        // Campos principales.
        private Socket SocketServer { get; set; }               // El socket principal del servidor.
        private List<Socket> Clientes { get; set; }             // Lista de los Sockets de los clientes.
        private Queue<String> ColaMensajes { get; set; }        // Cola de mensajes pendientes para ser enviados. 


        // Eventos
        public event Action UsuarioConectado;                   // Se llama cuando se conecta un cliente y es añadido a la lista de sockets
        public event Action<String> MensajeRecibido;            // Se llama cuando se recibe un mensaje.


        // Constructor
        public Server()
        {
            Clientes = new List<Socket>();
            ColaMensajes = new Queue<String>();
        }

        // Intentamos iniciar el servidor. 
        public bool StartServer()
        {
            // Creamos nuestro punto de conexión. 
            IPEndPoint Ep = null;
            try
            {
                Ep = new IPEndPoint(IPAddress.Any, PUERTO);
                SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SocketServer.Bind(Ep);
                SocketServer.Listen(CONEXIONESMAXIMAS);
            }
            catch (SocketException)
            {
                return false;
            }

            IniciarRecibirConexionesThread();
            IniciarProcesadorDeMensajesThread();

            return true;
        }

        // Difundimos un mensaje a todos los clientes. 
        private void DifundirMensaje(String mensaje)
        {
            lock (Clientes)
            {
                foreach (Socket socket in Clientes)
                {
                    byte[] mensajeBytes = Encoding.UTF8.GetBytes(mensaje);
                    socket.Send(mensajeBytes);
                }
            }
        }

        // Se encarga de repartir los mensajes en cola.
        private void IniciarProcesadorDeMensajesThread()
        {
            Thread ProcesadorDeMensajesThread = new Thread(() =>
            {
                while (ColaMensajes != null)
                {
                    if (ColaMensajes.Count != 0)
                    {
                        String mensaje = ColaMensajes.Dequeue();
                        DifundirMensaje(mensaje);
                        if (MensajeRecibido != null)
                            MensajeRecibido(mensaje);
                    }
                    Thread.Sleep(SLEEPTIME);
                }
                // Sería más bonito utilizar un ResetEvent. 
            });

            ProcesadorDeMensajesThread.IsBackground = true;
            ProcesadorDeMensajesThread.Start();
        }

        private void IniciarRecibirConexionesThread()
        {
            Thread RecibirConexionesThread = new Thread(() =>
            {
                while (Clientes.Count != CONEXIONESMAXIMAS)
                {
                    // Intentaremos encontrar a un cliente.
                    Socket client = null;
                    try
                    {
                        // Intentando encontrar alguna conexión entrante...

                        // Si no ejecutamos esta linea en un thread separado vamos a bloquear el programa.
                        client = SocketServer.Accept();

                        // Cliente conectado!

                        // Lo agregamos a nuestra lista.
                        lock (Clientes) // Quizás este lock esté de más.
                        {
                            Clientes.Add(client);
                        }

                        // Avisamos
                        if (UsuarioConectado != null)
                            UsuarioConectado();

                        // Iniciamos un Thread parametrizado que se dedicará a escuchar por este socket.
                        IniciarEscucharClienteThread(client);
                    }
                    catch (SocketException)
                    {

                    }
                }
            });

            RecibirConexionesThread.IsBackground = true;
            RecibirConexionesThread.Start();
        }

        private void IniciarEscucharClienteThread(Socket socket)
        {
            Thread EscucharClienteThread = new Thread(() =>
            {
                while (socket != null)
                {
                    string mensaje;
                    byte[] dataBuffer;
                    int largo;

                    try
                    {
                        dataBuffer = new byte[256];
                        // Debemos ejecutar esto en un thread o bloquearemos el thread principal (el que tiene la GUI).
                        largo = socket.Receive(dataBuffer);  // Este método se queda esperando hasta recibir algo.
                        mensaje = Encoding.UTF8.GetString(dataBuffer, 0, largo);

                        // Mandamos el mensaje a la cola de salida.
                        ColaMensajes.Enqueue(mensaje);
                    }
                    catch (SocketException)
                    {

                    }
                }
            });

            EscucharClienteThread.IsBackground = true;
            EscucharClienteThread.Start();
        }


        public static String GetLocalIP()
        {
            // Esto es como ir a la consola (cmd.exe) y ejecutar:
            // ipconfig 
            // Y ver cuál es nuestra dirección IP local.

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            IPAddress dir = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return dir.ToString();
        }
    }
}
