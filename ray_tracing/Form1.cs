using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ray_tracing
{
    public partial class Form1 : Form
    {
        private View view;

        public Form1()
        {
            InitializeComponent();
            t1 = new TrackBar();
            t1.Minimum = -20;
            t1.Maximum = 20;
            t1.Value = 0;
            t1.Dock = DockStyle.Right; // Или другое расположение
            this.Controls.Add(t1); 
            t1.Minimum = -100;
            t1.Maximum = 100;
            t1.Value = 0;
            t1.Scroll += (s, e) => {
                view.SetCameraOffset(t1.Value / 10.0f);
                glControl1.Invalidate(); // Важно: вызываем Invalidate для обновления
            };

            trackBar2.Minimum = -100;
            trackBar2.Maximum = 100;
            trackBar2.Value = 0;
            trackBar2.Scroll += (s, e) => {
                view.SetCameraOffset1(trackBar2.Value /10.0f);
                glControl1.Invalidate(); // Важно: вызываем Invalidate для обновления
            };

            trackBar1.Minimum = -200;
            trackBar1.Maximum = 100;
            trackBar1.Value = -80;
            trackBar1.Scroll += (s, e) => {
                view.SetCameraOffset2(trackBar1.Value / 10.0f);
                glControl1.Invalidate(); 
            };
            trackBar3.Minimum = 0;
            trackBar3.Maximum = 200;
            trackBar3.Value = 0;
            trackBar3.Scroll += (s, e) => {
                view.SetRW(trackBar3.Value / 100.0f);
                glControl1.Invalidate();
            };

            glControl1.Dock = DockStyle.Fill;
            view = new View();

            glControl1.Load += glControl1_Load;
            glControl1.Paint += glControl1_Paint;
            glControl1.Resize += glControl1_Resize;

        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();
            GL.Enable(EnableCap.DepthTest);
            view.InitShaders();
            view.InitBuffers();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            glControl1.Invalidate();
        }

        private void Render()
        {
            view.Draw();
            glControl1.SwapBuffers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            view.SetCubeColor(new Vector3(
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble()));
            view.Draw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            view.SetTetraColor(new Vector3(
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble()));
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            view.SetCameraOffset(t1.Value / 100.0f);
            Render();
        }
    }
}
