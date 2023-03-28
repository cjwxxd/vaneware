using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using NReco.VideoConverter;
using VideoLibrary;
using MediaToolkit;
using MediaToolkit.Model;
using System.IO.Compression;


namespace vaneware
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
            var skinManager = MaterialSkinManager.Instance;
            skinManager.Theme = MaterialSkinManager.Themes.DARK;
            skinManager.ColorScheme = new ColorScheme(Primary.Red300, Primary.Red300, Primary.Red300, Accent.Red100, TextShade.WHITE);
        }
        private void downup()
        {
            WebClient web = new WebClient();
            web.DownloadFile("https://github.com/cjwxxd/vaneware/raw/master/updater.exe", Environment.CurrentDirectory + @"\updater.exe");
        }
        private void autoupdate()
        {
            string version = new System.Net.WebClient() { Proxy = null }.DownloadString("https://raw.githubusercontent.com/cjwxxd/vaneware/master/version");
            if (version.Contains("1.0.1"))
            {

            }
            else
            {
                if (File.Exists(Environment.CurrentDirectory + @"\updater.exe"))
                {
                    File.Delete(Environment.CurrentDirectory + @"\updater.exe");
                    downup();
                    Process.Start(Environment.CurrentDirectory + @"\updater.exe");
                    Application.Exit();
                }
                else
                {
                    downup();
                    Process.Start(Environment.CurrentDirectory + @"\updater.exe");
                    Application.Exit();
                }
            }
        }

        System.Windows.Forms.Timer t1 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer t2 = new System.Windows.Forms.Timer();
        private void Form1_Load(object sender, EventArgs e)
        {
            Opacity = 0;      //first the opacity is 0

            t1.Interval = 10;  //we'll increase the opacity every 10ms
            t1.Tick += new EventHandler(fadeIn);  //this calls the function that changes opacity 
            t1.Start();
            splitContainer1.SplitterDistance = 20;
            try
            {
                //autoupdate();
            }
            catch
            {

            }

            string dirpath;
            string dir2 = Environment.CurrentDirectory + "\\dir.txt";
            try
            {
                string text;
                var fileStream = new FileStream(dir2, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    text = streamReader.ReadToEnd();
                    dirpath = text;
                }
            }
            catch
            {
                dirpath = Environment.CurrentDirectory;
            }
            treeView1.Nodes.Clear();
            if (dirpath != "" && Directory.Exists(dirpath))
                LoadDirectory(dirpath);
            else
                MessageBox.Show("Select Directory!!");


            this.TopMost = true;
            Task.Delay(1);
            this.TopMost = false;

        }
        void fadeIn(object sender, EventArgs e)
        {
            if (Opacity >= 1)
                t1.Stop();   //this stops the timer if the form is completely displayed
            else
                Opacity += 0.10;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t2.Interval = 10;  //we'll increase the opacity every 10ms
            t2.Tick += new EventHandler(fadeOut);  //this calls the function that changes opacity 
            t2.Start();
        }
        void fadeOut(object sender, EventArgs e)
        {
            if (Opacity == 0)
                Application.Exit();  //this stops the timer if the form is completely displayed
            else
                Opacity -= 0.10;
        }

        public void LoadDirectory(string Dir)
        {
            DirectoryInfo di = new DirectoryInfo(Dir);

            TreeNode tds = treeView1.Nodes.Add(di.Name);
            tds.Tag = di.FullName;
            tds.StateImageIndex = 0;
            LoadFiles(Dir, tds);
            LoadSubDirectories(Dir, tds);
        }
        private void LoadSubDirectories(string dir, TreeNode td)
        {
            // Get all subdirectories  
            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            // Loop through them to see if they have any other subdirectories  
            foreach (string subdirectory in subdirectoryEntries)
            {

                DirectoryInfo di = new DirectoryInfo(subdirectory);
                TreeNode tds = td.Nodes.Add(di.Name);
                tds.StateImageIndex = 0;
                tds.Tag = di.FullName;
                LoadFiles(subdirectory, tds);
                LoadSubDirectories(subdirectory, tds);


            }
        }
        private void LoadFiles(string dir, TreeNode td)
        {
            string[] Files = Directory.GetFiles(dir, "*.*");

            // Loop through them to see files  
            foreach (string file in Files)
            {
                FileInfo fi = new FileInfo(file);
                TreeNode tds = td.Nodes.Add(fi.Name);
                tds.Tag = fi.FullName;
                tds.StateImageIndex = 1;


            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dirpath;
            string dir = Environment.CurrentDirectory + "\\dir.txt";
            try
            {
                string text;
                var fileStream = new FileStream(dir, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    text = streamReader.ReadToEnd();
                    dirpath = text;
                }
            }
            catch
            {
                dirpath = Environment.CurrentDirectory;
            }

            this.saveFileDialog1.Title = "Save";
            this.saveFileDialog1.Filter = "Text (*.txt)|*.txt";
            this.saveFileDialog1.InitialDirectory = dirpath;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, presetbox.Text);
                treeView1.Nodes.Clear();

                if (dirpath != "" && Directory.Exists(dirpath))
                    LoadDirectory(dirpath);
                else
                    MessageBox.Show("Select Directory!!");
                
            }
        }

        private void changePresetFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dirpath = Environment.CurrentDirectory;
            folderBrowserDialog1.SelectedPath = dirpath;
            DialogResult drResult = folderBrowserDialog1.ShowDialog();
            if (drResult == System.Windows.Forms.DialogResult.OK)
                dirpath = folderBrowserDialog1.SelectedPath;
            string dir = Environment.CurrentDirectory + "\\dir.txt";
            TextWriter txt = new StreamWriter(dir);
            txt.Write(dirpath);
            txt.Close();
            treeView1.Nodes.Clear();

            if (dirpath != "" && Directory.Exists(dirpath))
                LoadDirectory(dirpath);
            else
                MessageBox.Show("Select Directory!!");
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                presetbox.SelectAll();
                presetbox.Copy();
                Clipboard.SetText(presetbox.Text);
            }
            catch{

            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                presetbox.Paste();
            }
            catch
            {

            }
            
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                presetbox.Clear();
            }
            catch
            {

            }
            
        }

        private void cm3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://panzoid.com/preview/clipmaker");
        }

        private void cm2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://panzoid.com/tools/clipmaker");
        }

        private void forumsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://panzoid.com/discussions");
        }

        private void checkForUpadtesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string version = new System.Net.WebClient() { Proxy = null }.DownloadString("https://raw.githubusercontent.com/cjwxxd/vaneware/master/version");
            if (version.Contains("1.0.0"))
            {
                MessageBox.Show("no updates found");
            }
            else
            {
                if (MessageBox.Show("An update has been found.\n Would you like to download it now?", "Update!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    autoupdate();
                }
                else
                {

                }
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                TreeNode theNode = treeView1.GetNodeAt(e.X, e.Y);

                presetbox.Text = File.ReadAllText(theNode.Tag.ToString());
            }
            catch
            {

            }
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                presetbox.Copy();
                Clipboard.SetText(presetbox.Text);
            }
            catch
            {

            }
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                presetbox.Paste();
            }
            catch
            {

            }
        }
        private bool mouseDown;
        private Point lastLocation;
        private void MenuStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void MenuStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void MenuStrip1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void TreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void TreeView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void TreeView1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        /*
        ******************************************************
        * *****************************************************
        ******************************************************
        * *****************************************************
        * *****************************************************
        * *****************************************************
        * *****************************************************
        * *****************************************************
        * *****************************************************
        * *****************************************************
        * *****************************************************
    */

        string InputFile;
        string OutputFile;

        private void pickfilebutton_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "pz files (*.pz)|*.pz|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }
            fileout.Text = filePath;
            string rem = filePath.Replace(".pz" , "");
            filename.Text = rem;
        }

        private async void dumpbutton_Click(object sender, EventArgs e)
        {
            // Making pz file gzip
            string source = fileout.Text;
            string destination = filename.Text + ".gzip";
            try
            {
                File.Move(source, destination);
            }
            catch
            {
                MessageBox.Show("Unable to rename file!", Application.ProductName);
            }
            await Task.Delay(10);
            // Doing first extract
            string filename2 = filename.Text;
            string outputFilename = filename.Text + ".gzip";
            using (FileStream inputStream = new FileStream(outputFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (FileStream outputStream = new FileStream(filename2, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    using (GZipStream gzip = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        gzip.CopyTo(outputStream);
                    }
                }
            }
            // Changing level 2 to gzip
            await Task.Delay(10);
            string source2 = filename.Text;
            string destination2 = filename.Text + "fin" + ".gzip";
            
            try
            {
                File.Move(source2, destination2);
            }
            catch
            {
                MessageBox.Show("Unable to rename file!", Application.ProductName);
            }
            //Extracting level 2

            // hahah everything i tried didnt work so heres a comment so i can see where to code later
        }
        
        private void texturebutton_Click(object sender, EventArgs e)
        {

        }
        private async void bunifuThinButton21_Click(object sender, EventArgs e)
        {

            string track = Clipboard.GetText();
            track = track.Replace(prop.Text + ".X", prop.Text + "." + axis.Text);
            track = track.Replace(prop.Text + ".Y", prop.Text + "." + axis.Text);
            track = track.Replace(prop.Text + ".Z", prop.Text + "." + axis.Text);
            Clipboard.SetText(track);
            groupBox1.Text = "copied";
            await Task.Delay(1000);
            groupBox1.Text = "track changer";
        }

        private async void bunifuThinButton22_Click(object sender, EventArgs e)
        {
            if (openFileVideo.ShowDialog() == DialogResult.OK)
            {
                InputFile = openFileVideo.FileName;
                OutputFile = InputFile.Substring(0, InputFile.IndexOf("."));
                this.Text = InputFile;

                MessageBox.Show("Converting (this might take a few seconds)");
                var ConvertVideo = new NReco.VideoConverter.FFMpegConverter();
                Thread dThread;
                dThread = new Thread(() => ConvertVideo.ConvertMedia(InputFile, OutputFile + ".mp4", Format.mp4));
                dThread.Start();
                //ConvertVideo.ConvertMedia(InputFile, OutputFile + ".mp4", Format.mp4);
                await Task.Delay(5000);
                MessageBox.Show("done");
            }
        }

        private void bunifuThinButton24_Click(object sender, EventArgs e)
        {
            string dirpath = Environment.CurrentDirectory;
            folderBrowserDialog1.SelectedPath = dirpath;
            DialogResult drResult = folderBrowserDialog1.ShowDialog();
            if (drResult == System.Windows.Forms.DialogResult.OK)
                dirpath = folderBrowserDialog1.SelectedPath;
            bunifuMaterialTextbox1.Text = dirpath + @"\";
        }

        private void ytconv()
        {
            if (yttype == true)
            {
                try
                {
                    var source = bunifuMaterialTextbox1.Text;
                    var youtube = YouTube.Default;
                    var vid = youtube.GetVideo(bunifuMaterialTextbox2.Text);
                    File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

                    var inputFile = new MediaFile { Filename = source + vid.FullName };
                    var outputFile = new MediaFile { Filename = $"{source + vid.FullName}.mp3" };

                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(inputFile);

                        engine.Convert(inputFile, outputFile);
                    }
                    File.Delete(source + vid.FullName);
                    MessageBox.Show("Downloaded to: " + source);
                }
                catch
                {
                    MessageBox.Show("Something went wrong. Youtube video could be copyright claimed.");
                }
            }
            if (yttype == false)
            {
                try
                {
                    var source = bunifuMaterialTextbox1.Text;
                    var youtube = YouTube.Default;
                    var vid = youtube.GetVideo(bunifuMaterialTextbox2.Text);
                    File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

                    var inputFile = new MediaFile { Filename = source + vid.FullName };
                    var outputFile = new MediaFile { Filename = $"{source + vid.FullName}.mp3" };

                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(inputFile);

                        engine.Convert(inputFile, outputFile);
                    }
                    File.Delete($"{source + vid.FullName}.mp3");
                    MessageBox.Show("Downloaded to: " + source);
                }
                catch
                {
                    MessageBox.Show("Something went wrong. Youtube video could be copyright claimed.");
                }
            }
        }

        private void bunifuThinButton23_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Started. This might take a while depending on the length of the video.");
            Thread dThread;
            dThread = new Thread(() => ytconv());
            dThread.Start();

        }
        public static bool yttype = true;
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "mp3")
            {
                yttype = true;
            }
            else
            {
                yttype = false;
            }

        }

        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        /*private void BunifuThinButton25_Click(object sender, EventArgs e)
        {
            page1.Show();
            page2.Hide();
        }

        private void BunifuThinButton26_Click(object sender, EventArgs e)
        {
            page2.Show();
            page1.Hide();
        }*/
        private void replaceshader()
        {
            string shader = shaderbox.Text;
            string time = "uniform float time;";
            string mouse = "uniform vec2 mouse;";
            string highpf = "precision highp float;";
            string res = "uniform vec2 resolution;";
            string highpi = "precision highp int;";
            string vUv = "varying vec2 vUv;";
            if (checkedListBox1.CheckedItems.Contains(time))
            {
                if (shader.Contains(time))
                {
                    //MessageBox.Show("Error: " + time + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    shaderbox.Text = time + "\n\r" + shader;
                    shader = shaderbox.Text;
                }
            }
            if (checkedListBox1.CheckedItems.Contains(mouse))
            {
                if (shader.Contains(mouse))
                {
                    //MessageBox.Show("Error: " + mouse + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    shaderbox.Text = mouse + "\n\r" + shader;
                    shader = shaderbox.Text;
                }
            }

            if (checkedListBox1.CheckedItems.Contains(res))
            {
                if (shader.Contains(res))
                {
                    //MessageBox.Show("Error: " + res + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    shaderbox.Text = res + "\n\r" + shader;
                    shader = shaderbox.Text;
                }
            }

            if (checkedListBox1.CheckedItems.Contains(vUv))
            {
                if (shader.Contains(vUv))
                {
                    //MessageBox.Show("Error: " + vUv + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    shaderbox.Text = vUv + "\n\r" + shader;
                    shader = shaderbox.Text;
                }
            }
            if (checkedListBox1.CheckedItems.Contains(highpi))
            {
                if (shader.Contains(highpi))
                {
                    shader = shaderbox.Text;
                    string test = shader.Replace(highpi, "");
                    shaderbox.Text = test;
                    shader = shaderbox.Text;
                    shaderbox.Text = highpi + "\n\r" + shader;
                    //MessageBox.Show("Error: " + highpi + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    shaderbox.Text = highpi + "\n\r" + shader;
                    shader = shaderbox.Text;
                }
            }
            if (checkedListBox1.CheckedItems.Contains(highpf))
            {
                if (shader.Contains(highpf))
                {

                    shader = shaderbox.Text;
                    string test = shader.Replace(highpf, "");
                    shaderbox.Text = test;
                    shader = shaderbox.Text;
                    shaderbox.Text = highpf + "\n\r" + shader;
                    //MessageBox.Show("Error: " + highpf + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    shaderbox.Text = highpf + "\n\r" + shader;
                    shader = shaderbox.Text;
                }
            }
            string vUv2 = "vUv * resolution";
            string fragcoord = "gl_FragCoord";
            //string vUv = "varying vec2 vUv;";
            if (checkedListBox1.CheckedItems.Contains(fragcoord))
            {
                if (shader.Contains("varying vec2 vUv;"))
                {
                    string test = shader.Replace(fragcoord, vUv2);
                    shaderbox.Text = test;
                }
                else
                {
                    shaderbox.Text = vUv + "\n\r" + shader;
                    shader = shaderbox.Text;
                    string test = shader.Replace(fragcoord, vUv2);
                    shaderbox.Text = test;
                }
            }

        }

        private void replaceallshader()
        {
            string shader = shaderbox.Text;
            string time = "uniform float time;";
            string mouse = "uniform vec2 mouse;";
            string highpf = "precision highp float;";
            string res = "uniform vec2 resolution;";
            string highpi = "precision highp int;";
            string vUv = "varying vec2 vUv;";

            if (shader.Contains(time))
            {
                //MessageBox.Show("Error: " + time + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                shaderbox.Text = time + "\n\r" + shader;
                shader = shaderbox.Text;
            }

            if (shader.Contains(mouse))
            {
                //MessageBox.Show("Error: " + mouse + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                shaderbox.Text = mouse + "\n\r" + shader;
                shader = shaderbox.Text;
            }

            if (shader.Contains(res))
            {
                //MessageBox.Show("Error: " + res + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                shaderbox.Text = res + "\n\r" + shader;
                shader = shaderbox.Text;
            }

            if (shader.Contains(vUv))
            {
                //MessageBox.Show("Error: " + vUv + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                shaderbox.Text = vUv + "\n\r" + shader;
                shader = shaderbox.Text;
            }

            if (shader.Contains(highpi))
            {
                shader = shaderbox.Text;
                string test = shader.Replace(highpi, "");
                shaderbox.Text = test;
                shader = shaderbox.Text;
                shaderbox.Text = highpi + "\n\r" + shader;
                shader = shaderbox.Text;
                //MessageBox.Show("Error: " + highpi + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                shaderbox.Text = highpi + "\n\r" + shader;
                shader = shaderbox.Text;
            }


            if (shader.Contains(highpf))
            {

                shader = shaderbox.Text;
                string test = shader.Replace(highpf, "");
                shaderbox.Text = test;
                shader = shaderbox.Text;
                shaderbox.Text = highpf + "\n\r" + shader;
                shader = shaderbox.Text;
                //MessageBox.Show("Error: " + highpf + " is already defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                shaderbox.Text = highpf + "\n\r" + shader;
                shader = shaderbox.Text;
            }

            string vUv2 = "vUv * resolution";
            string fragcoord = "gl_FragCoord";
            //string vUv = "varying vec2 vUv;";

            if (shader.Contains("varying vec2 vUv;"))
            {
                string test = shader.Replace(fragcoord, vUv2);
                shaderbox.Text = test;
            }
            else
            {
                shaderbox.Text = vUv + "\n\r" + shader;
                shader = shaderbox.Text;
                string test = shader.Replace(fragcoord, vUv2);
                shaderbox.Text = test;
            }

        }


        private void BunifuThinButton28_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Contains("Fix All"))
            {
                replaceallshader();
                string finbox = watermark + shaderbox.Text;
                shaderbox.Text = finbox;
            }
            else
            {
                replaceshader();
                string finbox = watermark + shaderbox.Text;
                shaderbox.Text = finbox;
            }
        }
        static string watermark = "//---------------------------------------------//\n//*********************************************//\n//============= Fixed By Vaneware =============//\n//****************** cjwxxd *******************//\n//---------------------------------------------//\n";
        private void shadertoyfix()
        {
            string test = ("uniform vec2 iResolution;\n\runiform vec2 iMouse;\n\runiform vec2 resolution;\n\r#define iTime time\n\r#define iResolution resolution\n\r#define iFrame frame\n\runiform sampler2D tDiffuse;\n\r");
            string test2 = ("void main(void)\n\r{\n\r    mainImage(gl_FragColor, vUv * resolution.xy);\n\r}");
            string shader = shaderbox.Text + "\n\r";
            shaderbox.Text = test + shader + test2;
            if (shaderbox.Text.Contains("float time;"))
            {
                string rep = shaderbox.Text.Replace("float time;", "");
                shaderbox.Text = rep;
            }
            if (shaderbox.Text.Contains("time = iTime;"))
            {
                string rep = shaderbox.Text.Replace("time = iTime;", "");
                shaderbox.Text = rep;
            }
            if (shaderbox.Text.Contains("texture"))
            {
                string rep = shaderbox.Text.Replace("texture", "texture2D");
                shaderbox.Text = rep;
            }
            if (shaderbox.Text.Contains("iChannel0"))
            {
                string rep = shaderbox.Text.Replace("iChannel0", "tDiffuse");
                shaderbox.Text = rep;
            }
            replaceallshader();
        }

        private void BunifuThinButton27_Click(object sender, EventArgs e)
        {
            if (checkedListBox2.CheckedItems.Contains("Fix All"))
            {
                shadertoyfix();
                string finbox = watermark + shaderbox.Text;
                shaderbox.Text = finbox;
            }
            else
            {

            }

        }

        private void ColourToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // get selected node text
            tree_search.Text = treeView1.SelectedNode.Text;

            // get selected node name
           // tree_search.Text = treeView1.SelectedNode.Name;
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            // get selected node text
            try
            {
                string a = treeView1.SelectedNode.Text;
                tree_search.Text = a;
            }
            catch
            {
               // MessageBox.Show("cunt didnt work");
            }
           
            // get selected node name
           //  tree_search.Text = treeView1.SelectedNode.Name;
        }

        private void tree_search_Enter(object sender, EventArgs e)
        {
            try
            {
                tree_search.Text = treeView1.SelectedNode.Name;
                presetbox.Text = File.ReadAllText(tree_search.Text);
            }
            catch
            {

            }
        }

        private void tree_search_KeyDown(object sender, KeyEventArgs e)
        {
            /*try
            {
                tree_search.Text = treeView1.SelectedNode.Name;
                
            }
            catch
            {

            }*/
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = Environment.CurrentDirectory + "\\dir.txt";
            presetbox.Text = File.ReadAllText(dir + "\\" +tree_search.Text); // file path not spec
        }
    }
}
