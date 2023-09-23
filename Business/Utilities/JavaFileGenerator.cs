using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Business.Utilities
{
    public static class JavaFileGenerator
    {
        public static async Task StartAutomation(
            string soundboardName,
            string packageName,
            string unityGameId,
            string unityBannerName,
            string unityInterstitialName,
            string tabAdFrequency,
            string soundAdFrequency,
            string soundFolderName,
            int soundsPerTab,
            string colorHex,
            ICollection<string> soundStrings,
            ICollection<string> soundPaths
        )
        {
            var soundsCount = soundStrings.Count;
            var tabAmount = CalculateTabAmount(soundsCount, soundsPerTab);
            var tabNames = new List<string>();
            
            CreatePackageFolder(packageName);

            for (var i = 0; i < tabAmount; i++)
            {
                var tabNumber = i + 1;
                var skip = i * soundsPerTab;

                var soundStringsJoined = string.Join("\", \"", soundStrings.ToList().Skip(skip).Take(soundsPerTab));
                var soundPathsJoined = string.Join(", ", soundPaths.ToList().Skip(skip).Take(soundsPerTab));
                
                tabNames.Add($"<string name=\"tab{tabNumber}\">Sounds {tabNumber}</string>");

                await TabJavaStringGenerator(tabNumber, packageName, soundStringsJoined, soundPathsJoined);
                await TabLayoutFileGenerator(packageName, tabNumber);
            }

            await TabFragmentFileGenerator(packageName, tabAmount);
            await MainActivityFileGenerator(packageName, tabAmount);
            await ColorFileGenerator(packageName, colorHex);

            var tabNamesJoined = string.Join("\n", tabNames);
            await StringFileGenerator
            (
                soundboardName,
                packageName,
                unityGameId, unityBannerName, unityInterstitialName,
                tabAdFrequency, soundAdFrequency,
                soundFolderName,
                tabNamesJoined
            );
        }

        private static async Task TabJavaStringGenerator
        (
            int tabNumber,
            string packageName,
            string soundStringsJoined,
            string soundPathsJoined
        )
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tabFilePath = Path.Combine(dirPath ?? "", "Files", "Originals", "tab.java");

            var text = await File.ReadAllTextAsync(tabFilePath);
            text = text.Replace("{{packageName}}", packageName);
            text = text.Replace("{{tabNumber}}", tabNumber.ToString());
            text = text.Replace("{{soundStrings}}", $"\"{soundStringsJoined}\"");
            text = text.Replace("{{soundPaths}}", soundPathsJoined);

            await File.WriteAllTextAsync(Path.Combine(dirPath ?? "", "Files", packageName, $"Tab{tabNumber}.java"),
                text);
        }

        private static async Task TabLayoutFileGenerator(string packageName, int tabNumber)
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var layoutFilePath = Path.Combine(dirPath ?? "", "Files", "Originals", "tab_layout.xml");

            var text = await File.ReadAllTextAsync(layoutFilePath);
            text = text.Replace("{{tabNumber}}", tabNumber.ToString());

            await File.WriteAllTextAsync(
                Path.Combine(dirPath ?? "", "Files", packageName, $"tab{tabNumber}_layout.xml"), text);
        }

        private static async Task TabFragmentFileGenerator(string packageName, int tabAmount)
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tabFragmentFilePath = Path.Combine(dirPath ?? "", "Files", "Originals", "tabfragment.java");

            var tabImports = new List<string>();
            var tabGetItems = new List<string>();
            var tabPositions = new List<string>();

            for (var i = 0; i < tabAmount; i++)
            {
                var tabNumber = i + 1;

                tabImports.Add($"import {packageName}.tabs.Tab{tabNumber};");
                tabGetItems.Add($"if (position == {i}) {{ return new Tab{tabNumber}(); }}");
                tabPositions.Add($"case {i}: return getText(R.string.tab{tabNumber});");
            }

            var text = await File.ReadAllTextAsync(tabFragmentFilePath);
            text = text.Replace("{{packageName}}", packageName);
            text = text.Replace("{{tabImports}}", string.Join("\n", tabImports));
            text = text.Replace("{{tabAmount}}", string.Join("\n", tabAmount));
            text = text.Replace("{{tabGetItem}}", string.Join("\n", tabGetItems));
            text = text.Replace("{{tabPositions}}", string.Join("\n", tabPositions));

            await File.WriteAllTextAsync(Path.Combine(dirPath ?? "", "Files", packageName, "TabFragment.java"), text);
        }

        private static async Task MainActivityFileGenerator(string packageName, int tabAmount)
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mainActivityFilePath = Path.Combine(dirPath ?? "", "Files", "Originals", "mainactivity.java");

            var tabImports = new List<string>();
            var tabClickMethods = new List<string>();

            for (var i = 0; i < tabAmount; i++)
            {
                var tabNumber = i + 1;
                tabImports.Add($"import {packageName}.tabs.Tab{tabNumber};");
                tabClickMethods.Add(
                    $"public void Tab{tabNumber}ItemClicked(int position) {{ cleanUpMediaPlayer(); mp = MediaPlayer.create(MainActivity.this, Tab{tabNumber}.soundfiles[position]); mp.start(); sound_played_counter++; if (Integer.parseInt(getText(R.string.interstitial_ad_frequency_sound).toString()) == sound_played_counter) {{ DisplayInterstitialAd(); sound_played_counter = 0; }}}}");
            }

            var text = await File.ReadAllTextAsync(mainActivityFilePath);
            text = text.Replace("{{packageName}}", packageName);
            text = text.Replace("{{tabImports}}", string.Join("\n", tabImports));
            text = text.Replace("{{tabClickMethods}}", string.Join("\n \n", tabClickMethods));

            await File.WriteAllTextAsync(Path.Combine(dirPath ?? "", "Files", packageName, "MainActivity.java"), text);
        }

        private static async Task ColorFileGenerator(string packageName, string colorHex)
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var colorFilePath = Path.Combine(dirPath ?? "", "Files", "Originals", "color.xml");

            var text = await File.ReadAllTextAsync(colorFilePath);
            text = text.Replace("{{mainColor}}", colorHex);

            await File.WriteAllTextAsync(Path.Combine(dirPath ?? "", "Files", packageName, "color.xml"), text);
        }

        private static async Task StringFileGenerator
        (
            string soundboardName,
            string packageName,
            string unityGameId,
            string unityBannerName,
            string unityInterstitialName,
            string tabAdFrequency,
            string soundAdFrequency,
            string soundFolderName,
            string tabNames
        )
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var stringFilePath = Path.Combine(dirPath ?? "", "Files", "Originals", "strings.xml");

            var text = await File.ReadAllTextAsync(stringFilePath);
            text = text.Replace("{{soundboardName}}", soundboardName);
            text = text.Replace("{{packageName}}", packageName);
            text = text.Replace("{{unityGameId}}", unityGameId);
            text = text.Replace("{{unityBannerName}}", unityBannerName);
            text = text.Replace("{{unityInterstitialName}}", unityInterstitialName);
            text = text.Replace("{{tabAdFrequency}}", tabAdFrequency);
            text = text.Replace("{{soundAdFrequency}}", soundAdFrequency);
            text = text.Replace("{{soundFolderName}}", soundFolderName);
            text = text.Replace("{{tabNames}}", tabNames);

            await File.WriteAllTextAsync(Path.Combine(dirPath ?? "", "Files", packageName, "strings.xml"), text);
        }

        private static int CalculateTabAmount(int soundsCount, int soundsPerTab)
        {
            return (int) (soundsCount <= soundsPerTab ? 1 : Math.Ceiling((double) soundsCount / soundsPerTab));
        }

        private static void CreatePackageFolder(string packageName)
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var packageFolder = Path.Combine(dirPath ?? "", "Files", packageName);

            if (!Directory.Exists(packageFolder))
                Directory.CreateDirectory(packageFolder);
        }
    }
}