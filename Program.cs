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

                case "commit":
                    if (args.Length < 3 || args[1] != "-m")
                    {
                        //Incorrect format
                        Console.WriteLine("Usage: chgit commit -m '"'Commit Description'"'");
                    }
                    else
                    {
                        //All good pass message to commit
                        string message = string.Join(" ", args.Skip(2));
                        Commit(message);
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

                //writing to index file
                string indexPath = Path.Combine(".chgit", "index");

                if (!File.Exists(indexPath))
                {
                    File.WriteAllText(indexPath, fileName + "\t" + hash);
                }
                else
                {
                    string[] lines = File.ReadAllLines(indexPath);

                    List<string> indexUpdate = new List<string>();

                    bool noMatch = true;

                    foreach (string line in lines)
                    {
                        string[] indexFileName = line.Split('\t');
                        
                        if (indexFileName[0] == fileName)
                        {
                            indexUpdate.Add(fileName + "\t" + hash);
                            noMatch = false;
                        }
                        else
                        {
                            indexUpdate.Add(line);
                        }
                    }

                    if(noMatch)
                    {
                        indexUpdate.Add(fileName + "\t" + hash);
                    }

                    File.WriteAllLines(indexPath, indexUpdate);

                }

            }
        }

        static void Commit(string message)
        {
            if (!Directory.Exists(".chgit"))
            {
                Console.WriteLine("Repository not initialized. Please run 'chgit init' first.");
                return;
            }

            string indexPath = Path.Combine(".chgit", "index");
            if (!File.Exists(indexPath))
            {
                Console.WriteLine("Nothing to commit. Please run 'chgit commit -m '"'Commit Description'"' first.");
                return;
            }

            string[] indexLines = File.ReadAllLines(indexPath);

            if(indexLines.Length == 0)
            {
                Console.WriteLine("Nothing to commit");
            }


            //previous commit hash
            string headPath = Path.Combine(".chgit", "HEAD");
            string prevCommitHash = null;

            if (File.Exists(headPath) && File.ReadLines(headPath).Length > 0)
            {
                prevCommitHash = File.ReadLines(headPath).Last();
            }

            //Build commit data
            StringBuilder commitContent = new StringBuilder();
            commitContent.AppendLine("parent: " + (prevCommitHash ?? ""));
            commitContent.AppendLine("date: " + DateTime.UtcNow.ToString("o"));
            commitContent.AppendLine("message: " + message);
            commitContent.AppendLine("files:");
            foreach (string line in indexLines)
            {
                commitContent.AppendLine(line);
            }

            //Hash commit data
            string commitHash;
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(commitContent.ToString()));
                commitHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            //writing commit
            string commitPath = Path.Combine(".chgit", "commits", commitHash);
            File.WriteAllText(commitPath, commitContent.ToString());

            //HEAD append
            File.AppendAllText(headPath, commitHash + "\n");

            Console.WriteLine($"Committed as {commitHash}");
        }

        static void Log()
        {
            string headPath = Path.Combine(".chgit", "HEAD");
            string[] allCommitHashes = File.ReadAllLines(headPath);

            for (int i = allCommitHashes.Length - 1; i >= 0; i--)
            {
                //reading back commit data from newest to oldest
                string commitHash = allCommitHashes[i];

                string commitPath = Path.Combine(".chgit", "commits", commitHash);

                string[] commitContent = File.ReadAllLines(commitPath);

                string date = commitContent[1];
                string message = commitContent[2];

                Console.WriteLine("commit: " + commitHash);
                Console.WriteLine(date);
                Console.WriteLine(message);
            }
        }

    }
}

