# ChGit
*A lightweight Git-like version control system written in C#.*

## ✨ Features
- `init` – Initialize a new ChGit repository
- `add <file>` – Stage a file (stores content by SHA-256 hash)
- `commit -m "message"` – Create a commit snapshot with message + parent
- `log` – View commit history (newest → oldest)
- `checkout <hash>` – Restore files from a previous commit

## 🚀 Getting Started
Clone and run from the command line:

```bash
git clone https://github.com/CHHerndon/ChGit.git
cd ChGit
dotnet build
dotnet run -- <command> [options]
```

### Example Workflow
This will initialize the repository, create a text file, update the text file, and print a log of the history:
```bash
dotnet run -- init

echo Hello ChGit! > hello.txt
dotnet run -- add hello.txt
dotnet run -- commit -m "Initial commit"

echo Goodbye ChGit! >> hello.txt
dotnet run -- add hello.txt
dotnet run -- commit -m "Updated hello.txt"

dotnet run -- log
```
Afterwards you can utilize checkout to restore the text file before the update:
```bash
dotnet run -- checkout <commitHash>
```

## 📂 How it works
- Files stored in `.chgit/objects/` by **SHA-256** hash  
- `.chgit/index` maps `filename ↔ hash` for staged files  
- Commits in `.chgit/commits/` hold message, parent, and file list  
- `HEAD` tracks commit history (latest at bottom)

## 🎯 Why I Built This
I created ChGit as a learning project to better understand how real version control systems like Git work under the hood.  

This project helped me practice:
- Using SHA-256 hashing to identify and store file content
- Designing a simple CLI tool in C#
- Implementing commit history and file restoration
- Managing staged vs committed changes through an index
