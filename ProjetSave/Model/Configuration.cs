using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ProjetSave.Model
{
    class Configuration
    {
        public string ConfigFilePath { get; set; } = "defaultConfig.json"; // Ajouter une valeur par défaut
        public List<string> EncryptionExtensions { get; set; } = new List<string>(); // Déjà bien initialisée
        public string BusinessSoftwareName { get; set; } = string.Empty; // Valeur par défaut pour éviter null
        public bool BusinessSoftwareActive { get; set; } = false; // Valeur par défaut pour bool est false
        public string LogFileFormat { get; set; } = "json"; // Valeur par défaut

        

        public void LoadConfiguration()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<Configuration>(json);
                    if (config != null)
                    {
                        EncryptionExtensions = config.EncryptionExtensions;
                        BusinessSoftwareName = config.BusinessSoftwareName;
                        BusinessSoftwareActive = config.BusinessSoftwareActive;
                        LogFileFormat = config.LogFileFormat;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
            }
        }

        public void SaveConfiguration()
        {
            try
            {
                string json = JsonSerializer.Serialize(this);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }
    }
}
