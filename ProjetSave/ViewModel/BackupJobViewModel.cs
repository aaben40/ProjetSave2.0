using ProjetSave.Model;
using ProjetSave.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjetSave.ViewModel
{
    internal class BackupJobViewModel
    {

        public enum BackupTypeEnum
        {
            Full,
            Incremental
        }

        public BackupTypeEnum BackupType { get; set; }
        public string SourceDirectory { get; set; } = string.Empty;

        public string TargetDirectory { get; set; } = string.Empty;

        public bool EncryptFiles { get; set; } = false;
        public string JobName { get; set; } = string.Empty;

        

        public BackupJobViewModel()
        {
            
        }

        public void ExecuteBackup()
        {
            BackupJob job = new BackupJob(
                name: this.JobName,
                sourceDiretory: this.SourceDirectory,
                targetDiretory: this.TargetDirectory,
                type: (BackupType)this.BackupType // Conversion explicite de BackupTypeEnum en BackupType
            );

            
        }

    }
}
