using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Tomograph_2lab
{
    public partial class Form1 : Form
    {
        private Bin Bin1 = new Bin();
        private View view = new View();
        bool loaded = false;
        int currentLayer = 0;
        private bool isPlaying = true;
        private DateTime lastUpdateTime = DateTime.Now;
        private int targetLayer = 0;
        private int playSpeed = 50;
        private bool useTextureMode = false;
        public Form1()
        {
            InitializeComponent();
            trackBar1.Maximum = 10;
        }

        private void VisualizationModeChanged(object sender, EventArgs e)
        {
            useTextureMode = radioButton1.Checked;
            if (loaded)
            {
                glControl1.Invalidate();
            }
        }

        private void открытьФайлСТомографиейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                Bin1.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                trackBar1.Maximum = Bin.Z - 1;
                trackBar1.Value = 0;
                loaded = true;
                isPlaying = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            {
                if (loaded)
                {
                    if ((isPlaying && (DateTime.Now - lastUpdateTime).TotalMilliseconds > playSpeed)&&(currentLayer<=targetLayer))
                    {
                        displayFPS();
                        currentLayer = (currentLayer + 1) % Bin.Z; 
                        needReload = true;
                        lastUpdateTime = DateTime.Now;
                    }

                    if (currentLayer >= targetLayer) currentLayer = 0;

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    if (useTextureMode)
                    {
                        if (needReload)
                        {
                       
                                view.generateTextureImage(currentLayer);
                                view.Load2DTexture();
                         
                            needReload = false;
                        }
                        view.DrawTexture();
                    }
                    else
                    {
                            view.DrawQuads(currentLayer);
                      
                    }

                    glControl1.SwapBuffers();

                    if (isPlaying)
                    {
                        glControl1.Invalidate();
                    }
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            targetLayer = trackBar1.Value; 
            if (!isPlaying)
            {
                currentLayer = targetLayer;
                needReload = true;
                glControl1.Invalidate();
            }
        }
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                //displayFPS();

                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("Tomography Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        bool needReload = false;

        private void glControl2_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                if (needReload)
                {
                    view.generateTextureImage(currentLayer);
                    view.Load2DTexture();
                    needReload = false;
                }
                view.DrawTexture();
                glControl1.SwapBuffers();
            }
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            useTextureMode = false;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.tfMin = trackBar2.Value;
            label1.Text = "минимум TF: " + trackBar2.Value;
            if (loaded)
            {
                if (useTextureMode) needReload = true; 

                glControl1.Invalidate();
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.tfWidth = trackBar3.Value;
            label2.Text = "ширина TF: " + trackBar3.Value;
            if (loaded)
            {
                if (useTextureMode) needReload = true;

                glControl1.Invalidate();
            }
        }
    }
}
