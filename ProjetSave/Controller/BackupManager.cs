using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjetSave.Model;
using System.IO;
using System.Text.Json;
using ProjetSave.Service;


namespace ProjetSave.Controller
{
    class BackupManager
    {
        public List<BackupJob> backupJobs { get; set; } = new List<BackupJob>();
       
        

        public IEnumerable<BackupJob> BackupJobs => backupJobs;
        private readonly Logger logger;

        public BackupManager(Logger logger)
        { 
            this.backupJobs = new List<BackupJob>();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public long CalculateFileSize(string filePath)
        {
            if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }
            return 0;
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

        public void ExecuteJob(int jobIndex)
        {
            if (jobIndex < 0 || jobIndex >= backupJobs.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(jobIndex), "Invalid job index");
            }
            var job = backupJobs[jobIndex];

            try
            {
                job.Execute();
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
                    TransferTimeMs = ex.HResult,
                    EncryptionTimeMs = ex.HResult
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
