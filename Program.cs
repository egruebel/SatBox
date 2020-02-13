using System;
using System.Reflection;
using System.Threading;

namespace SatBox
{
    class Program
    {
        static void Main(string[] args)
        {
            //parse comand line args
            if (args.Length < 2)
            {
                //show the user an error
                Console.WriteLine("This application requires a minimum of two command line arguments...");
                return;
            }

            //process command line args
            try
            {
                int interval = int.Parse(args[0]);
                var system = new object();
                switch (args[1])
                {
                    case "fbb500":
                        system = new FBB500Model(args[2]);
                        BeginAcq((ISatSystem)system, interval);
                        break;
                    case "verizon":
                        system = new VerizonSystemModel(args[2], args[3]);
                        BeginAcq((ISatSystem)system, interval);
                        break;
                    case "kvhv7ip":
                        system = new TracphoneModel(args[2]);
                        BeginAcq((ISatSystem)system, interval);
                        break;
                    case "hiseasnet":
                        system = new HiSeasNetSystemModel(args[2], args[3]);
                        BeginAcq((ISatSystem)system, interval);
                        break;
                    default:
                        Console.WriteLine("Unrecognized system " + args[1]);
                        return;
                }
            }
            catch
            {
                Console.WriteLine("Unable to process the supplied parameters");
                return;
            }
        }

        public static void BeginAcq(ISatSystem system, int interval)
        {

            for (; ; )
            {
                if (system.QuerySystemStatus())
                {
                    //SatMonRepository.AddToDb(system);
                    PrintSystemProps(system);
                }
                else
                {
                    //wait a few seconds and try one more time
                    Thread.Sleep(1000 * 2);
                    if (system.QuerySystemStatus())
                    {
                        //SatMonRepository.AddToDb(system);
                        PrintSystemProps(system);
                    }
                }
                Thread.Sleep(1000 * 4 * 60); //four minute delay
            }
        }

        public static void PrintSystemProps(ISatSystem system)
        {
            Console.Clear();
            const string underline = "-----------------------------------";
            Console.WriteLine(underline);
            Console.WriteLine(system.GetType().Name);
            Console.WriteLine(underline);
            //Console.WriteLine(string.Format("Lat: {0}", system.Lat));
            foreach (PropertyInfo prop in system.GetType().GetProperties())
            {
                Console.WriteLine(prop.Name + ": " + prop.GetValue(system, null));
            }
        }

        public static void LogError(string systemName, string systemLocalName, string title, string message)
        {
            Console.Error.WriteLine(($"{ systemName},{ systemLocalName},{ title},{ message}"));
        }
    }
}
