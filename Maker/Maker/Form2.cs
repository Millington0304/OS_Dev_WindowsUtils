using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maker
{
    public partial class Form2 : Form
    {
        private void SaveSettings()
        {
            Form1 parentFrm = (Form1)Owner;

            parentFrm.nasmPath = textBox1.Text;
            parentFrm.CygwinPath = textBox2.Text;
            parentFrm.gccPath = textBox3.Text;
        }

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox1.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Form1 parentFrm = (Form1)Owner;

            textBox1.Text = parentFrm.nasmPath;
            textBox2.Text = parentFrm.CygwinPath;
            textBox3.Text = parentFrm.gccPath;

            textBox1.Select(textBox1.Text.Length, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.SelectedPath = textBox2.Text;

            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }
    }
}
