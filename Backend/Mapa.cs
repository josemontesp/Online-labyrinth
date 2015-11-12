using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Backend
{
    [Serializable]
    public class Mapa
    {
        public string[,] grilla;
        public int alto;
        public int largo;

        public Mapa(string nombre)
        {
            #region Cargar mapa desde archivo
            using (StreamReader r = new StreamReader("..\\..\\..\\" + nombre + ".txt"))
            {
                string[] dimensiones = r.ReadLine().Split(new string[] { "x" }, StringSplitOptions.None);
                alto = Int32.Parse(dimensiones[0]);
                largo = Int32.Parse(dimensiones[1]);
                grilla = new string[largo, alto];

                string line;
                int x = 0;
                int y = 0;
                while ((line = r.ReadLine()) != null)
                {
                    foreach (char c in line)
                    {
                        grilla[x, y] = c.ToString();
                        x++;
                    }
                    x = 0;
                    y++;
                }
            }
            #endregion
        }

        public string elemento(int x, int y)
        {
            return grilla[x,y];
        }



    }
}
