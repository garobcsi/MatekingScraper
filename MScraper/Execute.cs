namespace MScraper;
using System.Diagnostics;

public class Execute
{
    public static void exec(string command)
    {
        ProcessStartInfo procStartInfo = new ProcessStartInfo("/bin/bash",$"-c \"{command}\"");
        Process proc = new Process() { StartInfo = procStartInfo, };
        proc.Start();
    }
}