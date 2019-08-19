using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Speech;
using System.Speech.Synthesis;

namespace Budilnik
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();           
            
        }
        //int hours = Convert.ToInt32(textBox1.Text.ToString());

        //    if (hours>25 && hours<0 || Regex.IsMatch(textBox1.Text, "[^0-9]")//Regex.IsMatch(textBox1.Text, "[^0-9]")
        //    {
        //        MessageBox.Show("неверный ввод");
        //        textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
        //        textBox1.SelectionStart = textBox1.TextLength;
        //    }

        int hour, minute;
        string path1 = @"C:\VS\С#\Budilnik\FileNames.txt";
        FileStream file1;
        
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(@"C:\VS\С#\Budilnik\"))
                {
                    FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                    DialogResult result = new DialogResult();
                    result = folderBrowser.ShowDialog();
                    path1 = $@"{folderBrowser.SelectedPath}\FileNames.txt";
                    Directory.CreateDirectory(folderBrowser.SelectedPath);
                    File.Create (path1).Close();                  
                }
                else {
                    //FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                    //DialogResult result = new DialogResult();
                    //result = folderBrowser.ShowDialog();
                    //path1 = folderBrowser.SelectedPath;
                }
                    
            }
            catch (Exception){
                throw;
            }

            //notifyIcon1.Icon = SystemIcons.WinLogo;//Shield - значок щита
            //SystemIcons - это стандартные значки Windows
            notifyIcon1.Text = "Budilnik";
            notifyIcon1.Visible = false;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;      

            new Thread(async () =>
            {
                while (true)
                {                  
                    string timeNow = DateTime.Now.ToShortTimeString();
                   label4.Text = timeNow;                  
                    await Task.Delay(10000);
                }
            }).Start();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            label3.Text = "";
            //очистка массива под напоминание
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    data[i, j] = null;
                }
            }
            count = 0;
           File.Delete(path1);
         
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            File.Delete(path1);           
        }

        string[,] data = new string[5,3];
        int count = 0;
        SpeechSynthesizer ss = new SpeechSynthesizer();

        //private void Form1_Deactivate(object sender, EventArgs e)
        //{
        //    this.Hide();
        //    notifyIcon1.Visible = true;
        //}

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Show();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Hide();
            notifyIcon1.Visible = true;
        }

        private void текущие_событияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (label3.Text=="")
            {
                MessageBox.Show("Нет запланированных задач","Message");              
            }
            else
            {
                MessageBox.Show($"{label3.Text}", "Напоминание");
            }            
        }

        private void текущиеСобытияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.Delete(path1);          
            this.Close();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(comboBox1.Text, "[^0-9-.]"))
            {
                MessageBox.Show("неверный ввод");
                comboBox1.Text = comboBox1.Text.Remove(comboBox1.Text.Length - 1);
                
                //comboBox1.SelectionStart = comboBox1.TextLength;
            }
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(comboBox2.Text, "[^0-9-.]"))
            {
                MessageBox.Show("неверный ввод");
                comboBox2.Text = comboBox2.Text.Remove(comboBox2.Text.Length - 1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text!=""&&comboBox2.Text!="")
            {
                if (count < 5)
                {
                    //List<string> path = new List<string>();
                    //List<List<string>> data = new List<List<string>>();
                  

                    hour = Convert.ToInt32(comboBox1.Text);
                    minute = Convert.ToInt32(comboBox2.Text);
                    // path.Add($" {hour}:{minute} - {textBox1.Text}");

                    //File.WriteAllText(path1, $"{hour}:{minute} - {textBox1.Text}");
                    //label3.Text += string.Join("\n", path);
                    using (file1 = new FileStream(path1, FileMode.Append))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(file1))
                        {
                            streamWriter.WriteLine($"{hour}:{minute} - {textBox1.Text}");
                        }
                    }

                    data[count, 0] = comboBox1.Text;
                    data[count, 1] = comboBox2.Text;
                    data[count, 2] = textBox1.Text;

                    label3.Text = File.ReadAllText(path1);
                    textBox1.Text = "";
                    count++;

                    new Thread(async () =>
                    {
                        while (true)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (Convert.ToInt32(data[i, 0]) == DateTime.Now.Hour &&
                                Convert.ToInt32(data[i, 1]) == DateTime.Now.Minute)
                                {
                                    //MessageBox.Show($"{data[i, 2]}");
                                    ss.Volume = 100;// от 0 до 100
                                    ss.Rate = 0;//от -10 до 10
                                    ss.SpeakAsync($"{data[i, 2]}");
                                }
                            }
                            await Task.Delay(10000);
                        }
                    }).Start();
                }
                else
                {
                    MessageBox.Show("Не более 5 напоминаний!", "Message");
                }
            }

            else
            {
                MessageBox.Show("Укажите время","Message");
            }
            
        }     
    }
}
