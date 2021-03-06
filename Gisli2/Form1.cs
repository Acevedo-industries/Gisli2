﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gisli2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


        }
       
        public int xClick = 0, yClick = 0;

        //PASO 2: en el evento MouseMove del Form
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            { xClick = e.X; yClick = e.Y; }
            else
            { this.Left = this.Left + (e.X - xClick); this.Top = this.Top + (e.Y - yClick); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            formulario_compra_venta ventana = new formulario_compra_venta();
            ventana.Show();
        }

        private void cerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            formulario_servicios ventana2 = new formulario_servicios();
            ventana2.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cotizacion ventana3 = new cotizacion();
            ventana3.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            formulario_compra_venta ventana2 = new formulario_compra_venta();
            ventana2.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            formulario_compra_venta ventana2 = new formulario_compra_venta();
            ventana2.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            formulario_servicios ventana2 = new formulario_servicios();
            ventana2.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            cotizacion ventana3 = new cotizacion();
            ventana3.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            formulario_servicios ventana2 = new formulario_servicios();
            ventana2.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            cotizacion ventana3 = new cotizacion();
            ventana3.Show();
        }
    }
}
