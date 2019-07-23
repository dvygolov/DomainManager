using System;
using System.Diagnostics;
using System.Management.Automation;
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
            using (var myPowerShellInstance = PowerShell.Create())
            {
                // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                myPowerShellInstance.AddScript(command);

                // invoke execution on the pipeline (collecting output)
                var PSOutput = myPowerShellInstance.Invoke();

                var res = new StringBuilder();
                // loop through each output object item
                foreach (var outputItem in PSOutput)
                {
                    if (outputItem != null)
                    {
                        Console.WriteLine(outputItem.ToString());
                        res.AppendLine(outputItem.ToString());
                    }
                }
                return res.ToString();
            }
        }

        public string ExecuteCmd(string command)
        {
            Process cmd = new Process();
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
