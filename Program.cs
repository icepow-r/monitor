using System;
using System.Diagnostics;
using System.Threading;

namespace monitor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!(args.Length == 3))
            {
                ShowHelp();
                return;
            }
            if (!(int.TryParse(args[1], out int lifeTime) && lifeTime >= 0) ||
                !(int.TryParse(args[2], out int checkFreq) && checkFreq >= 1))
            {
                ShowHelp();
                return;
            }
            TimerCallback tm = new TimerCallback(MonitorTargetProcess);
            Timer timer = new Timer(tm, args, 0, checkFreq * 60 * 1000);

            Console.Write("\rq to exit: ");
            var quit = Console.ReadKey();
            while (quit.Key != ConsoleKey.Q)
            {
                Console.Write("\rq to exit: ");
                quit = Console.ReadKey();
            }
        }
        static void MonitorTargetProcess(object arg)
        {
            var args = (string[])arg;
            var targetProcessList = Process.GetProcessesByName(args[0]);
            foreach (var item in targetProcessList)
            {
                if ((DateTime.Now - item.StartTime).Minutes >= Convert.ToInt32(args[1]))
                {
                    item.Kill();
                    Console.WriteLine($"\rKilled process: {item.ProcessName}, ID: {item.Id}");
                    Console.Write("q to exit: ");
                }
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("usage: monitor.exe <PROCESS_NAME> <PROCESS_LIFETIME> <CHECK_FREQUENCY>");
            Console.WriteLine("PROCESS_LIFETIME is in minutes and must be greater than or equal to 0");
            Console.WriteLine("CHECK_FREQUENCY is in minutes and must be greater than or equal to 1");
        }
    }
}
