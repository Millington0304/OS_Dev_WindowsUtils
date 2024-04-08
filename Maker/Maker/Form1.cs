using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Maker
{
    public partial class Form1 : Form
    {
        private String confPath = "";
        public String nasmPath = "";
        public String CygwinPath = "";
        public String gccPath = "/usr/bin/gcc";
        public Int32 bytePerSector = 512;
        List<String> coms = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void resetNasmPath()
        {
            nasmPath = "C:\\Program Files\\NASM";
        }

        private void resetCygwinPath()
        {
            CygwinPath = "C:\\Cygwin\\bin";
        }

        private String treatCpath(String param)
        {
            String tmp=param;
            tmp=tmp.Replace('\\', '/');
            tmp=tmp.Replace(":", "");
            return tmp;
        }

        private void updateOutSize()
        {
            label4.Text = "";
            FileInfo fi = new FileInfo("c:\\1.txt");
            if(coms.Count==0)
            {
                label4.Text = "0 Sector";
                return;
            }
            for (int i = 0; i < coms.Count; i++)
            {
                if (File.Exists(coms[0].Substring(coms[0].LastIndexOf("-o ") + 3)))
                {
                    fi = new FileInfo(coms[0].Substring(coms[0].LastIndexOf("-o ") + 3));
                    label4.Text += (fi.Length / Convert.ToDouble(bytePerSector)).ToString()+";";
                }
                else
                {
                    label4.Text += "0;";
                }
            }
            label4.Text += " Sector";
        }

        private void readConf()
        {
            if(!File.Exists(confPath))
            {
                FileStream confCreator = new FileStream(confPath, FileMode.CreateNew);
                resetNasmPath();
                resetCygwinPath();
                return;
            }

            StreamReader confReader = new StreamReader(confPath);
            nasmPath = confReader.ReadLine();
            textBox1.Text = confReader.ReadLine();
            textBox2.Text = confReader.ReadLine();
            textBox3.Text = confReader.ReadLine();
            textBox4.Text = confReader.ReadLine();
            CygwinPath = confReader.ReadLine();
            gccPath = confReader.ReadLine();
            String tmp = "";
            String[] tmparr = new String[4];
            while(true)
            {
                tmp = confReader.ReadLine();
                if (tmp == "EOL" ||tmp==null)
                    break;
                tmparr = tmp.Split('&');
                listView1.Items.Add(new ListViewItem(tmparr));
            }
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (confReader.ReadLine() == "1")
                    listView1.Items[i].Checked = true;
            }
            textBox5.Text = confReader.ReadLine();
            if (confReader.ReadLine() == "1")
                checkBox1.Checked = true;

            textBox6.Text = confReader.ReadLine();
            if (confReader.ReadLine() == "1")
                checkBox6.Checked = true;
            if (confReader.ReadLine() == "1")
                checkBox7.Checked = true;
            if (confReader.ReadLine() == "1")
                checkBox8.Checked = true;

            String[] cFileNameArr=new String[2];
            while (true)
            {
                tmp = confReader.ReadLine();
                if (tmp == "EOL" || tmp == null)
                    break;
                cFileNameArr = tmp.Split('&');
                listView2.Items.Add(new ListViewItem(cFileNameArr));
            }

            for (int i = 0; i < listView2.Items.Count; i++)
            {
                if (confReader.ReadLine() == "1")
                    listView2.Items[i].Checked = true;
            }
            if (confReader.ReadLine() == "1")
                checkBox9.Checked = true;
            confReader.Close();
            confReader.Dispose();

            if (nasmPath == "")
                resetNasmPath();
            if (CygwinPath == "")
                resetCygwinPath();
            if (gccPath == "")
                gccPath = "/usr/bin/gcc";

            //listView1.Items.RemoveAt(listView1.Items.Count - 1);
        }

        private void saveConf()
        {
            StreamWriter confSaver = new StreamWriter(confPath,false);
            confSaver.WriteLine(nasmPath);
            confSaver.WriteLine(textBox1.Text);
            confSaver.WriteLine(textBox2.Text);
            confSaver.WriteLine(textBox3.Text);
            confSaver.WriteLine(textBox4.Text);
            confSaver.WriteLine(CygwinPath);
            confSaver.WriteLine(gccPath);
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    confSaver.Write(listView1.Items[i].SubItems[j].Text+"&");
                }
                confSaver.Write("\n");
            }
            //confSaver.WriteLine();
            confSaver.WriteLine("EOL");
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked == true)
                    confSaver.WriteLine("1");
                else
                    confSaver.WriteLine("0");
            }
            confSaver.WriteLine(textBox5.Text);
            if (checkBox1.Checked)
                confSaver.WriteLine("1");
            else
                confSaver.WriteLine("0");

            confSaver.WriteLine(textBox6.Text);
            if (checkBox6.Checked)
                confSaver.WriteLine("1");
            else
                confSaver.WriteLine("0");
            if (checkBox7.Checked)
                confSaver.WriteLine("1");
            else
                confSaver.WriteLine("0");
            if (checkBox8.Checked)
                confSaver.WriteLine("1");
            else
                confSaver.WriteLine("0");


            for (int i = 0; i < listView2.Items.Count; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    confSaver.Write(listView2.Items[i].SubItems[j].Text + "&");
                }
                confSaver.Write("\n");
            }
            //confSaver.WriteLine();
            confSaver.WriteLine("EOL");
            for (int i = 0; i < listView2.Items.Count; i++)
            {
                if (listView2.Items[i].Checked == true)
                    confSaver.WriteLine("1");
                else
                    confSaver.WriteLine("0");
            }

            if (checkBox9.Checked)
                confSaver.WriteLine("1");
            else
                confSaver.WriteLine("0");

            confSaver.Flush();
            confSaver.Close();
        }

        private void hintOKASM()
        {
            label7.Text = "OK";
            timer1.Start();
        }

        private void hintOKC()
        {
            label8.Text = "OK";
            timer2.Start();
        }

        private void addListFunc(String param)
        {
            if (checkBox2.Checked)
                param += "-E ";
            if (checkBox3.Checked)
                param += "-O2 ";
            if (checkBox4.Checked)
                param += "-Ox ";
            if (checkBox5.Checked)
                param += "-f elf ";
            param = param.Trim();
            param = " " + param + " ";
            listView1.Items.Add(new ListViewItem(new String[] { textBox1.Text, textBox2.Text, param, textBox5.Text }));
        }

        void BuildList()
        {//= new String[listView1.Items.Count];
            //String makeCommand = "";
            coms = new List<String>();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked == true)
                    coms.Add("nasm" + listView1.Items[i].SubItems[2].Text + listView1.Items[i].SubItems[0].Text + " -o " + listView1.Items[i].SubItems[1].Text.Substring(0, listView1.Items[i].SubItems[1].Text.LastIndexOf('.')) + listView1.Items[i].SubItems[3].Text);
            }

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            p.StandardInput.WriteLine("cd " + nasmPath);
            for (int i = 0; i < coms.Count; i++)
            {
                p.StandardInput.WriteLine(coms[i]);
            }
            p.StandardInput.WriteLine("exit");

            String tmp = p.StandardError.ReadToEnd();
            if (tmp != "")
            {
                MessageBox.Show(tmp);
                return;
            }
            if (checkBox1.Checked)
                MessageBox.Show("Machine Code Write OK!");
            hintOKASM();
            updateOutSize();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                openFileDialog1.InitialDirectory = textBox1.Text.Substring(0, textBox1.Text.LastIndexOf('\\'));
                openFileDialog1.FileName = textBox1.Text.Substring(textBox1.Text.LastIndexOf('\\') + 1);
            }

            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveConf();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label7.Text = label8.Text = "";

            listView1.View = View.Details;
            listView1.Columns.Add("In File", 200, HorizontalAlignment.Left);
            listView1.Columns.Add("Out File", 200, HorizontalAlignment.Left);
            listView1.Columns.Add("Param", 60, HorizontalAlignment.Left);
            listView1.Columns.Add("Ext", 60, HorizontalAlignment.Left);

            listView2.View = View.Details;
            listView2.Columns.Add("In File", 300, HorizontalAlignment.Left);
            listView2.Columns.Add("Is CPP", 60, HorizontalAlignment.Left);
            //listView1.Items.Add(new ListViewItem(new String[] {"Para","C://","D://",".asm"}));

            confPath = Application.StartupPath + "\\conf.ini";
            readConf();
            textBox1.Select(textBox1.Text.Length,0);
            listView1.Update();
            updateOutSize();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                saveFileDialog1.InitialDirectory = textBox2.Text.Substring(0, textBox2.Text.LastIndexOf('\\'));
                saveFileDialog1.FileName = textBox2.Text.Substring(textBox2.Text.LastIndexOf('\\') + 1);
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = saveFileDialog1.FileName;
                updateOutSize();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 settingFrm = new Form2();
            settingFrm.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            addListFunc("");
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                saveFileDialog2.InitialDirectory = textBox3.Text.Substring(0, textBox3.Text.LastIndexOf('\\'));
                saveFileDialog2.FileName = textBox3.Text.Substring(textBox3.Text.LastIndexOf('\\') + 1);
            }

            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = saveFileDialog2.FileName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                openFileDialog2.InitialDirectory = textBox4.Text.Substring(0, textBox4.Text.LastIndexOf('\\'));
                openFileDialog2.FileName = textBox4.Text.Substring(textBox4.Text.LastIndexOf('\\') + 1);
            }
            else { openFileDialog2.InitialDirectory = "C:\\";openFileDialog2.FileName = " "; }

            if (openFileDialog2.ShowDialog(this) == DialogResult.OK)
            {
                textBox4.Text = openFileDialog2.FileName;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox3.Text;
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

            if (checkBox6.Checked)
            {
                p.StandardInput.WriteLine("cd "+nasmPath);
                p.StandardInput.WriteLine("nasm "+textBox6.Text+ " -f elf -o C:\\Programs\\Cygwin\\home\\Milli\\loadKnl.o ");
            }
            String commandTmp = CygwinPath + "\\bash.exe --login -i -c \"";
            List<String> nameTmp=new List<String>();
            String curName = "";
            //String TMP = CygwinPath + "\\bash.exe --login -i -c \"" + gccPath + " -S /cygdrive/" + treatCpath(textBox4.Text) + " -o /cygdrive/" + treatCpath(textBox3.Text) + "\"";
            for (int i = 0; i < listView2.Items.Count; i++)
            {
                if(listView2.Items[i].Checked)
                {
                    if (listView2.Items[i].SubItems[1].Text == "true")      //IS C++
                        commandTmp += "g++ ";
                    else
                        commandTmp += "gcc ";
                    if (checkBox3.Checked)
                        commandTmp += "-02 ";
                    curName = listView2.Items[i].SubItems[0].Text.Substring(listView2.Items[i].SubItems[0].Text.LastIndexOf('\\') + 1, listView2.Items[i].SubItems[0].Text.LastIndexOf('.') - (listView2.Items[i].SubItems[0].Text.LastIndexOf('\\') + 1));
                    nameTmp.Add(curName);
                    commandTmp+="-ffreestanding -c /cygdrive/" + treatCpath(listView2.Items[i].SubItems[0].Text) + " -o "+curName+ ".o -m32&&";
                }
            }
            if (nameTmp.Count == 0)
            {
                hintOKC();
                return;
            }
            /*
            if (!checkBox8.Checked)
                p.StandardInput.WriteLine(CygwinPath + "\\bash.exe --login -i -c \"" + "gcc -ffreestanding -c /cygdrive/" + treatCpath(textBox4.Text) + " -o KernelEntry.o -m32" + "\"");
            else
                p.StandardInput.WriteLine(CygwinPath + "\\bash.exe --login -i -c \"" + "g++ -ffreestanding -c /cygdrive/" + treatCpath(textBox4.Text) + " -o KernelEntry.o -m32" + "\"");
            

            if (checkBox7.Checked)
            {
                p.StandardInput.WriteLine(CygwinPath + "\\bash.exe --login -i -c \"" + "ld -Ttext 0x1000 -s -o bootsect.tmp KernelEntry.o Start.o -m i386pe");
                p.StandardInput.WriteLine(CygwinPath + "\\bash.exe --login -i -c \"" + "objcopy -O binary bootsect.tmp /cygdrive/" + treatCpath(textBox3.Text) + "\"");
            }
            */
            if (checkBox7.Checked)
            {
                commandTmp += "ld -Ttext 0x1000 -s -o bootsect.tmp loadKnl.o ";
                foreach (String item in nameTmp)
                {
                    commandTmp += item + ".o ";
                }
                commandTmp += "-m i386pe&&";
            }
            if(checkBox9.Checked)
            { 
                commandTmp += "objcopy -O binary bootsect.tmp /cygdrive/" + treatCpath(textBox3.Text) + "\"";
            }
            else
            {
                commandTmp = commandTmp.Substring(0, commandTmp.LastIndexOf('&') - 1);//Remove "&&"
            }
            p.StandardInput.WriteLine(commandTmp);
            p.StandardInput.WriteLine("exit");
            
            String tmp = p.StandardError.ReadLine();
            String temp = "";
            while(tmp!=null)
            {
                if (tmp.Substring(0, 4) != "bash")
                    temp += tmp+"\n\t";
                tmp=p.StandardError.ReadLine();
            }
            if(temp!="")
                MessageBox.Show(temp);
            if (checkBox1.Checked)
                MessageBox.Show("Build Kernel OK!");
            hintOKC();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox2.Text = textBox1.Text.Substring(0, textBox1.Text.LastIndexOf('.'))+".mlf";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox3.Text = textBox4.Text.Substring(0, textBox4.Text.LastIndexOf('.')) + ".asm";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            label7.Text = "";
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            label8.Text = "";
        }

        
        private void button13_Click(object sender, EventArgs e)
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

        private void button14_Click(object sender, EventArgs e)
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

        private void button16_Click(object sender, EventArgs e)
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

        private void button15_Click(object sender, EventArgs e)
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

        private void button10_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count!=0)
                listView1.Items.RemoveAt(listView1.SelectedIndices[0]);

        }

        private void button17_Click(object sender, EventArgs e)
        {
            BuildList();
            hintOKASM();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            if (textBox6.Text != "")
            {
                openFileDialog3.InitialDirectory = textBox6.Text.Substring(0, textBox6.Text.LastIndexOf('\\'));
                openFileDialog3.FileName = textBox6.Text.Substring(textBox6.Text.LastIndexOf('\\') + 1);
            }
            else { openFileDialog3.InitialDirectory = "C:\\"; openFileDialog3.FileName = " "; }

            if (openFileDialog3.ShowDialog(this) == DialogResult.OK)
            {
                textBox6.Text = openFileDialog3.FileName;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
                listView2.Items.Add(new ListViewItem(new String[] { textBox4.Text,checkBox8.Checked?"true":"false"}));
        }

        private void button12_Click(object sender, EventArgs e)
        {
            listView2.Items.RemoveAt(listView2.SelectedIndices[0]);
        }
    }
}
