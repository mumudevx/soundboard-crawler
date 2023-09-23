using System.ComponentModel.DataAnnotations;

namespace Web.UI.Models
{
    public class DownloadSoundboardViewModel
    {
        [DataType(DataType.Url, ErrorMessage = "Please enter a valid url.")]
        [Required(ErrorMessage = "Soundboard url is required.")]
        public string SoundboardUrl { get; set; }

        [Required(ErrorMessage = "Soundboard name is required.")]
        public string SoundboardName { get; set; }

        [Required(ErrorMessage = "Package name is required.")]
        public string PackageName { get; set; }

        [Required(ErrorMessage = "Sounds amount per tab is required.")]
        public int SoundsPerTab { get; set; }

        [Required(ErrorMessage = "Main color is required.")]
        public string MainColor { get; set; }

        [Required(ErrorMessage = "Unity game id is required.")]
        public string UnityGameId { get; set; }

        [Required(ErrorMessage = "Unity banner name is required.")]
        public string UnityBannerName { get; set; } = "Banner_Android";

        [Required(ErrorMessage = "Unity interstitial name is required.")]
        public string UnityInterstitialName { get; set; } = "Interstitial_Android";

        [Required(ErrorMessage = "Tab ad frequency is required.")]
        public string TabAdFrequency { get; set; }

        [Required(ErrorMessage = "Sound ad frequency is required.")]
        public string SoundAdFrequency { get; set; }

        [Required(ErrorMessage = "Sound folder name is required.")]
        public string SoundFolderName { get; set; }
    }
}