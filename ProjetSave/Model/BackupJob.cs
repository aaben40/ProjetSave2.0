using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProjetSave.Service;
using System.Diagnostics;

namespace ProjetSave.Model
{
    public enum BackupType
    {
        Full,
        Incremental
    }
    class BackupJob
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public BackupType Type { get; set; }
        public bool IsEncrypted { get; set; }
        public long EncryptionTimeMs { get; set; }

        

        public long TransferTimeMs { get; set; }
        public long EncryptionTime { get; set; }

        public BackupJob(string name, string sourceDiretory, string targetDiretory, BackupType type, Logger logger)
        {
            Name = name;
            SourceDirectory = sourceDiretory;
            TargetDirectory = targetDiretory;
            Type = type;
            
        }

        public void Execute()
        {
            ValidatePaths();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"Starting backup job {Name}");
            // Logic for performing backup
           

            if (IsEncrypted)
            {
                EncryptFile();
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

    }
}
