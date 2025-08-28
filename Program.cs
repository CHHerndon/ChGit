using System;

namespace ChGit
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: chgit <command> [options]");
                return;
            }

            string command = args[0].ToLower();

            switch (command)
            {
                case "init":
                    InitRepo();
                    break;

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        static void InitRepo()
        {
            Console.WriteLine("Init command called (not implemented yet).");
        }
    }
}
