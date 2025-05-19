using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Filters_lab1
{
    public partial class Form1 : Form
    {
        Bitmap image;
        Bitmap image1;
        Boolean i = false;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.MouseClick += pictureBox1_MouseClick;
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int areaSize = 10;
            int x = e.X;
            int y = e.Y;

            ApplySimpleFilterAroundPoint(x, y, areaSize);
            pictureBox1.Image = image;
            pictureBox1.Refresh();

        }

        private void ApplySimpleFilterAroundPoint(int centerX, int centerY, int size)
        {

            int startX = centerX - (size / 2);
            int startY = centerY - (size / 2);
            int endX = centerX + (size / 2);
            int endY = centerY + (size / 2);


            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int gray = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                    image.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertFilter fil = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(fil);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
                Bitmap newIm = ((FiltersByMe)e.Argument).processimage(image, backgroundWorker1);
                if (backgroundWorker1.CancellationPending != true)
                {
                    image = newIm;
                }          
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void гауссовоРазмытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void чБToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void яркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new BrightnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрСобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new SharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Embossing();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сдвигToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Move();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void волныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Waves();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpg|*.jpg";
            saveFileDialog1.Title = "Сохранить изображение!";
            saveFileDialog1.ShowDialog(); 
            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                fs.Close();
            }

        }

        private void стеклоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Glass();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void размытиеВДвиженииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new MotionBlur();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void выделениеГраницToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new BorderFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void волны2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Waves2();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            image = image1;
            pictureBox1.Image = image;
            pictureBox1.Refresh();
            i = true;
        }

        private void повернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Turn();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void dilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Dilation();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void erosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Erosion();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        int h = 0;
        int k = 0;
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (h == 0)
            {
                FiltersByMe filter1 = new Dilation();
                backgroundWorker1.RunWorkerAsync(filter1);
                h++;
            }
            else
            {
                return;
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted1(object sender, RunWorkerCompletedEventArgs e)
        {
            if (k == 0)
            {
                FiltersByMe filter1 = new Erosion();
                backgroundWorker1.RunWorkerAsync(filter1);
                k++;
            }
            else
            {
                return;
            }
        }

        private void openingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Erosion();
            backgroundWorker1.RunWorkerAsync(filter);
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
        }

        private void closingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Dilation();
            backgroundWorker1.RunWorkerAsync(filter);
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted1;
        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new Gray_World();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void autoContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersByMe filter = new AutoContrast();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void кольцаToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FiltersByMe filter = new RingFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseClick_1(object sender, MouseEventArgs e)
        {
            int areaSize = 100;
            
            int x = Form1.MousePosition.X - pictureBox1.Location.X; 
            int y = Form1.MousePosition.Y - pictureBox1.Location.Y;

            ApplySimpleFilterAroundPoint(x, y, areaSize);

            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }


        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files (*.*) | *.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                image = new Bitmap(dialog.FileName);
                pictureBox1.Width = image.Width/2;
                pictureBox1.Height = image.Height/2;
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        //private void pictureBox1_MouseClick(object sender, EventArgs e)
        //{
        //    int areaSize = 10;
        //    int x = e.X;
        //    int y = e.Y;

        //    ApplySimpleFilterAroundPoint(x, y, areaSize);
        //    pictureBox1.Image = image;
        //    pictureBox1.Refresh();

        //    float scaleX = (float)image.Width / pictureBox1.Width;
        //    float scaleY = (float)image.Height / pictureBox1.Height;
        //    int imgX = (int)(e.X * scaleX);
        //    int imgY = (int)(e.Y * scaleY);
        //    ApplySimpleFilterAroundPoint(imgX, imgY, 50);
        //}
    }
}
