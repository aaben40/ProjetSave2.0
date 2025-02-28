using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProjetSave.Service;
using System.Diagnostics;
using System.ComponentModel;

namespace ProjetSave.Model
{
    public enum BackupType
    {
        Full,
        Incremental
    }
    public class BackupJob
    {

        public CancellationTokenSource? cancellationTokenSource;
        private Task? currentTask;
        private bool isPaused;
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public BackupType Type { get; set; }
        public bool IsEncrypted { get; set; }
        public long EncryptionTimeMs { get; set; }
        private int progress;
        public int Progress
        {
            get => progress;
            set
            {
                if (progress != value)
                {
                    progress = value;
                    Console.WriteLine($"Progress updated to {progress}%");
                    OnPropertyChanged(nameof(Progress));
                }
            }
        }


        private int totalFiles;
        public int TotalFiles
        {
            get => totalFiles;
            set
            {
                if (totalFiles != value)
                {
                    totalFiles = value;
                    OnPropertyChanged(nameof(TotalFiles));
                }
            }
        }

        private int filesCopied;
        public int FilesCopied
        {
            get => filesCopied;
            set
            {
                if (filesCopied != value)
                {
                    filesCopied = value;
                    OnPropertyChanged(nameof(FilesCopied));
                    Progress = (FilesCopied * 100) / TotalFiles;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public long TransferTimeMs { get; set; }
        public long EncryptionTime { get; set; }
        public int? Priority { get; set; }

        public BackupJob(string name, string sourceDiretory, string targetDiretory, BackupType type)
        {
            Name = name;
            SourceDirectory = sourceDiretory;
            TargetDirectory = targetDiretory;
            Type = type;
            cancellationTokenSource = new CancellationTokenSource();

        }

        public void Execute(CancellationToken token)
        {
            ValidatePaths();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"Starting backup job {Name}");
            // Logic for performing backup
            try
            {
                if (IsEncrypted)
                {

                    EncryptFile();

                    Console.WriteLine($"Starting backup job {Name}");

                    // Déterminer si la source est un fichier ou un dossier
                    if (File.Exists(SourceDirectory))
                    {
                        // Copie du fichier
                        CopyFile(SourceDirectory, TargetDirectory, token);
                    }
                    else if (Directory.Exists(SourceDirectory))
                    {
                        // Copie du dossier
                        CopyDirectory(SourceDirectory, TargetDirectory, token);
                    }
                    else
                    {
                        Console.WriteLine("Source path does not exist.");
                        return;
                    }

                    if (IsEncrypted)
                    {
                        EncryptFile(); // Encrypte le fichier ou le dossier destination
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Backup job {Name} cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                TransferTimeMs = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"Backup job {Name} completed in {TransferTimeMs} ms");
            }
            stopwatch.Stop();
            TransferTimeMs = stopwatch.ElapsedMilliseconds;
        }







        private void ValidatePaths()
        {
            if (!Directory.Exists(SourceDirectory) || !Directory.Exists(TargetDirectory))
            {
                throw new InvalidOperationException("Source or target directory does not exist.");
            }
        }

        //private void CopyFile(string sourcePath, string targetPath)
        //{
        //    string fileName = Path.GetFileName(sourcePath);
        //    string destPath = Path.Combine(targetPath, fileName);
        //    File.Copy(sourcePath, destPath, true);
        //    Console.WriteLine($"File copied from {sourcePath} to {destPath}");
        //}

        //private void CopyDirectory(string sourceDir, string targetDir)
        //{
        //    // Créer le dossier de destination s'il n'existe pas
        //    Directory.CreateDirectory(targetDir);

        //    foreach (var file in Directory.GetFiles(sourceDir))
        //    {
        //        string fileName = Path.GetFileName(file);
        //        string destFile = Path.Combine(targetDir, fileName);
        //        File.Copy(file, destFile, true);
        //    }

        //    foreach (var dir in Directory.GetDirectories(sourceDir))
        //    {
        //        string dirName = Path.GetFileName(dir);
        //        string destDir = Path.Combine(targetDir, dirName);
        //        CopyDirectory(dir, destDir);
        //    }
        //}

        private void CopyFile(string sourcePath, string targetPath, CancellationToken token)
        {
            string fileName = Path.GetFileName(sourcePath);
            string destPath = Path.Combine(targetPath, fileName);
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (FileStream destStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[81920];
                int bytesRead;
                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    token.ThrowIfCancellationRequested();
                    destStream.Write(buffer, 0, bytesRead);
                }
            }
            Console.WriteLine($"File copied from {sourcePath} to {destPath}");
        }

        private void CopyDirectory(string sourceDir, string targetDir, CancellationToken token)
        {
            // Créer le dossier de destination s'il n'existe pas
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                CopyFile(file, destFile, token);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                string destDir = Path.Combine(targetDir, dirName);
                CopyDirectory(dir, destDir, token);

            }
        }

        private void EncryptFile()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Logic for encrypting files
            Console.WriteLine($"Encrypting files from {SourceDirectory} to {TargetDirectory}");
            System.Threading.Thread.Sleep((int)EncryptionTimeMs);
            Console.WriteLine("Encryption completed.");
            stopwatch.Stop();
            EncryptionTime = stopwatch.ElapsedMilliseconds;
        }

        public void Pause()
        {
            // Logique pour mettre en pause le job
            Console.WriteLine($"Job {Name} paused.");
            if (currentTask != null && !isPaused)
            {
                isPaused = true;
                cancellationTokenSource?.Cancel();
                Console.WriteLine($"Job {Name} paused.");
            }
        }

        public void Resume()
        {
            // Logique pour reprendre le job
            Console.WriteLine($"Job {Name} resumed.");
            if (isPaused)
            {
                isPaused = false;
                cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;
                currentTask = Task.Run(() => Execute(token), token);
                Console.WriteLine($"Job {Name} resumed.");
            }
        }

        public void Stop()
        {
            // Logique pour arrêter le job
            Console.WriteLine($"Job {Name} stopped.");
            cancellationTokenSource?.Cancel();
            
        }


    }
}
