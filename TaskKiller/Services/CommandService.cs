using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TaskKiller.Services
{
    public class CommandService
    {
        public string ExecuteCommand(string cmdText)
        {
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.FileName = @"CMD.exe";
            startinfo.Arguments = cmdText;
            Process process = new Process();
            process.StartInfo = startinfo;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string response = "";

            while (!process.StandardOutput.EndOfStream)
            {
                response += "\r\n" + process.StandardOutput.ReadLine();
            }

            process.WaitForExit();

            return response;
        }
    }
}
