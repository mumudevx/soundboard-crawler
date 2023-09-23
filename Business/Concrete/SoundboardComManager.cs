using Business.Abstract;
using Business.Abstract.General;
using Entities;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class SoundboardComManager : ISoundboardComService
    {
        private readonly HtmlWeb _htmlWeb = new HtmlWeb { AutoDetectEncoding = false, OverrideEncoding = Encoding.UTF8 };

        private readonly ISoundService _soundService;
        private readonly IDirectoryService _directoryService;

        public SoundboardComManager(ISoundService service, IDirectoryService directoryService)
        {
            _soundService = service;
            _directoryService = directoryService;
        }

        public async Task<HtmlDocument> GetHtmlFromUrl(string webUrl)
        {
            return await _htmlWeb.LoadFromWebAsync(webUrl);
        }

        public async Task<List<string>> GetSoundUrlsOfSoundboard(string webUrl)
        {
            var soundUrls = new List<string>();

            var getHtml = await GetHtmlFromUrl(webUrl);

            var soundNodes = getHtml.DocumentNode.SelectNodes("//a[contains(concat(\" \",normalize-space(@class),\" \"),\" downloadSound \")]");

            soundUrls.AddRange(from soundNode in soundNodes
                               select "https://www.soundboard.com" + soundNode.Attributes["href"].Value);

            return soundUrls;
        }

        public async Task<List<Sound>> GetSoundsOfSoundboard(string webUrl)
        {
            var getSoundUrls = await GetSoundUrlsOfSoundboard(webUrl);
            var soundsOfSoundboard = new List<Sound>();

            foreach (var soundUrl in getSoundUrls)
            {
                var getHtml = await GetHtmlFromUrl(soundUrl);

                var soundOnClickDownloadLink = getHtml.DocumentNode.SelectSingleNode("//button[@id=\"btnDownload\"]").Attributes["onclick"].Value;

                var soundDownloadLink = _soundService.CleanSoundDownloadUrl(soundOnClickDownloadLink);

                var soundTitle = getHtml.DocumentNode.SelectSingleNode("//div[contains(concat(\" \",normalize-space(@class),\" \"),\" download_title \")]").InnerText
                    .Trim();

                var soundFileName = _soundService.GenerateSoundFileNameForAndroid(soundTitle);

                var soundRaw = $"R.raw.{soundFileName}";

                var newSound = new Sound
                {
                    Title = soundTitle,
                    Raw = soundRaw,
                    FileName = soundFileName,
                    Link = soundDownloadLink
                };

                soundsOfSoundboard.Add(newSound);
            }

            return soundsOfSoundboard;
        }

        public async Task<Soundboard> DownloadSounds(string webUrl, string soundboardName)
        {
            var getPath = _directoryService.CreateDirectory(soundboardName);

            var soundsOfSoundboard = await GetSoundsOfSoundboard(webUrl);
            var returnedSounds = new List<Sound>();

            foreach (var sound in soundsOfSoundboard)
            {
                var getFileName = sound.FileName;

                var sizeOfSound = _soundService.DownloadSound(sound.Link, getFileName, getPath);

                if (sizeOfSound <= 0)
                    _directoryService.RemoveFile($"{getPath}/{getFileName}.mp3");
                else
                    returnedSounds.Add(sound);
            }

            var getZipLink = _directoryService.ZipSoundboard(getPath, soundboardName);

            var newSoundboard = new Soundboard
            {
                Name = soundboardName,
                Sounds = returnedSounds,
                ZipPath = $"{getPath}.zip",
                ZipLink = getZipLink
            };

            return newSoundboard;
        }
    }
}
