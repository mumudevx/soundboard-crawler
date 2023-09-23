using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Business.Abstract;
using Business.Abstract.General;
using Entities;
using HtmlAgilityPack;

namespace Business.Concrete
{
    public class RealmOfDarknessNetManager : IRealmOfDarknessNetService
    {
        private readonly HtmlWeb _htmlWeb = new HtmlWeb { AutoDetectEncoding = false, OverrideEncoding = Encoding.UTF8 };

        private readonly ISoundService _soundService;
        private readonly IDirectoryService _directoryService;

        public RealmOfDarknessNetManager(ISoundService service, IDirectoryService directoryService)
        {
            _soundService = service;
            _directoryService = directoryService;
        }

        public async Task<HtmlDocument> GetHtmlFromUrl(string webUrl)
        {
            return await _htmlWeb.LoadFromWebAsync(webUrl);
        }

        public async Task<string> GetScriptUrlOfSoundboard(string webUrl)
        {
            var getHtml = await GetHtmlFromUrl(webUrl);

            var scriptFile = getHtml.DocumentNode
                .SelectNodes(".//div[contains(concat(\" \",normalize-space(@class),\" \"),\" entry-content \")]//script");

            var sourceOfScript = scriptFile.FirstOrDefault(node => node.Attributes["src"].Value.EndsWith("sb.js"))?.Attributes["src"].Value;

            return $"http://realmofdarkness.net{sourceOfScript}";
        }

        public async Task<string> GetSourceOfSoundboard(string scriptUrl)
        {
            var getHtml = await GetHtmlFromUrl(scriptUrl);

            var sourceNode = getHtml.DocumentNode.InnerText;
            var sourceStart = sourceNode.IndexOf("src: [\"", StringComparison.Ordinal);
            var sourceEnd = sourceNode.LastIndexOf("\" + sounds[i] + \".mp3\"]", StringComparison.Ordinal);
            var source = sourceNode.Substring(sourceStart + "src: [\"".Length, sourceEnd - sourceStart - "src: [\"".Length);

            return source;
        }

        public async Task<List<string>> GetSoundUrlsOfSoundboard(string webUrl)
        {
            var getHtml = await GetHtmlFromUrl(webUrl);

            var soundNodes = getHtml.DocumentNode.SelectNodes(".//div[contains(concat(\" \",normalize-space(@class),\" \"),\" sb \")]//button");

            var getScriptUrl = await GetScriptUrlOfSoundboard(webUrl);
            var sourceOfSounds = await GetSourceOfSoundboard(getScriptUrl);

            return (from soundNode in soundNodes
                    where !soundNode.InnerText.Equals("Stop Sounds")
                          && soundNode.Attributes["value"] == null
                          && soundNode.Attributes["id"] != null
                    select $"http://realmofdarkness.net{sourceOfSounds}{soundNode.Attributes["id"].Value}.mp3").ToList();
        }

        public async Task<List<string>> GetSoundNamesOfSoundboard(string webUrl)
        {
            var getHtml = await GetHtmlFromUrl(webUrl);

            var soundNodes = getHtml.DocumentNode.SelectNodes(".//div[contains(concat(\" \",normalize-space(@class),\" \"),\" sb \")]//button");

            return (from soundNode in soundNodes
                    where !soundNode.InnerText.Equals("Stop Sounds")
                          && soundNode.Attributes["value"] == null
                          && soundNode.Attributes["id"] != null
                    select HttpUtility.HtmlDecode(soundNode.InnerText)).ToList();
        }

        public async Task<List<Sound>> GetSoundsOfSoundboard(string webUrl)
        {
            var soundsOfSoundboard = new List<Sound>();
            var getSoundUrls = await GetSoundUrlsOfSoundboard(webUrl);
            var getSoundNames = await GetSoundNamesOfSoundboard(webUrl);

            for (var i = 0; i < getSoundNames.Count; i++)
            {
                var soundFileName = _soundService.GenerateSoundFileNameForAndroid(getSoundNames[i]);
                var soundRaw = $"R.raw.{soundFileName}";

                var newSound = new Sound
                {
                    Title = getSoundNames[i],
                    Raw = soundRaw,
                    FileName = soundFileName,
                    Link = getSoundUrls[i]
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
