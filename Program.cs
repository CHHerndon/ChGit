using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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

                case "add":
                    if (args.Length < 2)
                    {
                        //Missing name of file
                        Console.WriteLine("Usage: chgit add <file>");
                    }
                    else
                    {
                        //All good pass file name to method
                        AddFile(args[1]);
                    }
                    break;

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        static void InitRepo()
        {
            string repoPath = Path.Combine(Directory.GetCurrentDirectory(), ".chgit");
            string objectsPath = Path.Combine(repoPath, "objects");
            string commitsPath = Path.Combine(repoPath, "commits");
            string headPath = Path.Combine(repoPath, "HEAD");

            if (Directory.Exists(repoPath))
            {
                Console.WriteLine("Repository already initialized.");
                return;
            }

            //Create directories
            Directory.CreateDirectory(objectsPath);
            Directory.CreateDirectory(commitsPath);

            //Create HEAD file
            File.WriteAllText(headPath, string.Empty);

            Console.WriteLine("Initialized empty ChGit repository in " + repoPath);
        }

        static void AddFile(string fileName)
        {
            if(!File.Exists(fileName))
            {
                Console.WriteLine($"File '{fileName}' does not exist.");
                return;
            }

            //Reading File
            byte[] fileContent = File.ReadAllBytes(fileName);

            //HASHING
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(fileContent);
                string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                //makes sure init has been done first
                string objectsPath = Path.Combine(".chgit", "objects");

                if (!Directory.Exists(objectsPath))
                {
                    Console.WriteLine("Repository not initialized. Please run 'chgit init' first.");
                    return;
                }

                //saving file
                string objectsHashPath = Path.Combine(objectsPath, hash);
                if (!File.Exists(objectsHashPath))
                {
                    File.WriteAllBytes(objectsHashPath, fileContent);
                    Console.WriteLine($"Added '{fileName}' as object {hash}");
                }
                else
                {
                    Console.WriteLine($"Object {hash} already exists.");
                }
            }
        }
    }
}
