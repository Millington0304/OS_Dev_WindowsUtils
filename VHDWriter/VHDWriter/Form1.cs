using System;
using System.IO;
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
    public partial class Form1 : Form
    {

        public Byte bytePerSector = 9;  //actual val=2^9
        public Int64 vhdSize = 0;
        public String runLoc = "";
        public String debugLoc = "";
        public readonly String[] suff = { ".img", ".vhd" };
        List<String> coms = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void hintOK()
        {
            label5.Text = "OK";
            label5.ForeColor = Color.Green;
            timer1.Start();
        }

        private void updateVHDSize()
        {
            FileInfo vhdFInfo = new FileInfo(textBox1.Text);
            vhdSize = vhdFInfo.Length;                          //vhdSize is in Byte
            textBox4.Text = (vhdSize / Math.Pow(2, bytePerSector)).ToString();
        }

        private void updateBinSize()
        {
            FileInfo vhdFInfo = new FileInfo(textBox2.Text);
            vhdSize = vhdFInfo.Length;                          
            label4.Text =vhdSize.ToString()+"B ("+ (vhdSize / Math.Pow(2, bytePerSector)).ToString()+" Sectors )";
        }

        private Boolean checkIsByte(String param)
        {
            try
            {
                Convert.ToByte(param);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private Boolean checkLegalSector(String param)
        {
            try
            {
                return (Convert.ToInt32(param)<=vhdSize);
            }
            catch
            {
                return false;
            }
            //return true;
        }

        private Boolean checkFileNameCorrect(String param,String[] suffix,String alertName)
        {
            if (!System.IO.File.Exists(param))
            {
                MessageBox.Show(alertName + " does not exists!");
                return false;
            }

            for (int i = 0; i < suffix.Length; i++)
            {
                if (suffix[i] == "" || param.EndsWith(suffix[i]))
                {
                    return true;
                }
            }

            MessageBox.Show(alertName + " has illegal extention!");
            return false;
        }

        private Boolean checkAllLegal()
        {
            if (!checkFileNameCorrect(textBox1.Text, suff, "VHD file"))
            {
                return false;
            }

            updateVHDSize();
            if (!checkLegalSector(textBox3.Text))
            {
                MessageBox.Show("Illigal Sector!");
                return false;
            }
            else if (!checkFileNameCorrect(textBox2.Text, new String[] { "" }, "Binary file"))
            {
                return false;
            }
            else if (textBox1.Text == textBox2.Text)
            {
                MessageBox.Show("Please make sure the file locations are not same!");
                return false;
            }
            return true;
        }

        private void readConfig()
        {
            String pathfilePath = Application.StartupPath + "\\config.ini";
            if (!File.Exists(pathfilePath))
            {
                FileStream pathfileCreator = new FileStream(pathfilePath, FileMode.CreateNew);
                pathfileCreator.Dispose();
                return;
            }

            StreamReader pathReader = new StreamReader(pathfilePath);
            textBox1.Text = pathReader.ReadLine();
            textBox2.Text = pathReader.ReadLine();
            bytePerSector = Convert.ToByte(pathReader.ReadLine());
            runLoc = pathReader.ReadLine();
            debugLoc = pathReader.ReadLine(); String tmp = "";
            String[] tmparr = new String[4];
            while (true)
            {
                tmp = pathReader.ReadLine();
                if (tmp == "EOL" || tmp == null)
                    break;
                tmparr = tmp.Split('&');
                listView1.Items.Add(new ListViewItem(tmparr));
            }
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (pathReader.ReadLine() == "1")
                    listView1.Items[i].Checked = true;
            }
            if (pathReader.ReadLine() == "1")
                checkBox1.Checked = true;
            pathReader.Close();
            pathReader.Dispose();
        }
        
        private void saveConfigs()
        {
            String pathfilePath = Application.StartupPath + "\\config.ini";
            StreamWriter confSaver = new StreamWriter(pathfilePath);
            confSaver.WriteLine(textBox1.Text);
            confSaver.WriteLine(textBox2.Text);
            confSaver.WriteLine(bytePerSector);
            confSaver.WriteLine(runLoc);
            confSaver.WriteLine(debugLoc);
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    confSaver.Write(listView1.Items[i].SubItems[j].Text + '&');
                }
                confSaver.WriteLine();
            }
            confSaver.WriteLine("EOL");
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked == true)
                    confSaver.WriteLine("1");
                else
                    confSaver.WriteLine("0");
            }
            if (checkBox1.Checked)
                confSaver.WriteLine("1");
            confSaver.Flush();
            confSaver.Close();
            confSaver.Dispose();
        }

        private void fillLastSector()
        {
            FileStream writer = new FileStream(textBox1.Text, FileMode.Open);
            writer.Seek(0, SeekOrigin.End);

            Byte[] zeroArr = new Byte[1];
            Int64 emptyArea = Convert.ToInt64(Math.Pow(2, bytePerSector)) * Convert.ToInt64(Math.Ceiling(Convert.ToSingle(textBox4.Text))) - vhdSize;
            for (int i = 0; i < emptyArea; i++)
            {
                writer.WriteByte(0x00);
            }
            writer.Flush();
            writer.Close();
            writer.Dispose();
        }

        private void writeEmptySector(Int32 count)
        {
            fillLastSector();

            Byte[] zeroArr = new Byte[64];
            FileStream writer = new FileStream(textBox1.Text, FileMode.Open);
            writer.Seek(0, SeekOrigin.End);
            for (int secNum = 1; secNum <= count; secNum++)
            {
                for (int i = 0; i < Math.Pow(2, bytePerSector) / 64; i++)
                {
                    writer.Write(zeroArr, 0, 64);
                }
            }
            writer.Flush();
            writer.Close();
            updateVHDSize();
            writer.Dispose();
        }

        private void writeEmptySector(Int32 startSector,Int32 count)
        {
            Byte[] zeroArr = new Byte[64];
            FileStream writer = new FileStream(textBox1.Text, FileMode.Open);
            writer.Seek((startSector-1) * Convert.ToInt64(Math.Pow(2, bytePerSector)), SeekOrigin.Begin);
            for (int secNum = 1; secNum <= count; secNum++)
            {
                for (int i = 0; i < Math.Pow(2, bytePerSector) / 64; i++)
                {
                    writer.Write(zeroArr, 0, 64);
                }
            }
            writer.Flush();
            writer.Close();
            updateVHDSize();
            writer.Dispose();
        }


        private void writeButton_Click(object sender, EventArgs e)
        {
            if (!checkAllLegal())
                return;
            Byte[] buffer;
            FileStream writer;
            FileStream reader;
            ListViewItem.ListViewSubItemCollection a;
            String err = "";
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked)
                {
                    a = listView1.Items[i].SubItems;
                    buffer = new Byte[Convert.ToInt32(new FileInfo(a[0].Text).Length)];
                    reader = new FileStream(a[0].Text, FileMode.Open);
                    reader.Read(buffer, 0, Convert.ToInt32(new FileInfo(a[0].Text).Length));
                    reader.Close();
                    try
                    {
                        writer = new FileStream(a[1].Text, FileMode.Open);
                        writer.Seek((Convert.ToInt32(a[2].Text) - 1) * Convert.ToInt64(Math.Pow(2, bytePerSector)), SeekOrigin.Begin);
                        writer.Write(buffer, 0, buffer.Length);
                        writer.Flush();
                        writer.Close();
                    }
                    catch (IOException)
                    {
                        err += a[1].Text + " on using!\n";
                    }
                }
            }
            if (err != "")
            {
                MessageBox.Show(err, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkBox1.Checked)
                MessageBox.Show("Write OK!");
            hintOK();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                openFileDialog1.InitialDirectory = textBox1.Text.Substring(0, textBox1.Text.LastIndexOf('\\')); ;
                openFileDialog1.FileName = textBox1.Text.Substring(textBox1.Text.LastIndexOf('\\') + 1);
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                updateVHDSize();
            }
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

        private void filesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 settingForm = new Form2();
            settingForm.ShowDialog(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("Bin",200);
            listView1.Columns.Add("VHD", 200);
            listView1.Columns.Add("Sector", 60);
            //listView1.Items.Add(textBox2.Text, textBox1.Text, textBox3.Text);

            readConfig();
            if (textBox1.Text != "" && checkFileNameCorrect(textBox1.Text, suff, "Old VHD File"))
                updateVHDSize();
            else
                textBox1.Text = "";

            if (textBox2.Text != "" && checkFileNameCorrect(textBox2.Text, new String[1] {""}, "Old Binary File"))
                updateBinSize();
            else
                textBox2.Text = "";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveConfigs();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkFileNameCorrect(textBox1.Text, suff, "VHD File"))
            {
                if (Convert.ToInt32(textBox3.Text) <= Convert.ToSingle(textBox4.Text))
                {
                    writeEmptySector(Convert.ToInt32(textBox3.Text), 1);
                }
                else
                {
                    writeEmptySector(Convert.ToInt32(textBox3.Text) - Convert.ToInt32(Math.Ceiling(Convert.ToSingle(textBox4.Text))));
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FileStream writer = new FileStream(textBox1.Text, FileMode.Open);
            writer.Seek(510, SeekOrigin.Begin);
            writer.WriteByte(0x55);
            writer.WriteByte(0xaa);
            writer.Flush();
            writer.Close();
            writer.Dispose();
            
            if(checkBox1.Checked)
                MessageBox.Show("0x55aa write OK!");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            label5.Text = "";
            label5.ForeColor = Color.Black;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (checkFileNameCorrect(textBox1.Text, suff, "VHD File"))
            {
                if (Convert.ToInt32(textBox3.Text) <= Convert.ToSingle(textBox4.Text))
                {
                    writeEmptySector(1, Convert.ToInt32(textBox3.Text));
                }
                else
                {
                    if (MessageBox.Show("Will Expand Size, Sure?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        writeEmptySector(1, Convert.ToInt32(textBox3.Text));
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            p.StandardInput.WriteLine("explorer.exe " + textBox2.Text.Substring(0, textBox2.Text.LastIndexOf('\\')));
            p.StandardInput.WriteLine("exit");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            p.StandardInput.WriteLine("explorer.exe " + textBox1.Text.Substring(0, textBox1.Text.LastIndexOf('\\')));
            p.StandardInput.WriteLine("exit");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            p.StandardInput.WriteLine("explorer.exe " + textBox2.Text);
            p.StandardInput.WriteLine("exit");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            p.StandardInput.WriteLine("explorer.exe " + textBox1.Text);
            p.StandardInput.WriteLine("exit");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (debugLoc == "")
                return;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = debugLoc;
            /*
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            */
            p.Start();//启动程序

            /*
            MessageBox.Show("cd " + debugLoc.Substring(0, debugLoc.LastIndexOf('\\') + 1));
            p.StandardInput.WriteLine("cd "+debugLoc.Substring(0, debugLoc.LastIndexOf('\\') + 1));
            p.StandardInput.WriteLine(debugLoc);
            */
            //p.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (runLoc == "")
                return;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = runLoc;
            p.Start();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count != 0)
                listView1.Items.RemoveAt(listView1.SelectedIndices[0]);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (!checkAllLegal())
                return;
            listView1.Items.Add(new ListViewItem(new String[] { textBox2.Text, textBox1.Text, textBox3.Text }));
        }
    }
}
