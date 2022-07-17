using DevExpress.Mvvm;
using DiamondListCreator.Models;
using DiamondListCreator.Services;
using System.Windows.Input;

namespace DiamondListCreator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Paths = PathSettingsService.ReadSettings();
        }

        private PathSettings _paths;
        public PathSettings Paths
        {
            get { return _paths; }
            set
            {
                _paths = value;
                RaisePropertyChanged(() => Paths);
            }
        }

        private string _listText;
        public string ListText
        {
            get { return _listText; }
            set
            {
                _listText = value;
                RaisePropertyChanged(() => ListText);
            }
        }

        private bool _isListChecked;
        public bool IsListChecked
        {
            get { return _isListChecked; }
            set
            {
                _isListChecked = value;
                RaisePropertyChanged(() => IsListChecked);
                IsAccountingChecked = IsListChecked;
                IsListStickersChecked = IsListChecked;
            }
        }

        private bool _isAccountingChecked;
        public bool IsAccountingChecked
        {
            get { return _isAccountingChecked; }
            set
            {
                _isAccountingChecked = value;
                RaisePropertyChanged(() => IsAccountingChecked);
            }
        }

        private bool _isListStickersChecked;
        public bool IsListStickersChecked
        {
            get { return _isListStickersChecked; }
            set
            {
                _isListStickersChecked = value;
                RaisePropertyChanged(() => IsListStickersChecked);
            }
        }

        private bool _isLegendsChecked;
        public bool IsLegendsChecked
        {
            get { return _isLegendsChecked; }
            set
            {
                _isLegendsChecked = value;
                RaisePropertyChanged(() => IsLegendsChecked);
            }
        }

        private bool _isStickersChecked;
        public bool IsStickersChecked
        {
            get { return _isStickersChecked; }
            set
            {
                _isStickersChecked = value;
                RaisePropertyChanged(() => IsStickersChecked);
            }
        }

        private bool _isCanvasesChecked;
        public bool IsCanvasesChecked
        {
            get { return _isCanvasesChecked; }
            set
            {
                _isCanvasesChecked = value;
                RaisePropertyChanged(() => IsCanvasesChecked);
            }
        }

        public ICommand Start
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    
                },
                () => ListText != "" && (IsListChecked || IsLegendsChecked || IsStickersChecked || IsCanvasesChecked));
            }
        }


        public ICommand ChooseDiamondsFolder
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Paths.DiamondsFolderPath = FileService.OpenDirectory("Оберіть шлях до папки з алмазками", Paths.DiamondsFolderPath);
                });
            }
        }

        public ICommand ChooseFilesSaveFolder
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Paths.FilesSavePath = FileService.OpenDirectory("Оберіть шлях до збереження списку", Paths.FilesSavePath);
                });
            }
        }

        public ICommand ChooseAccountingFile
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Paths.AccountingExcelFilePath = FileService.OpenFile("Оберіть шлях до створених легенд", Paths.AccountingExcelFilePath);
                });
            }
        }

        public ICommand ChooseSavedLegendsFolder
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Paths.SavedLegendsPath = FileService.OpenDirectory("Оберіть шлях до створених легенд", Paths.SavedLegendsPath);
                });
            }
        }

        public ICommand ChooseSavedCanvasesFolder
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Paths.SavedCanvasesPath = FileService.OpenDirectory("Оберіть шлях до створених холстів", Paths.SavedCanvasesPath);
                });
            }
        }

        public ICommand ChooseCanvasesSaveFolder
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Paths.CanvasesSavePath = FileService.OpenDirectory("Обріть шлях до збереження холстів", Paths.CanvasesSavePath);
                });
            }
        }
    }
}
