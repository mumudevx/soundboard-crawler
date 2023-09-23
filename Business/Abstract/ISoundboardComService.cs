using Entities;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ISoundboardComService
    {
        Task<Soundboard> DownloadSounds(string downloadUrl, string soundboardName);
    }
}
