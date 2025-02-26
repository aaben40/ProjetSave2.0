using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using System.Collections.Generic;

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
        public long EncryptionTimeMs { get; set; } = 0;
    }

    public enum LogFormat
    {
        Json,
        Xml
    }

    public class Logger
    {
        public delegate void LogUpdatedHandler(string logMessage);
        public event LogUpdatedHandler LogUpdated;

        private readonly string logFilePath;
        private LogFormat logFormat;

        public Logger(string path, LogFormat format = LogFormat.Json)
        {
            this.logFilePath = path;
            this.logFormat = format;
            SetupLogFile();
        }

        private void NotifyLogUpdated(string logMessage)
        {
            LogUpdated?.Invoke(logMessage);
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

        private void LogToJson(LogEntry entry)
        {
            List<LogEntry> entries = LoadEntriesFromJson() ?? new List<LogEntry>();
            entries.Add(entry);
            string json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(logFilePath, json);
            NotifyLogUpdated(JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true }));
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
            NotifyLogUpdated($"Log added: {entry.BackupName} - {entry.Timestamp}");
        }

        private void SetupLogFile()
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
                            XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));
                            serializer.Serialize(writer, new List<LogEntry>());
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
                return JsonSerializer.Deserialize<List<LogEntry>>(content);
            }
            return new List<LogEntry>();
        }

        private List<LogEntry> LoadEntriesFromXml()
        {
            if (File.Exists(logFilePath) && new FileInfo(logFilePath).Length > 0)
            {
                using var stream = new FileStream(logFilePath, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));
                return serializer.Deserialize(stream) as List<LogEntry>;
            }
            return new List<LogEntry>();
        }
    }
}
