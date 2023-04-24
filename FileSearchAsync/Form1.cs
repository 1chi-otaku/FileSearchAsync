using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FileSearchAsync
{
    public partial class Form1 : Form
    {
        public string pathToFolder = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                pathToFolder = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(FileSearch);
            thread.Start();
            thread.Join();
        }

        private void FileSearch()
        {
            Mutex m = new Mutex();
            m.WaitOne();
            if (pathToFolder == null) { return; }
            listView1.Items.Clear();
            
            try
            {
                string[] files = Directory.GetFileSystemEntries(pathToFolder, "*txt", SearchOption.AllDirectories);
                foreach (var item in files)
                {
                    StreamReader streamReader = new StreamReader(item);
                    string line = streamReader.ReadToEnd();
                    if (line.Contains(textBox2.Text))
                    {
                        Regex REX = new Regex(@textBox2.Text, RegexOptions.IgnoreCase);
                        MatchCollection RTQ = REX.Matches(line);
                        FileInfo fileInfo = new FileInfo(item);
                        ListViewItem list_item = new ListViewItem(fileInfo.Name);
                        list_item.SubItems.Add(fileInfo.Directory.ToString());
                        list_item.SubItems.Add(RTQ.Count.ToString());
                        listView1.Items.Add(list_item);
                    }

                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access to the file was denied, try to change the directory", "UnauthorizedAccessException");
            }
            m.ReleaseMutex();

        }
    }
}

