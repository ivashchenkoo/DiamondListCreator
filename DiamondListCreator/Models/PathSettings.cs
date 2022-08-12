using System.IO;
using DevExpress.Mvvm;
using DiamondListCreator.Services;
using Newtonsoft.Json;

namespace DiamondListCreator.Models
{
    public class PathSettings : ViewModelBase
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
                RaisePropertyChanged(() => FilesSavePath);
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
                RaisePropertyChanged(() => DiamondsFolderPath);
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
                RaisePropertyChanged(() => AccountingExcelFilePath);
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
                RaisePropertyChanged(() => SavedLegendsPath);
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
                RaisePropertyChanged(() => CanvasesSavePath);
            }
        }

        private string _savedCanvasesPath;
        [JsonProperty("Шлях до збережених холстів")]
        public string SavedCanvasesPath
        {
            get { return _savedCanvasesPath; }
            set
            {
                string oldValue = _savedCanvasesPath;
                _savedCanvasesPath = value;
                if (oldValue != null && oldValue != _savedCanvasesPath)
                {
                    PathSettingsService.WriteSettings(this);
                }
                RaisePropertyChanged(() => SavedCanvasesPath);
            }
        }

        public bool IsFilesSavePathExists()
        {
            return Directory.Exists(FilesSavePath);
        }

        public bool IsDiamondsFolderPathExists()
        {
            return Directory.Exists(DiamondsFolderPath);
        }

        public bool IsAccountingExcelFilePathExists()
        {
            return File.Exists(AccountingExcelFilePath);
        }

        public bool IsSavedLegendsPathExists()
        {
            return Directory.Exists(SavedLegendsPath);
        }

        public bool IsCanvasesSavePathExists()
        {
            return Directory.Exists(CanvasesSavePath);
        }

        public bool IsSavedCanvasesPathExists()
        {
            return Directory.Exists(SavedCanvasesPath);
        }
    }
}
