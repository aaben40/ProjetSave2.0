using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace ProjetSave.Service
{

    public class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string BackupName { get; set; } = string.Empty;
        public string SourceFilePath { get; set; } = string.Empty;
        public string TargetFilePath { get; set; } = string.Empty;
        public long FileSize { get; set; } = 0;
        public long TransferTimeMs { get; set; } = 0;
        public long EncryptionTimeMs { get; set; } = 0; // Temps de cryptage ou code d'erreur
    }
    public enum LogFormat
    {
        Json,
        Xml
    }
    class Logger
    {
        private readonly string logFilePath;
        private LogFormat logFormat;
       

        public Logger(string path, LogFormat format = LogFormat.Json)
        {
            this.logFilePath = path;
            this.logFormat = format;
            SetupLogFile();

        }


        
       
        public void logAction(LogEntry entry)
        {
            try
            {
                switch (logFormat)
                {
                    case LogFormat.Json:
                        LogToJson(entry);
                        break;
                    case LogFormat.Xml:
                        LogToXml(entry);
                        break;
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging action: {ex.Message}");
            }
        }

        public void LogToJson(LogEntry entry)
        {
            List<LogEntry> entries = LoadEntriesFromJson() ?? new List<LogEntry>();
            entries.Add(entry);
            string json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true});
            File.WriteAllText(logFilePath, json);

        }

        private void LogToXml(LogEntry entry)
        {
            List<LogEntry> entries = LoadEntriesFromXml() ?? new List<LogEntry>();
            entries.Add(entry);
            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));
                serializer.Serialize(writer, entries);
            }
        }

        public void SetupLogFile()
        {
            if (!File.Exists(logFilePath))
            {
                switch (logFormat)
                {
                    case LogFormat.Json:
                        File.WriteAllText(logFilePath, "[]");
                        break;
                    case LogFormat.Xml:
                        using (StreamWriter writer = new StreamWriter(logFilePath))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                            serializer.Serialize(writer, new List<string>());
                        }
                        break;
                }
            }
        }

        private List<LogEntry> LoadEntriesFromJson()
        {
            if (File.Exists(logFilePath))
            {
                string content = File.ReadAllText(logFilePath);
                var result = JsonSerializer.Deserialize<List<LogEntry>>(content);
                return result ?? new List<LogEntry>();
            }
            return new List<LogEntry>();
        }

        private List<LogEntry> LoadEntriesFromXml()
        {
            if (File.Exists(logFilePath) && new FileInfo(logFilePath).Length > 0)
            {
                using var stream = new FileStream(logFilePath, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));
                var result = serializer.Deserialize(stream) as List<LogEntry>;
                return result ?? new List<LogEntry>();
            }
            return new List<LogEntry>();
        }
        public void SwitchLogFormat(LogFormat newFormat)
        {
            if (logFormat != newFormat)
            {
                logFormat = newFormat;
                SetupLogFile();

            }
        }
    }
}
