using System;
using System.Diagnostics;
using System.Text;

namespace DomainManager
{
    public class CmdExecutor
    {
        private readonly string _workingDir;

        public CmdExecutor(string workingDir)
        {
            _workingDir = workingDir;
        }

        public string ExecutePowerShell(string command)
        {
            var cmd = new Process();
            cmd.StartInfo.FileName = "powershell.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            var result = cmd.StandardOutput.ReadToEnd();
            Console.WriteLine(result);
            return result;
        }

        public string ExecuteCmd(string command)
        {
            var cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.WorkingDirectory = _workingDir;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            var result = cmd.StandardOutput.ReadToEnd();
            Console.WriteLine(result);
            return result;
        }
    }
}
