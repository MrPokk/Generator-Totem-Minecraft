using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Generator
{
    public partial class Form1 : Form
    {

        public Bitmap _bitmapTotem;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;


            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {


            OpenFileDialog FileData = new OpenFileDialog();

            FileData.Filter = "Export PNG (*.png)|*.png";
            if (FileData.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap image = new Bitmap(FileData.FileName);
                    Bitmap totem = CreateBitmapTotem(image);

                    _bitmapTotem = totem;

                    pictureBox1.Image = ScaleBimaps(image, 4);
                    pictureBox2.Image = ScaleBimaps(totem, 5);

                }
                catch { }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1 != null)
            {
                SaveFileDialog FileData = new SaveFileDialog();
                FileData.Filter = "PNG (*.png)|*.png";
                if (FileData.ShowDialog() == DialogResult.OK)
                {
                    try
                    {

                        _bitmapTotem.Save(FileData.FileName, ImageFormat.Png);
                    }
                    catch { }
                }
            }
        }

        private Bitmap CreateBitmapTotem(Bitmap inputFile)
        {


            inputFile = new Bitmap(inputFile);
            Bitmap outputFile = new Bitmap(16, 16);

            Bitmap Head = CutBitmap(inputFile, 8, 8, new Point(3, 0), new Size(8, 8));
            Bitmap Body = CutBitmap(inputFile, 20, 20, new Point(3, 8), new Size(8, 4));
            Bitmap Leg = MergerBitmaps(CutBitmap(inputFile, 7, 28, new Point(4, 12), new Size(6, 2)), CutBitmap(inputFile, 8, 31, new Point(5, 14), new Size(4, 1)));

            Bitmap HandRight = MergerBitmaps(CutBitmap(inputFile, 33, 52, new Point(11, 8), new Size(2, 3)), CutBitmap(inputFile, 40, 48, new Point(13, 8), new Size(1, 2)));
            Bitmap HandLeft = MergerBitmaps(CutBitmap(inputFile, 33, 52, new Point(1, 8), new Size(2, 3)), CutBitmap(inputFile, 40, 48, new Point(0, 8), new Size(1, 2)));


            List<Bitmap> allElement = new List<Bitmap>() { Head, Body, HandLeft, HandRight, Leg };

            outputFile = MergerAllBitmaps(allElement, new Point(1, 1));


            return outputFile;

        }
        // Увеличение Размера картинки 
        private Bitmap ScaleBimaps(Bitmap inputBitmap, int resizeScale)
        {
            Bitmap scaleOutputFile = new Bitmap(inputBitmap.Width * resizeScale, inputBitmap.Height * resizeScale);
            using (Graphics GraphicsPixel = Graphics.FromImage(scaleOutputFile))
            {
                GraphicsPixel.InterpolationMode = InterpolationMode.NearestNeighbor;
                GraphicsPixel.DrawImage(inputBitmap, new Rectangle(0, 0, inputBitmap.Width * resizeScale, inputBitmap.Height * resizeScale));
            }
            return scaleOutputFile;
        }
        // Отризает часть от картинки
        private Bitmap CutBitmap(Bitmap inputBitmap, int poseX, int poseY, Point offset, Size bitmapArea)
        {
            //16 константа размера картинки для удобства
            Bitmap outputFile = new Bitmap(16, 16);

            for (int y = 0; y < bitmapArea.Height; y++)
            {
                for (int x = 0; x < bitmapArea.Width; x++)
                {
                    Color pixelColor = inputBitmap.GetPixel(x + poseX, y + poseY);
                    outputFile.SetPixel(x + offset.X, y + offset.Y, pixelColor);
                }
            }

            return outputFile;
        }
        // Формируте только 2 картинки
        private Bitmap MergerBitmaps(Bitmap BitmapOne, Bitmap BitmapTwo)
        {
            Bitmap result = new Bitmap(Math.Max(BitmapOne.Width, BitmapTwo.Width),
                                       Math.Max(BitmapOne.Height, BitmapTwo.Height));
            using (Graphics GraphicsPixel = Graphics.FromImage(result))
            {
                GraphicsPixel.DrawImage(BitmapOne, Point.Empty);
                GraphicsPixel.DrawImage(BitmapTwo, Point.Empty);

            }
            return result;
        }
        // Формирует общие картинки
        private Bitmap MergerAllBitmaps(List<Bitmap> allBitmap, Point offset)
        {
            int maxHeigh = allBitmap[0].Height;
            int maxWidth = allBitmap[0].Width;
            for (int i = 0; i < allBitmap.Count; i++)
            {
                if (maxHeigh < allBitmap[i].Height)
                {
                    maxHeigh = allBitmap[i].Height;
                }
                if (maxWidth < allBitmap[i].Width)
                {
                    maxWidth = allBitmap[i].Width;
                }
            }
            Bitmap reasult = new Bitmap(maxWidth, maxHeigh);
            using (Graphics GrahicsPixel = Graphics.FromImage(reasult))
            {
                for (int i = 0; i < allBitmap.Count; i++)
                {
                    GrahicsPixel.DrawImage(allBitmap[i], offset);
                }
            }
            return reasult;
        }

    }
}