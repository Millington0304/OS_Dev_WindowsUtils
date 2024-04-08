using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VHDWriter
{
    public partial class Form2 : Form
    {
        //private Boolean saved = false;

        public Form2()
        {
            InitializeComponent();
        }

        private Boolean checkLegalExp()
        {
            try
            {
                if (Convert.ToInt16(textBox1.Text) <= 15)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Form1 prtForm = (Form1)Owner;
#pragma warning disable CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception
            textBox1.Text = (prtForm.bytePerSector).ToString();
            label3.Text = "= " + Math.Pow(2, Convert.ToInt16(textBox1.Text));
#pragma warning restore CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception
            textBox3.Text = prtForm.runLoc;
            textBox2.Text = prtForm.debugLoc;
            Form2_Click(0, new EventArgs());

            textBox1.Select(textBox1.Text.Length, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2_Click(0, new EventArgs());

            Form1 prtForm=(Form1) Owner;
            prtForm.bytePerSector = Convert.ToByte(textBox1.Text);
            prtForm.runLoc = textBox3.Text;
            prtForm.debugLoc = textBox2.Text;
            Close();
            Dispose();
        }

        private void Form2_Click(object sender, EventArgs e)
        {
            if (checkLegalExp())
                label3.Text = "= " + Math.Pow(2, Convert.ToInt16(textBox1.Text));
            else
            {
                MessageBox.Show("Illegal input!");
                textBox1.Text = "9";
                label3.Text = "= 512";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                openFileDialog1.InitialDirectory = textBox3.Text.Substring(0, textBox3.Text.LastIndexOf('\\'));
                openFileDialog1.FileName = textBox3.Text.Substring(textBox3.Text.LastIndexOf('\\') + 1);
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                textBox3.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                openFileDialog2.InitialDirectory = textBox2.Text.Substring(0, textBox2.Text.LastIndexOf('\\'));
                openFileDialog2.FileName = textBox2.Text.Substring(textBox2.Text.LastIndexOf('\\') + 1);
            }

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
                textBox2.Text = openFileDialog2.FileName;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (saved || MessageBox.Show("Save?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saved = true;
                button1_Click(0, new EventArgs());
                return;
            }
            */
        }
    }
}
