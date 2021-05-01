using System.IO;

namespace CodingChainApi.Infrastructure.Utils
{
    public static class FileUtils
    {
        public static void TryDeleteFile(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }

        public static void TryDeleteDirectory(string path)
        {
            if (Directory.Exists(path)) Directory.Delete(path);
        }

        public static FileStream CreateFileWithMissingParentDirectories(string fileFullName)
        {
            var fileName = Path.GetFileName(fileFullName);
            var parentPath = fileFullName.Replace(fileName, "");
            Directory.CreateDirectory(parentPath);
            return File.Create(fileFullName);
        }
    }
}