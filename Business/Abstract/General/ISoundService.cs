namespace Business.Abstract.General
{
    public interface ISoundService
    {
        int DownloadSound(string url, string fileName, string path);

        public void RemoveSoundboardFromServer(string zipPath);

        public string CleanSoundDownloadUrl(string downloadLink);

        string GenerateSoundFileNameForAndroid(string fileName);
    }
}
