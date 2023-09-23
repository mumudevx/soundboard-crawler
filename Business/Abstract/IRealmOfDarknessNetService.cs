using System.Threading.Tasks;
using Entities;

namespace Business.Abstract
{
    public interface IRealmOfDarknessNetService
    {
        Task<Soundboard> DownloadSounds(string downloadUrl, string soundboardName);
    }
}
