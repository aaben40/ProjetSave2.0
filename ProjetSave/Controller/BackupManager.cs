using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjetSave.Model;
using System.IO;
using System.Text.Json;
using ProjetSave.Service;
using System.Diagnostics;


namespace ProjetSave.Controller
{
    public class BackupManager
    {
        public List<BackupJob> backupJobs { get; set; } = new List<BackupJob>();
       
        

        public IEnumerable<BackupJob> BackupJobs => backupJobs;
        private readonly Logger logger;

        public BackupManager(Logger logger)
        { 
            this.backupJobs = new List<BackupJob>();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        // Ajouter un job de sauvegarde
        public void AddJob(BackupJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job), "Backup job cannot be null");
                
            }
            backupJobs.Add(job);
            logger.logAction(new LogEntry
            {
                BackupName = job.Name,
                SourceFilePath = job.SourceDirectory,
                TargetFilePath = job.TargetDirectory,
                FileSize = CalculateFileSize(job.SourceDirectory),
                TransferTimeMs = 0,
                EncryptionTimeMs = 0
            });
        }

        private void ValidatePaths(BackupJob job)
        {
            if (!Directory.Exists(job.SourceDirectory))
                throw new DirectoryNotFoundException($"Source directory not found: {job.SourceDirectory}");
            if (!Directory.Exists(job.TargetDirectory))
                Directory.CreateDirectory(job.TargetDirectory);
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            // Copiez chaque fichier dans le répertoire
            Directory.CreateDirectory(targetDir);
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFilePath = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, targetFilePath, true); // true pour écraser les fichiers existants
            }

            // Copiez récursivement chaque sous-répertoire
            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string targetDirectoryPath = Path.Combine(targetDir, Path.GetFileName(directory));
                CopyDirectory(directory, targetDirectoryPath);
            }
        }

        private void ExecuteFullBackup(BackupJob job)
        {
            Console.WriteLine($"Performing full backup for {job.Name}");
            try
            {
                // Vérifiez que le répertoire source existe
                if (!Directory.Exists(job.SourceDirectory))
                    throw new DirectoryNotFoundException($"Cannot find source directory: {job.SourceDirectory}");

                // Assurez-vous que le répertoire cible existe ou créez-le
                if (!Directory.Exists(job.TargetDirectory))
                    Directory.CreateDirectory(job.TargetDirectory);

                // Copiez tous les fichiers et répertoires de manière récursive
                CopyDirectory(job.SourceDirectory, job.TargetDirectory);
                Console.WriteLine("Backup completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during backup: {ex.Message}");
                throw; // Relancez l'exception pour une gestion plus générale des erreurs
            }
        }
        private void CopyModifiedFiles(string sourceDir, string targetDir)
        {
            // Copiez chaque fichier du répertoire source
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFilePath = Path.Combine(targetDir, Path.GetFileName(file));
                // Si le fichier n'existe pas dans le répertoire cible ou s'il a été modifié
                if (!File.Exists(targetFilePath) || File.GetLastWriteTime(file) > File.GetLastWriteTime(targetFilePath))
                {
                    File.Copy(file, targetFilePath, true); // Écraser les fichiers existants
                }
            }

            // Parcourez récursivement chaque sous-répertoire
            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string targetDirectoryPath = Path.Combine(targetDir, Path.GetFileName(directory));
                if (!Directory.Exists(targetDirectoryPath))
                {
                    Directory.CreateDirectory(targetDirectoryPath);
                }
                CopyModifiedFiles(directory, targetDirectoryPath);
            }
        }
        private void ExecuteIncrementalBackup(BackupJob job)
        {
            Console.WriteLine($"Performing incremental backup for {job.Name}");
            try
            {
                // Vérifiez que le répertoire source existe
                if (!Directory.Exists(job.SourceDirectory))
                    throw new DirectoryNotFoundException($"Cannot find source directory: {job.SourceDirectory}");

                // Assurez-vous que le répertoire cible existe ou créez-le
                if (!Directory.Exists(job.TargetDirectory))
                    Directory.CreateDirectory(job.TargetDirectory);

                // Copiez les modifications incrémentielles
                CopyModifiedFiles(job.SourceDirectory, job.TargetDirectory);
                Console.WriteLine("Incremental backup completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during incremental backup: {ex.Message}");
                throw; // Relancer l'exception pour permettre une gestion globale des erreurs
            }
        }

        private long EncryptFiles(BackupJob job)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"Encrypting files for {job.Name}");
            // Simulate encryption process
            Thread.Sleep(500); // Simulate some delay for demonstration
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        //private void EncryptDirectoryFiles(string directoryPath)
        //{
        //    foreach (string file in Directory.GetFiles(directoryPath))
        //    {
        //        try
        //        {
        //            Appel à CryptoSoft pour crypter le fichier
        //            CryptoSoft.EncryptFile(file);
        //            Console.WriteLine($"Encrypted {file}");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Failed to encrypt {file}: {ex.Message}");
        //        }
        //    }

        //    Récursivité pour les sous-dossiers
        //    foreach (string subdirectory in Directory.GetDirectories(directoryPath))
        //    {
        //        EncryptDirectoryFiles(subdirectory);
        //    }
        //}

        private long CalculateFileSize(string path)
        {
            // Method to calculate the total size of files in a directory
            return new DirectoryInfo(path).EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
        }
        public void ExecuteJob(BackupJob job)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                ValidatePaths(job);
                switch (job.Type)
                {
                    case BackupType.Full:
                        ExecuteFullBackup(job);
                        break;
                    case BackupType.Incremental:
                        ExecuteIncrementalBackup(job);
                        break;
                }

                stopwatch.Stop();
                job.TransferTimeMs = stopwatch.ElapsedMilliseconds;

                if (job.IsEncrypted)
                {
                    job.EncryptionTimeMs = EncryptFiles(job);
                }

                logger.logAction(new LogEntry
                {
                    BackupName = job.Name,
                    SourceFilePath = job.SourceDirectory,
                    TargetFilePath = job.TargetDirectory,
                    FileSize = CalculateFileSize(job.SourceDirectory),
                    TransferTimeMs = job.TransferTimeMs,
                    EncryptionTimeMs = job.EncryptionTimeMs
                });
            }
            catch (Exception ex)
            {
                logger.logAction(new LogEntry
                {
                    BackupName = job.Name,
                    SourceFilePath = job.SourceDirectory,
                    TargetFilePath = job.TargetDirectory,
                    FileSize = CalculateFileSize(job.SourceDirectory),
                    TransferTimeMs = ex.HResult, // HResult used to indicate error in transfer
                    EncryptionTimeMs = ex.HResult // HResult used to indicate error in encryption
                });
            }
        }

        public void ExecuteSequentialJobs()
        {
            foreach (var job in backupJobs)
            {
                job.Execute();
            }
        }

        public void LoadJobs()
        {
            string filePath = "backupJobs.json";

            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    backupJobs = JsonSerializer.Deserialize<List<BackupJob>>(json) ?? new List<BackupJob>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading backup jobs: {ex.Message}");
            }
        }

        public void SaveJobs()
        {
            string filePath = "backupJobs.json";
            try
            {
                string json = JsonSerializer.Serialize(backupJobs);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving backup jobs: {ex.Message}");
            }
        }
    }
}
