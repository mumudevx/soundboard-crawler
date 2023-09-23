namespace Business.Abstract.General
{
    public interface IDirectoryService
    {
        string CreateDirectory(string path);

        void RemoveDirectory(string path);

        string RemoveFile(string path);

        string ZipSoundboard(string path, string zipName);
    }
}
