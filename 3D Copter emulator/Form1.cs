using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

namespace _3D_Copter_emulator
{
    public partial class Form1 : Form
    {
        long time = 0;
        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // инициализация Glut 
            MainScene.init(AnT.Width, AnT.Height);

            MainScene.camCenter[0] = 4;
            MainScene.camCenter[1] = 4;
            MainScene.camCenter[2] = 2;

            MainScene.camAngle = 25;
            MainScene.camDist = 4;
            MainScene.camPos[2] = 4.5;

            trackBar3.Value = (int)MainScene.camAngle;
            trackBar4.Value = (int)(MainScene.camPos[2]*10);
            trackBar5.Value = (int)(MainScene.camDist*100);

            MainScene.computeCamState();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = trackBar2.Value;
        }
        double a = 0.5, b = 0.5;
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            MainScene.quadrocopter.centerCoord[0] = 4 + a * Math.Sin(time / 33.0) + b * Math.Cos(time / 33.0);
            MainScene.quadrocopter.centerCoord[1] = 4 - a * Math.Sin(time / 33.0) + b * Math.Cos(time / 33.0);
            MainScene.quadrocopter.centerCoord[2] = 2;

            MainScene.quadrocopter.angles[0] = -Math.Sin(time / 33.0)*30;
            MainScene.quadrocopter.angles[1] = -Math.Cos(time / 33.0) * 30;

            time++;

            if (checkBox1.Checked)
            {
                MainScene.camAngle -= 0.1;
                MainScene.computeCamState();
            }

            //MainScene.quadrocopter.angles[2] += 1;
            MainScene.DrawGLScene();
            AnT.Invalidate();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            //MainScene.camPos[0] = 5;
            //MainScene.camPos[1] = 5;
            MainScene.camPos[2] = trackBar4.Value/10.0;
            label3.Text = "Height " + (trackBar4.Value/10.0).ToString();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            MainScene.camAngle = trackBar3.Value;
            MainScene.computeCamState();
            label1.Text = "Angle " + trackBar3.Value.ToString();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            MainScene.camDist = trackBar5.Value/100.0;
            MainScene.computeCamState();
            label2.Text = "Distance " + ( trackBar5.Value / 100.0 ).ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
