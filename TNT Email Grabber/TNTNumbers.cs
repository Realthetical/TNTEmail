using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace TNT_Email_Grabber
{
    public partial class TNTGrabber : Form
    {
        public TNTGrabber()
        {
            InitializeComponent();
        }

        TNT_Emails.Properties.Settings settings = new TNT_Emails.Properties.Settings();
        string version = "v0.2";
        string searchFor = "Job Id        : ";

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] readText = File.ReadAllLines(files[0]);
                

                StringBuilder sb = new StringBuilder();

                foreach (string s in readText)
                {
                    if (!s.Contains(searchFor))
                        continue;
                    sb.AppendLine(s.Replace(searchFor, ""));
                }

                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var path = Path.Combine(appDataPath, @"TNTOutput\");
                string fileName = "output.txt";

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                try
                {
                    StreamWriter writer = new StreamWriter(path + fileName);

                    writer.Write(sb);
                    writer.Dispose();
                    writer.Close();
                    if(settings.notepadLocation == "")
                    {
                        OpenWithDefaultProgram(path + fileName);
                    }
                    else
                    {
                        Process.Start(settings.notepadLocation, path + fileName);
                    }
                    
                }
                catch
                {
                    MessageBox.Show(string.Format("Error Creating {0}, the file is in use.", fileName), "Error Creating File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                

            }
        }

        public static void OpenWithDefaultProgram(string path)
        {
            Process fileopener = new Process();

            fileopener.StartInfo.FileName = path;
            fileopener.Start();
        }

        private void TNTGrabber_Load(object sender, EventArgs e)
        {
            settings.firstOpen = true;
            this.Text = "TNT Numbers " + version;
            

            if (settings.firstOpen)
            {
                DialogResult dresult = MessageBox.Show("Would you like to set an alternate program to open text files?", "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dresult == DialogResult.Yes)
                {
                    var filePath = string.Empty;

                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.InitialDirectory = "c:\\";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            filePath = openFileDialog.FileName;
                        }
                    }

                    if(!(filePath == ""))
                    {
                        //MessageBox.Show(filePath, "File Content at path: " + filePath, MessageBoxButtons.OK);
                        settings.notepadLocation = filePath;
                    }
                    else
                    {
                        MessageBox.Show("You selected nothing, default application will be used.", "No Application Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                }
                else
                {

                }

                settings.firstOpen = false;
                
            }
        }

        private void TNTGrabber_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.Save();
        }
    }
}
