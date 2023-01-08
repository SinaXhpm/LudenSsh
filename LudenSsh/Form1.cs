using System;
using Renci.SshNet;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace LudenSsh
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public async Task<string> Runcmd(string address,string pass,string command)
        {
            var task = Task.Run(() => {
                ConnectionInfo connnfo = new ConnectionInfo(address, 22, "root",
                  new AuthenticationMethod[]{
                     new PasswordAuthenticationMethod("root",pass.Trim()),
                  }
                );
                try
                {
                    var sshclient = new SshClient(connnfo);
                    sshclient.Connect();
                    var cmd = sshclient.CreateCommand(command);
                    string result = cmd.Execute();
                    sshclient.Disconnect();
                    File.AppendAllText(Directory.GetCurrentDirectory()
                    + "\\results.txt","["+ address + "] : " + result + Environment.NewLine);
                    return address + " Done. \n";
                }
                catch (Exception except)
                {
                    return address + "(" + except.Message + " Error. \n";
                }
            });
            var myOutput = await task;
            return myOutput.ToString();
        }
        private async void button2_ClickAsync(object sender, EventArgs e)
        {
            button2.Enabled = false;
            File.AppendAllText(Directory.GetCurrentDirectory() + "\\results.txt",
            "========= START ========= " + Environment.NewLine);
            string[] servers = textBox1.Text.Split('\r');
            for (int i = 0; i < servers.Count() / 2; i += 2)
            {
                textBox2.Text += "[" +DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString()+":"+ DateTime.Now.Second.ToString() + "] " + await Runcmd(servers[i], servers[i+1],textBox3.Text)+ Environment.NewLine;
            }
            File.AppendAllText(Directory.GetCurrentDirectory() + "\\results.txt",
            "========= FINISH ========= " + Environment.NewLine + "https://github.com/SinaXhpm/LudenSsh" + Environment.NewLine);
            textBox2.Text += "Results Saved - Results.txt" + Environment.NewLine;
            button2.Enabled = true;
        }
    }
}
