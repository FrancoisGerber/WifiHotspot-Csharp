using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WifiHotspot
{
    public partial class Form1 : Form
    {
        bool connect;
        ContextMenu cm = new ContextMenu();
        public Form1()
        {
            InitializeComponent();


           
            cm.MenuItems.Add("Start WIFI Hotspot", (sender, e) => button1.PerformClick());
            cm.MenuItems.Add("Stop WIFI Hotspot", (sender, e) => button1.PerformClick());
            cm.MenuItems.Add("-");
            cm.MenuItems.Add("Show", (sender, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            });
            cm.MenuItems.Add("Hide", (sender, e) => this.Visible = false);
            cm.MenuItems.Add("-");
            cm.MenuItems.Add("Exit", (sender, e) => Application.Exit());
            notifyIcon1.ContextMenu = cm;

            cm.MenuItems[1].Visible = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ssid = textBox1.Text, key = textBox2.Text;
            if (!connect)
            {
                if (textBox1.Text == null || textBox1.Text == "")
                {
                    MessageBox.Show("SSID cannot be left blank !",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {

                    if (textBox2.Text == null || textBox2.Text == "")
                    {
                        MessageBox.Show("Key value cannot be left blank !",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (key.Length >= 6)
                        {
                            Zedfi_Hotspot(ssid, key, true);
                            textBox1.Enabled = false;
                            textBox2.Enabled = false;
                            button1.Text = "Stop";
                            notifyIcon1.BalloonTipText = "WIFI Hotspot is running...";
                            notifyIcon1.ShowBalloonTip(5000);
                            cm.MenuItems[0].Visible = false;
                            cm.MenuItems[1].Visible = true;
                            connect = true;
                        }
                        else
                        {
                            MessageBox.Show("Key should be more then or Equal to 6 Characters !",
                            "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
            {
                Zedfi_Hotspot(null, null, false);
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Text = "Start";
                notifyIcon1.BalloonTipText = "WIFI Hotspot stopped.";
                notifyIcon1.ShowBalloonTip(5000);
                cm.MenuItems[0].Visible = true;
                cm.MenuItems[1].Visible = false;
                connect = false;
            }
        }


        private void Zedfi_Hotspot(string ssid, string key, bool status)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            Process process = Process.Start(processStartInfo);

            if (process != null)
            {
                if (status)
                {
                    process.StandardInput.WriteLine("netsh wlan set hostednetwork mode=allow ssid = " + ssid + " key = " + key);
                    process.StandardInput.WriteLine("netsh wlan start hosted network");
                    process.StandardInput.Close();
                }
                else
                {
                    process.StandardInput.WriteLine("netsh wlan stop hostednetwork");
                    process.StandardInput.Close();
                }
            }
        }


        public static bool IsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public void RestartElevated()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
            }
            catch
            {

            }

            System.Windows.Forms.Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!IsAdmin())
            {
                RestartElevated();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Zedfi_Hotspot(null, null, false);
            Application.Exit();
        }
    }
}
