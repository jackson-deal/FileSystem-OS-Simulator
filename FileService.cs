// FileService.cs
using System;
using System.Collections.Generic;
using System.IO;

namespace CS3502_P3_FileSystem_JacksonDeal
{
    public class FileService
    {
        public List<FileSystemEntry> ListDirectory(string path)
        {
            var entries = new List<FileSystemEntry>();

            foreach (var dir in Directory.GetDirectories(path))
                entries.Add(new FileSystemEntry { Name = Path.GetFileName(dir), FullPath = dir, IsDirectory = true });

            foreach (var file in Directory.GetFiles(path))
                entries.Add(new FileSystemEntry { Name = Path.GetFileName(file), FullPath = file, IsDirectory = false });

            return entries;
        }

        public string ReadFile(string path) => File.ReadAllText(path);

        public void UpdateFile(string path, string content) => File.WriteAllText(path, content);

        public void DeleteItem(string path, bool isDirectory)
        {
            if (isDirectory) Directory.Delete(path, true);
            else File.Delete(path); // OS call: unlink
        }

        public void RenameItem(string oldPath, string newPath, bool isDirectory)
        {
            if (isDirectory) Directory.Move(oldPath, newPath);
            else File.Move(oldPath, newPath); // Atomic operation
        }

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);

        public void CreateFile(string path)
        {
            using (FileStream fs = File.Create(path)) { }
        }

        public string GetMetadata(string path, bool isDirectory)
        {
            if (isDirectory)
            {
                var di = new DirectoryInfo(path);
                return $"Type: Directory | Modified: {di.LastWriteTime}";
            }
            else
            {
                var fi = new FileInfo(path);
                return $"Type: File | Size: {fi.Length} bytes | Modified: {fi.LastWriteTime} | Read-Only: {fi.IsReadOnly}";
            }
        }
    }
}