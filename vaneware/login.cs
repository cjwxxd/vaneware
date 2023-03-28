using System;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vaneware
{
    public partial class login : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
        Thread th;
        public login()
        {
            InitializeComponent();
        }
        //hwid grabber
        public string GetHDDSerial()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                if (wmi_HD["SerialNumber"] != null)
                    return wmi_HD["SerialNumber"].ToString();
            }

            return string.Empty;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        
        private async void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string hwid = new System.Net.WebClient() { Proxy = null }.DownloadString("https://raw.githubusercontent.com/cjwxxd/vaneware/master/ids");
                if (hwid.Contains(GetHDDSerial()))
                {
                    await Task.Delay(1);
                    this.Hide();
                    Form1 f = new Form1();
                    f.Show();
                    
                }
                else
                {
                    MessageBox.Show("Invalid ID");
                }
            }
            catch
            {

            }
        }
        private void opennewform(object obj)
        {
            Application.Run(new Form1());
        }
        private async void login_Load(object sender, EventArgs e)
        {
            
            try
            {
                string hwid = new System.Net.WebClient() { Proxy = null }.DownloadString("https://raw.githubusercontent.com/cjwxxd/vaneware/master/ids");
                if (hwid.Contains(GetHDDSerial()))
                {
                    this.th = new Thread(new ParameterizedThreadStart(this.opennewform));
                    this.th.SetApartmentState(ApartmentState.STA);
                    this.th.Start();
                    //Thread dThread;
                    await Task.Delay(1);
                    base.Close();
                }
                else
                {
                    
                }
            }
            catch
            {

            }



            
            textBox1.Text = GetHDDSerial();

        }

        private void getIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(GetHDDSerial());
            Clipboard.SetText(GetHDDSerial());
            textBox1.Text = GetHDDSerial();
        }
    }
}
