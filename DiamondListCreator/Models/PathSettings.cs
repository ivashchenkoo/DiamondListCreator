using DiamondListCreator.Services;
using Newtonsoft.Json;

namespace DiamondListCreator.Models
{
    public class PathSettings : BaseVM
    {
        private string _filesSavePath;
        [JsonProperty("Шлях до збереження файлів пдф")]
        public string FilesSavePath
        {
            get { return _filesSavePath; }
            set
            {
                string oldValue = _filesSavePath;
                _filesSavePath = value;
                if (oldValue != null && oldValue != _filesSavePath)
                {
                    PathSettingsService.WriteSettings(this);
                }
            }
        }

        private string _diamondsFolderPath;
        [JsonProperty("Шлях до папки з алмазками")]
        public string DiamondsFolderPath
        {
            get { return _diamondsFolderPath; }
            set
            {
                string oldValue = _diamondsFolderPath;
                _diamondsFolderPath = value;
                if (oldValue != null && oldValue != _filesSavePath)
                {
                    PathSettingsService.WriteSettings(this);
                }
            }
        }

        private string _accountingExcelFilePath;
        [JsonProperty("Шлях до файлу обліку")]
        public string AccountingExcelFilePath
        {
            get { return _accountingExcelFilePath; }
            set
            {
                string oldValue = _accountingExcelFilePath;
                _accountingExcelFilePath = value;
                if (oldValue != null && oldValue != _accountingExcelFilePath)
                {
                    PathSettingsService.WriteSettings(this);
                }
            }
        }

        private string _savedLegendsPath;
        [JsonProperty("Шлях до збережених легенд")]
        public string SavedLegendsPath
        {
            get { return _savedLegendsPath; }
            set
            {
                string oldValue = _savedLegendsPath;
                _savedLegendsPath = value;
                if (oldValue != null && oldValue != _savedLegendsPath)
                {
                    PathSettingsService.WriteSettings(this);
                }
            }
        }

        private string _canvasesSavePath;
        [JsonProperty("Шлях до збереження холстів")]
        public string CanvasesSavePath
        {
            get { return _canvasesSavePath; }
            set
            {
                string oldValue = _canvasesSavePath;
                _canvasesSavePath = value;
                if (oldValue != null && oldValue != _canvasesSavePath)
                {
                    PathSettingsService.WriteSettings(this);
                }
            }
        }

        private string _savedCanvasesPath;
        [JsonProperty("Шлях до збережених холстів")]
        public string SavedCanvasesPath {
            get { return _savedCanvasesPath; }
            set
            {
                string oldValue = _savedCanvasesPath;
                _savedCanvasesPath = value;
                if (oldValue != null && oldValue != _savedCanvasesPath)
                {
                    PathSettingsService.WriteSettings(this);
                }
            }
        }
    }
}
