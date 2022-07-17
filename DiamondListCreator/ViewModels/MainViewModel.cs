using DevExpress.Mvvm;
using DiamondListCreator.Models;
using DiamondListCreator.Services;
using System.Windows;
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

    }
}
