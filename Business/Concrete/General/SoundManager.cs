using System.Globalization;
using Business.Abstract.General;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Business.Concrete.General
{
    public class SoundManager : ISoundService
    {
        private readonly WebClient _webClient = new WebClient();
        private readonly IDirectoryService _directoryService;

        public SoundManager(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        public int DownloadSound(string url, string fileName, string path)
        {
            var soundData = _webClient.DownloadData(url);

            if (soundData.Length <= 0)
                return 0;

            File.WriteAllBytes(Path.Combine(path, fileName + ".mp3"), soundData);
            return soundData.Length / 1024;
        }

        public void RemoveSoundboardFromServer(string zipPath)
        {
            _directoryService.RemoveFile(zipPath);
        }

        public string CleanSoundDownloadUrl(string downloadLink)
        {
            downloadLink = downloadLink.Replace("'", "");
            downloadLink = downloadLink.Replace(";", "");
            downloadLink = downloadLink.Replace("location.href=", "");

            return downloadLink;
        }

        public string GenerateSoundFileNameForAndroid(string fileName)
        {
            var r = new Regex("(?:[^a-zA-Z0-9 ]|(?<=['\"])s)",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            fileName = r.Replace(fileName, string.Empty);

            if (Regex.IsMatch(fileName, @"^\d"))
                fileName = $"a_{fileName}";

            fileName = fileName.Replace(" ", "_").ToLower(CultureInfo.GetCultureInfo("en-US")).Trim();

            return fileName;
        }
    }
}