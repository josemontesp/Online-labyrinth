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
using System.Windows.Shapes;
using Backend;
using System.Windows.Threading;

namespace Chat2___Client
{
    /// <summary>
    /// Lógica de interacción para Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        private Client Cliente { get; set; }
        private Mapa mapa;
        public List<string> comandos = new List<string>(new string[] { "Right", "Left", "Up", "Down", "A", "Democracy", "Anarchy", "Boom", "Antiboom" });
        public List<string> emotes = new List<string>(new string[] { "Kappa", "Kreygasm", "FrankerZ", "BibleThump", "FailFish"});
        public bool caminable = false;
        public List<Image> adyacentes;
        public bool llave = false;
        public Queue<string> comandoshistoricos = new Queue<string>();
        public bool anarquia = true;
        public string boomCount = "OFF"; // 0:OFF, 1:READY, 2:SET, 3:BOOM!
        public DispatcherTimer reloj = new DispatcherTimer();

        public Chat(Client cliente, Mapa _mapa)
        {
            InitializeComponent();
            reloj.Interval = new TimeSpan(0, 0, 1);
            reloj.Tick += reloj_Tick;
            reloj.Start();
            InputText.Focus();
            this.mapa = _mapa;
            CargarMapa();
            this.Cliente = cliente;

            this.Cliente.Boom += Cliente_Boom;

            this.Cliente.MoverGrilla += Cliente_MoverGrilla;

            this.Cliente.Accion += Cliente_Accion;

            this.Cliente.MensajeRecibido += (texto) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    char[] splitchar = { '|' };
                    var mensaje = texto.Split(splitchar);
                    if (comandos.Contains(mensaje[2]))
                    {
                        comandoshistoricos.Enqueue(mensaje[2]);
                        ComandosHistoricosFront.Children.Add(new TextBlock { Text = mensaje[2] });
                        ScrollViewerComandos.ScrollToEnd();
                        while (comandoshistoricos.Count > 100)
                            comandoshistoricos.Dequeue();
                    }
                    if (emotes.Contains(mensaje[2]))
                    {
                        StackPanelMensajes.Children.Add(stringToImage(mensaje[2]));
                    }
                    else
                    {
                        StackPanelMensajes.Children.Add(new TextBlock
                        {
                            Text = mensaje[1] + " -> " + mensaje[2],
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(mensaje[0])),
                            FontWeight = (comandos.Contains(mensaje[2]) ? FontWeights.Bold : FontWeights.Normal)
                        });
                    }
                    SrollViewer.ScrollToEnd();
                    actualizarSistemaDeGobierno();
                }));
                
            };

            InputText.KeyUp += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    EnviarMensaje();
                }
            };
        }

        void reloj_Tick(object sender, EventArgs e)
        {
            string texto = tiempo.Text;
            char[] splitchar = { ':' };
            var aux = texto.Split(splitchar);
            int[] Tiempo = new int[3];
            Tiempo[0] = Int32.Parse(aux[0]);
            Tiempo[1] = Int32.Parse(aux[1]);
            Tiempo[2] = Int32.Parse(aux[2]);

            Tiempo[2]++;
            if (Tiempo[2] == 60)
            {
                Tiempo[2] = 0;
                Tiempo[1]++;
            }
            if (Tiempo[1] == 60)
            {
                Tiempo[1] = 0;
                Tiempo[0]++;
            }
            tiempo.Text = Tiempo[0].ToString() + ":" + Tiempo[1].ToString() + ":" + Tiempo[2].ToString();
        }

        void Cliente_Boom(bool obj)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (obj)
                {
                    if (boomCount == "OFF")
                    {
                        boomCount = "READY";
                        boom.Text = "READY";
                    }
                    else if (boomCount == "READY")
                    {
                        boomCount = "SET";
                        boom.Text = "SET";
                    }
                    else if (boomCount == "READY")
                    {
                        boomCount = "SET";
                        boom.Text = "SET";
                    }
                    else if (boomCount == "SET")
                    {
                        Grilla = new Grid();
                        MyCanvas.Children.Clear();
                        MyCanvas.Children.Add(Grilla);
                        CargarMapa();
                        llave = false;
                        boomCount = "OFF";
                        boom.Text = "OFF";
                        ComandosHistoricosFront.Children.Clear();

                    }
                }
                else
                {
                    if (boomCount == "READY")
                    {
                        boomCount = "OFF";
                        boom.Text = "OFF";
                    }
                    else if (boomCount == "SET")
                    {
                        boomCount = "READY";
                        boom.Text = "READY";
                    }
                }
            }));
        }

        void Cliente_Accion()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                actualizarAdyacentes();
                var converter = new ImageSourceConverter();
                foreach (Image i in adyacentes)
                {
                    var source = i.Source.ToString();
                    if (source == "..\\..\\..\\Imagenes\\Puerta Cerrada.png")
                        i.Source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Puerta Abierta.png");
                    else if (source == "..\\..\\..\\Imagenes\\Llave.png")
                    {
                        i.Source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Suelo.png");
                        llave = true;
                        var imagen = stringToImage("KI");
                        MyCanvas.Children.Add(imagen);
                    }
                }
            }));
        }

        void Cliente_MoverGrilla(string comando)
        {
            double deltaX = 0.0;
            double deltaY = 0.0;
            lock (mapa)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    setBool(Caminable(comando));
                }));
                System.Threading.Thread.Sleep(10); // Para que termine el dispatcher
                if (comando == "Right" && caminable)
                {
                    deltaX = -32;
                }
                else if (comando == "Left" && caminable)
                {
                    deltaX = 32;
                }
                else if (comando == "Up" && caminable)
                {
                    deltaY = 32;
                }
                else if (comando == "Down" && caminable)
                {
                    deltaY = -32;
                }
                for (int i = 0; i < 32; i++)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Canvas.SetLeft(Grilla, Canvas.GetLeft(Grilla) + deltaX / 32);
                        Canvas.SetTop(Grilla, Canvas.GetTop(Grilla) + deltaY / 32);
                    }));
                    System.Threading.Thread.Sleep(8);
                }
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    int xPos = ((Int32)Canvas.GetLeft(Grilla) / 32) * -1;
                    int yPos = ((Int32)Canvas.GetTop(Grilla) / 32) * -1;
                    int AshPosX = 8 + xPos;
                    int AshPosY = 8 + yPos;
                    if (GetElementGrid(AshPosX, AshPosY).Source.ToString() == "..\\..\\..\\Imagenes\\Pozo.png")
                    {
                        Grilla = new Grid();
                        MyCanvas.Children.Clear();
                        MyCanvas.Children.Add(Grilla);
                        CargarMapa();
                        llave = false;
                        boomCount = "OFF";
                        boom.Text = "OFF";
                        ComandosHistoricosFront.Children.Clear();
                    }
                }));


            }
        }

        public void setBool(bool b)
        {
            caminable = b;
        }

        private void EnviarMensaje()
        {
            Cliente.EnviarMensaje(InputText.Text);
            InputText.Text = "";
        }

        public void CargarMapa()
        {
            #region Creacion de filas y columnas del grid
            for (int x = 0; x < mapa.largo; x++)
            {
                ColumnDefinition columna = new ColumnDefinition();
                columna.Width = new GridLength(32);
                Grilla.ColumnDefinitions.Add(columna);
            }
            for (int y = 0; y < mapa.alto; y++)
            {
                RowDefinition fila = new RowDefinition();
                fila.Height = new GridLength(32);
                Grilla.RowDefinitions.Add(fila);
            }
            #endregion

            for (int x = 0; x < mapa.largo; x++)
            {
                for (int y = 0; y < mapa.alto; y++)
                {
                    Image img = stringToImage(mapa.elemento(x, y));
                    Grid.SetColumn(img, x);
                    Grid.SetRow(img, y);
                    Grilla.Children.Add(img);
                }
            }
            Grilla.ShowGridLines = false;
            Canvas.SetLeft(Grilla, 32 * (1));
            Canvas.SetTop(Grilla, -32 * (8));
            Image ash = stringToImage("A");
            Canvas.SetTop(ash, 256);
            Canvas.SetLeft(ash, 256);

            MyCanvas.Children.Add(ash);
        }

        public Image stringToImage(string s)
        {
            var converter = new ImageSourceConverter();
            ImageSource source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Suelo.png");
            if (s == "X")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Pared.png");
            else if (s == "S")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Salida.png");
            else if (s == "C")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Suelo.png");
            else if (s == "E")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Entrada.png");
            else if (s == "P")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Puerta Cerrada.png");
            else if (s == "K")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Llave.png");
            else if (s == "A")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Ash.png");
            else if (s == "R")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\R.png");
            else if (s == "L")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\L.png");
            else if (s == "U")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\U.png");
            else if (s == "D")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\D.png");
            else if (s == "O")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Pozo.png");
            else if (s == "KI")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Llave Inventario.png");
            else if (s == "Kappa")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Kappa.png");
            else if (s == "Kreygasm")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\Kreygasm.png");
            else if (s == "FrankerZ")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\FrankerZ.png");
            else if (s == "BibleThump")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\BibleThump.png");
            else if (s == "FailFish")
                source = (ImageSource)converter.ConvertFromString(@"..\..\..\Imagenes\FailFish.png");
            var img = new Image()
            {
                Source = source,
                Height = 30,
                Width = 30,
            };
            return img;
        }

        public bool Caminable(string direccion)
        {
            int xPos = 0;
            int yPos = 0;
            string QueHay = "";
            xPos = ((Int32)Canvas.GetLeft(Grilla) / 32) * -1;
            yPos = ((Int32)Canvas.GetTop(Grilla) / 32) * -1;
            int AshPosX = 8 + xPos;
            int AshPosY = 8 + yPos;
            if (direccion == "Right")
                QueHay = GetElementGrid(AshPosX + 1, AshPosY).Source.ToString();
            else if (direccion == "Left")
                QueHay = GetElementGrid(AshPosX - 1, AshPosY).Source.ToString();
            else if (direccion == "Up")
                QueHay = GetElementGrid(AshPosX, AshPosY - 1).Source.ToString();
            else if (direccion == "Down")
                QueHay = GetElementGrid(AshPosX, AshPosY + 1).Source.ToString();
            if (QueHay == "..\\..\\..\\Imagenes\\Suelo.png")
                return true;
            if (QueHay == "..\\..\\..\\Imagenes\\Pared.png")
                return false;
            if (QueHay == "..\\..\\..\\Imagenes\\Puerta Cerrada.png")
                return false;
            if (QueHay == "..\\..\\..\\Imagenes\\R.png")
                return (direccion == "Right") ? true : false;
            if (QueHay == "..\\..\\..\\Imagenes\\L.png")
                return (direccion == "Left") ? true : false;
            if (QueHay == "..\\..\\..\\Imagenes\\U.png")
                return (direccion == "Up") ? true : false;
            if (QueHay == "..\\..\\..\\Imagenes\\D.png")
                return (direccion == "Down") ? true : false;
            return true;
        }

        public Image GetElementGrid(int columna, int fila)
        {
            Image img = null;

            foreach (UIElement ui in Grilla.Children)
            {
                var column = Grid.GetColumn(ui);
                if (Grid.GetColumn(ui) == columna && Grid.GetRow(ui) == fila)
                {
                    return (Image)ui;
                }
            }

            return img;
        }

        public void actualizarAdyacentes()
        {
            int GridXPos = ((Int32)Canvas.GetLeft(Grilla) / 32) * -1;
            int GridYPos = ((Int32)Canvas.GetTop(Grilla) / 32) * -1;
            int AshPosX = 8 + GridXPos;
            int AshPosY = 8 + GridYPos;
            adyacentes = new List<Image>();
            for (int dx = -1; dx < 2; dx++)
            {
                for (int dy = -1; dy < 2; dy++)
                {
                    var elemento = GetElementGrid(AshPosX + dx, AshPosY + dy);
                    if (elemento != null)
                        adyacentes.Add(elemento);
                }
            }
        }

        public void actualizarSistemaDeGobierno()
        {
            double demo = 0.0;
            double anar = 0.0;
            foreach(string c in comandoshistoricos)
            {
                if (c == "Democracy")
                    demo++;
                else if (c == "Anarchy")
                    anar++;
            }
            double total = demo + anar;
            double porcDemo = 0;
            double porcAnar = 0;
            if (total > 3)
            {
                porcDemo = (demo / total) * 100;
                porcAnar = (anar / total) * 100;
                if (porcAnar > 80.0)
                    anarquia = true;
                if (porcDemo > 80.0)
                    anarquia = false;
            }
            
            Dispatcher.BeginInvoke(new Action(() =>
            {
                anarquiaContent.Text = Math.Round(porcAnar).ToString() + "%";
                democracia.Text = Math.Round(porcDemo).ToString() + "%";
                if (anarquia)
                {
                    AnarquiaText.Foreground = Brushes.Red;
                    DemocraciaText.Foreground = Brushes.Blue;
                }
                else
                {
                    AnarquiaText.Foreground = Brushes.Blue;
                    DemocraciaText.Foreground = Brushes.Red;
                }

            }));

        }


    }
}
