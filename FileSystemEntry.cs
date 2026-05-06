// FileSystemEntry.cs
namespace CS3502_P3_FileSystem_JacksonDeal
{
    public class FileSystemEntry
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }

        public override string ToString() => $"[{(IsDirectory ? "DIR" : "FILE")}] {Name}";
    }
}