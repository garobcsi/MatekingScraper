namespace MScraper;
using System.Diagnostics;

public class Execute
{
    public static async Task exec(string command)
    {
        ProcessStartInfo procStartInfo = new ProcessStartInfo("/bin/bash",$"-c \"{command}\"");
        Process proc = new Process() { StartInfo = procStartInfo, };
        proc.Start();
    }
}