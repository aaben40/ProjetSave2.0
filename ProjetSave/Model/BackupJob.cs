using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProjetSave.Service;
using System.Diagnostics;

using System.ComponentModel;

using System.Security.Cryptography;
using System.Windows.Controls;
using System.Windows;


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
            // Assurez-vous que les chemins sont validés
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

                // Copiez chaque fichier et cryptez-le si nécessaire
                foreach (var file in Directory.GetFiles(SourceDirectory))
                {
                    string fileName = Path.GetFileName(file);
                    string targetFilePath = Path.Combine(TargetDirectory, fileName);
                    File.Copy(file, targetFilePath, true);
                    string encryptedFilePath = targetFilePath + ".aes";
                    MessageBox.Show($"> {targetFilePath} {encryptedFilePath}");
                    EncryptFile(targetFilePath, encryptedFilePath);
                    //File.Delete(targetFilePath); // Supprimez le fichier original si seulement la version cryptée est nécessaire
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                TransferTimeMs = stopwatch.ElapsedMilliseconds;
                MessageBox.Show($"Backup job {Name} completed in {TransferTimeMs} ms");
            }
        }


        private void CopyFile(string sourcePath, string targetPath)
        {
            string fileName = Path.GetFileName(sourcePath);
            string destPath = Path.Combine(targetPath, fileName);
            File.Copy(sourcePath, destPath, true);
            Console.WriteLine($"File copied from {sourcePath} to {destPath}");

            // Vérifier si le cryptage est activé
            if (IsEncrypted)
            {
                string encryptedFilePath = destPath + ".aes";
                EncryptFile(destPath, encryptedFilePath);
                File.Delete(destPath);  // Supprimez le fichier original si vous ne voulez conserver que la version cryptée

            }
        }


        private void CopyDirectory(string sourceDir, string targetDir)

        {
            // Créer le dossier de destination s'il n'existe pas
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(targetDir, Path.GetFileName(file));
                CopyFile(file, destFile);  // CopyFile inclut maintenant le cryptage si activé
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string targetSubDir = Path.Combine(targetDir, Path.GetFileName(dir));
                CopyDirectory(dir, targetSubDir);  // Appliquer récursivement la même logique
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
            // Valider que les chemins source existent et que le chemin de destination est valide
            return File.Exists(SourceDirectory) || Directory.Exists(SourceDirectory);
        }
        public void EncryptFile(string inputFile, string outputFile)
        {
            // Clés et IV doivent être de taille appropriée pour AES
            byte[] key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
            byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");

            using (FileStream inputFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream outputFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;
                        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                        using (CryptoStream cryptoStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write))
                        {
                            try
                            {
                                inputFileStream.CopyTo(cryptoStream);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error during encryption: " + ex.Message);
                            }
                        }
                    }
                }
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



        // Dans BackupJob.cs
        public void EncryptAllFilesInTarget()
        {
            // Parcourir tous les fichiers qui viennent d’être copiés
            foreach (var file in Directory.GetFiles(TargetDirectory))
            {
                // Éviter de re-chiffrer des fichiers .aes ou personnaliser selon vos besoins
                if (!file.EndsWith(".aes", StringComparison.OrdinalIgnoreCase))
                {
                    string encryptedFile = file + ".aes";
                    EncryptFile(file, encryptedFile);
                    File.Delete(file); // On supprime la version non cryptée
                }
            }
        }

    }
}
