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

using ProjetSave.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;




namespace ProjetSave.Controller
{
    public class BackupManager
    {
        public List<BackupJob> backupJobs { get; set; } = new List<BackupJob>();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public bool IsPaused { get; private set; } = false;



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

        //        public void ExecuteJob(int jobIndex)
        //        {
        //            if (jobIndex < 0 || jobIndex >= backupJobs.Count)
        //            {
        //                throw new ArgumentOutOfRangeException(nameof(jobIndex), "Invalid job index");
        //            }
        //            var job = backupJobs[jobIndex];

        //            try
        //            {
        //<<<<<<< Updated upstream
        //                job.Execute();
        //=======
        //                // Vérifiez que le répertoire source existe
        //                if (!Directory.Exists(job.SourceDirectory))
        //                    throw new DirectoryNotFoundException($"Cannot find source directory: {job.SourceDirectory}");

        //                // Assurez-vous que le répertoire cible existe ou créez-le
        //                if (!Directory.Exists(job.TargetDirectory))
        //                    Directory.CreateDirectory(job.TargetDirectory);

        //                // Copiez tous les fichiers et répertoires de manière récursive
        //                CopyDirectory(job.SourceDirectory, job.TargetDirectory);
        //                Console.WriteLine("Backup completed successfully.");
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Error during backup: {ex.Message}");
        //                throw; // Relancez l'exception pour une gestion plus générale des erreurs
        //            }
        //        }
        private async Task CopyModifiedFiles(string sourceDir, string targetDir, CancellationToken token)
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
                await CopyModifiedFiles(directory, targetDirectoryPath,token);
            }
        }
        private async Task ExecuteIncrementalBackup(BackupJob job, CancellationToken token)
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
                await CopyModifiedFiles(job.SourceDirectory, job.TargetDirectory,token);
                Console.WriteLine("Incremental backup completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during incremental backup: {ex.Message}");
                throw; // Relancer l'exception pour permettre une gestion globale des erreurs
            }
        }
        private async Task ExecuteFullBackup(BackupJob job, CancellationToken token)
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
                await CopyDirectory(job.SourceDirectory, job.TargetDirectory,job,token);
                Console.WriteLine("Backup completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during backup: {ex.Message}");
                throw; // Relancez l'exception pour une gestion plus générale des erreurs
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



        private long CalculateFileSize(string path)
        {
            // Method to calculate the total size of files in a directory
            return new DirectoryInfo(path).EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
        }
        public async Task ExecuteJob(BackupJob job)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                ValidatePaths(job);
                // Simulate some delay for demonstration
                // Rapport initial de progression
                job.TotalFiles = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories).Length;
                var token = job.cancellationTokenSource.Token;
                switch (job.Type)
                {
                    case BackupType.Full:
                        await ExecuteFullBackup(job,token);
                        break;
                    case BackupType.Incremental:
                        await ExecuteIncrementalBackup(job,token);
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
            catch (DirectoryNotFoundException ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Le répertoire est invalide pour le job '{job.Name}': {ex.Message}", "Erreur de répertoire", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    TransferTimeMs = ex.HResult,   // 
                    EncryptionTimeMs = ex.HResult  // 
                });
            }
        }


        public void ExecuteSequentialJobs()
        {
            foreach (var job in backupJobs)
            {
                job.Execute(cancellationTokenSource.Token);
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


        public void ExecuteAllJobs(ObservableCollection<JobViewModel> jobViewModels)
        {
            var priorities = jobViewModels
                .Where(job => job.Priority.HasValue && job.Priority > 0)
                .GroupBy(job => job.Priority)
                .Where(g => g.Count() > 1)
                .ToList();

            if (priorities.Any())
            {
                // Construire un message indiquant les priorités en double
                var message = "Des doublons de priorité ont été détectés pour les priorités suivantes: " +
                              string.Join(", ", priorities.Select(g => $"{g.Key} ({g.Count()} jobs)"));
                MessageBox.Show(message + ". Veuillez attribuer des priorités uniques à chaque job.");
                return; // Arrêter l'exécution si des doublons sont présents
            }

            // Trier les jobs par priorité, en considérant ceux sans priorité ou avec zéro comme les derniers à exécuter
            var sortedJobs = jobViewModels
                .Where(job => job.Priority.HasValue && job.Priority > 0) // Filtrer les jobs avec des priorités valides et positives
                .OrderBy(job => job.Priority) // Trier par priorité
                .Concat(jobViewModels.Where(job => !job.Priority.HasValue || job.Priority <= 0)) // Ajouter à la fin les jobs sans priorité ou avec zéro
                .ToList();

            // Afficher le nombre de jobs à exécuter
            Console.WriteLine($"Executing {sortedJobs.Count} jobs in order of priority.");

            // Exécuter chaque job
            foreach (var job in sortedJobs)
            {
                // Utiliser la commande Execute liée dans le ViewModel
                job.ExecuteCommand.Execute(null);
            }
        }
        public void PauseAllJobs()
        {
            // Logique pour mettre en pause tous les jobs
            foreach (var job in backupJobs)
            {
                job.Pause();
            }
        }

        public void ResumeAllJobs()
        {
            // Logique pour reprendre tous les jobs
            foreach (var job in backupJobs)
            {
                job.Resume();
            }
        }

        public void StopAllJobs(ObservableCollection<JobViewModel> jobViewModels)
        {
            //Console.WriteLine(backupJobs.Count);
            //foreach (var job in backupJobs)
            //{
            //    job.Stop();
            //}
            var sortedJobs = jobViewModels
               // Ajouter à la fin les jobs sans priorité ou avec zéro
               .ToList();

            foreach (var job in sortedJobs)
            {
                job.StopCommand.Execute(null);
            }
        }

        //private void CopyDirectory(string sourceDir, string targetDir)
        //{
        //    // Copiez chaque fichier dans le répertoire
        //    Directory.CreateDirectory(targetDir);
        //    Console.WriteLine($"Creating target directory: {targetDir}");
        //    foreach (string file in Directory.GetFiles(sourceDir))
        //    {
        //        string targetFilePath = Path.Combine(targetDir, Path.GetFileName(file));
        //        Console.WriteLine($"Copying file from {file} to {targetFilePath}");
        //        File.Copy(file, targetFilePath, true); // true pour écraser les fichiers existants

        //    }
        //}
        private async Task CopyDirectory(string sourceDir, string targetDir, BackupJob job, CancellationToken token)
        {
            // Créer le dossier de destination s'il n'existe pas
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                await CopyFile(file, destFile, job,token);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                string destDir = Path.Combine(targetDir, dirName);
                await CopyDirectory(dir, destDir, job,token);
            }
        }

        private async Task CopyFile(string sourcePath, string targetPath, BackupJob job, CancellationToken token)
        {
            string fileName = Path.GetFileName(sourcePath);
            string destPath = targetPath;
            // Vérifiez si targetPath contient déjà le nom du fichier
            if (!targetPath.EndsWith(fileName))
            {
                destPath = Path.Combine(targetPath, fileName);
            }
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (FileStream destStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[81920];
                long totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    token.ThrowIfCancellationRequested();
                    await destStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                    int progress = (int)(totalBytesRead * 100 / sourceStream.Length);
                    Application.Current.Dispatcher.Invoke(() => job.Progress = progress);
                }
            }
            job.FilesCopied++;
            Console.WriteLine($"File copied from {sourcePath} to {destPath}");
        }
    }
}
