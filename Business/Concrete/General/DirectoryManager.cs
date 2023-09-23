using Business.Abstract.General;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Writers;
using System.IO;

namespace Business.Concrete.General
{
    public class DirectoryManager : IDirectoryService
    {
        public bool CheckIfDirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string CreateDirectory(string soundboardName)
        {
            var path = GenerateDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" ,"soundboards", soundboardName));

            Directory.CreateDirectory(path);

            return path;
        }

        public void RemoveDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public string RemoveFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return "Soundboard zip is removed.";
            }

            return "There is no soundboard zip file.";
        }

        public string GenerateDirectoryName(string path)
        {
            if (!CheckIfDirectoryExists(path))
                return path;


            var counter = 1;
            while (CheckIfDirectoryExists(path))
            {
                path = $"{path}-{counter}";
                counter++;
            }

            return path;
        }

        public string ZipSoundboard(string path, string zipName)
        {
            using (var archive = ZipArchive.Create())
            {
                archive.AddAllFromDirectory(path);
                archive.SaveTo(path + ".zip", new WriterOptions(SharpCompress.Common.CompressionType.Deflate));
            }

            RemoveDirectory(path);

            return $"/soundboards/{zipName}.zip";
        }
    }
}
