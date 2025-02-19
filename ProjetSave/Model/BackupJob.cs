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

        public BackupJob(string name, string sourceDiretory, string targetDiretory, BackupType type)
        {
            Name = name;
            SourceDirectory = sourceDiretory;
            TargetDirectory = targetDiretory;
            Type = type;
            
        }

        public void Execute()
        {
            if (!ValidatePaths())
            {
                Console.WriteLine("Validation des chemins a échoué.");
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                Console.WriteLine($"Starting backup job {Name}");

                // Déterminer si la source est un fichier ou un dossier
                if (File.Exists(SourceDirectory))
                {
                    // Copie du fichier
                    CopyFile(SourceDirectory, TargetDirectory);
                }
                else if (Directory.Exists(SourceDirectory))
                {
                    // Copie du dossier
                    CopyDirectory(SourceDirectory, TargetDirectory);
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
        }

        private void CopyFile(string sourcePath, string targetPath)
        {
            string fileName = Path.GetFileName(sourcePath);
            string destPath = Path.Combine(targetPath, fileName);
            File.Copy(sourcePath, destPath, true);
            Console.WriteLine($"File copied from {sourcePath} to {destPath}");
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            // Créer le dossier de destination s'il n'existe pas
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destFile, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                string destDir = Path.Combine(targetDir, dirName);
                CopyDirectory(dir, destDir);
            }
        }

        private bool ValidatePaths()
        {
            // Valider que les chemins source existent et que le chemin de destination est valide
            return File.Exists(SourceDirectory) || Directory.Exists(SourceDirectory);
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
