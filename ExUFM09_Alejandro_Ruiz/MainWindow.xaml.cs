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

namespace ExUFM09_Alejandro_Ruiz
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGen_Click(object sender, RoutedEventArgs e)
        {
            //btnGen.Content = 1 % 10 + 1;
            imgFoto.Source = generarImtage();
        }

        private void btnEst_Click(object sender, RoutedEventArgs e)
        {
            WriteableBitmap foto = new WriteableBitmap(1920,1080,100,100,PixelFormats.Bgr24, null);
            esteganografiar(foto, "hola");
        }

        public WriteableBitmap generarImtage()
        {
            //2 linies
            var widthImatge = 150;
            var heigthImatge = 150;
            WriteableBitmap imatge = new WriteableBitmap(widthImatge, heigthImatge, 100, 100, PixelFormats.Bgr24, null);
            var bytesPerPixel = PixelFormats.Bgr24.BitsPerPixel / 8;
            var stride = widthImatge * bytesPerPixel;
            int nbytesImatge = widthImatge * heigthImatge * bytesPerPixel;
            byte[] bytesImatge = new byte[nbytesImatge];

            int nByte = 0;
            for(int i =0; i< heigthImatge;i++)
            {
                for(int j=0; j<widthImatge;j++)
                {
                    bytesImatge[nByte] = 0;
                    bytesImatge[nByte+1] = 0;
                    bytesImatge[nByte + 2] = 255;
                    
                    if(j< widthImatge/2)
                    {
                        bytesImatge[nByte] = 255;
                        bytesImatge[nByte + 1] = 0;
                        bytesImatge[nByte + 2] = 0;
                    }
                    nByte += 3;
                }
            }
            imatge.WritePixels(new Int32Rect(0, 0, widthImatge, heigthImatge), bytesImatge, stride, 0);
            return imatge;
        }

        public WriteableBitmap esteganografiar(WriteableBitmap img, string msg)
        {
            var widthImatge = img.PixelWidth;
            var heigthImatge = img.PixelHeight;
            WriteableBitmap imatge = new WriteableBitmap(widthImatge, heigthImatge, 100, 100, PixelFormats.Bgr24, null);
            var bytesPerPixel = img.Format.BitsPerPixel / 8;
            var stride = widthImatge * bytesPerPixel;
            byte[] bytesImatge = new byte[widthImatge*heigthImatge*bytesPerPixel];
            img.CopyPixels(bytesImatge, stride, 0);

            char[] frase = msg.ToCharArray();
            int primerPixel = 0; //Anem de 2 en 2
            int lletraActual;
            int[] lletraBinari;

            var iniciP = pixel(primerPixel, 0, img) * bytesPerPixel;

            for(int i=0; i<frase.Length;i++)
            {
                lletraActual = (int)frase[i];
                lletraBinari = int2bin(lletraActual);

                int[] R, G, B;
                R = int2bin(bytesImatge[iniciP]);
                G = int2bin(bytesImatge[iniciP + 1]);
                B = int2bin(bytesImatge[iniciP + 2]);

                R[5] = lletraBinari[0];
                R[6] = lletraBinari[1];
                R[7] = lletraBinari[2];
                G[5] = lletraBinari[3];
                G[6] = lletraBinari[4];
                G[7] = lletraBinari[5];
                B[6] = lletraBinari[6];
                B[7] = lletraBinari[7];

                int RDec, GDec, BDec;
                RDec = bin2int(R);

                GDec = bin2int(G);
                BDec = bin2int(B);
                bytesImatge[iniciP] = (byte)RDec;
                bytesImatge[iniciP + 1] = (byte)GDec;
                bytesImatge[iniciP + 2] = (byte)BDec;

                primerPixel += 2; ;
                iniciP = pixel(primerPixel, 0, img) * bytesPerPixel;
            }
            imatge = new WriteableBitmap(img.PixelWidth, img.PixelHeight, img.DpiX, img.DpiY, PixelFormats.Pbgra32, null);
            imatge.WritePixels(new Int32Rect(0, 0, img.PixelWidth, img.PixelHeight), bytesImatge, stride, 0);
            return imatge;
        }
        private int pixel(int y, int x, WriteableBitmap imatge)
        {
            if (!(y < imatge.PixelHeight))
            {
                MessageBox.Show("Et passes de límit Y.", "Error", MessageBoxButton.OK);
            }
            if (!(x < imatge.PixelWidth))
            {
                MessageBox.Show("Et passes de límit X.", "Error", MessageBoxButton.OK);
            }
            return y * imatge.PixelWidth + x;
        }

        private int[] int2bin(int valor)
        {
            int[] arrayBinari = new int[8];
            var desp = 7;
            for (int i = 0; i < 8; i++)
            {
                arrayBinari[i] = valor >> desp & 1;
                desp--;
            }
            return arrayBinari;
        }

        private int bin2int(int[] binari)
        {
            int dcm = 0;
            int potencia = 0;

            for (int i = binari.Length - 1; i >= 0; i--)
            {
                if (binari[i] == 1)
                {
                    dcm += (int)Math.Pow((double)2, (double)potencia);
                }
                potencia++;
            }
            return dcm;
        }
    }


}
