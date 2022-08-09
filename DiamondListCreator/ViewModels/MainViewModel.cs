using DevExpress.Mvvm;
using DiamondListCreator.Models;
using DiamondListCreator.Services;
using DiamondListCreator.Services.ConsumablesCreators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DiamondListCreator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly BackgroundWorker listBgWorker = new BackgroundWorker();
        private readonly BackgroundWorker legendsBgWorker = new BackgroundWorker();
        private readonly BackgroundWorker stickersBgWorker = new BackgroundWorker();
        private readonly BackgroundWorker canvasesBgWorker = new BackgroundWorker();

        private List<DiamondSettings> diamonds;

        public MainViewModel()
        {
            Paths = PathSettingsService.ReadSettings();

            IsListChecked = true;
            IsLegendsChecked = true;
            IsStickersChecked = true;
            IsCanvasesChecked = true;
            ListText = "";

            CheckMainPathes();

            // Initializing workers
            listBgWorker.DoWork += ListBgWorker_DoWork;
            listBgWorker.WorkerReportsProgress = true;
            listBgWorker.RunWorkerCompleted += ListBgWorker_RunWorkerCompleted;

            legendsBgWorker.DoWork += LegendsBgWorker_DoWork;
            legendsBgWorker.WorkerReportsProgress = true;
            legendsBgWorker.RunWorkerCompleted += LegendsBgWorker_RunWorkerCompleted;

            stickersBgWorker.DoWork += StickersBgWorker_DoWork;
            stickersBgWorker.WorkerReportsProgress = true;
            stickersBgWorker.RunWorkerCompleted += StickersBgWorker_RunWorkerCompleted;

            canvasesBgWorker.DoWork += CanvasesBgWorker_DoWork;
            canvasesBgWorker.WorkerReportsProgress = true;
            canvasesBgWorker.RunWorkerCompleted += CanvasesBgWorker_RunWorkerCompleted;
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

        private bool _isListChecked = true;
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

        private bool _isAccountingChecked = true;
        public bool IsAccountingChecked
        {
            get { return _isAccountingChecked; }
            set
            {
                _isAccountingChecked = value;
                RaisePropertyChanged(() => IsAccountingChecked);
            }
        }

        private bool _isListStickersChecked = true;
        public bool IsListStickersChecked
        {
            get { return _isListStickersChecked; }
            set
            {
                _isListStickersChecked = value;
                RaisePropertyChanged(() => IsListStickersChecked);
            }
        }

        private bool _isLegendsChecked = true;
        public bool IsLegendsChecked
        {
            get { return _isLegendsChecked; }
            set
            {
                _isLegendsChecked = value;
                RaisePropertyChanged(() => IsLegendsChecked);
            }
        }

        private bool _isStickersChecked = true;
        public bool IsStickersChecked
        {
            get { return _isStickersChecked; }
            set
            {
                _isStickersChecked = value;
                RaisePropertyChanged(() => IsStickersChecked);
            }
        }

        private bool _isCanvasesChecked = true;
        public bool IsCanvasesChecked
        {
            get { return _isCanvasesChecked; }
            set
            {
                _isCanvasesChecked = value;
                RaisePropertyChanged(() => IsCanvasesChecked);
            }
        }

        private int _listProgressValue;
        public int ListProgressValue
        {
            get { return _listProgressValue; }
            set
            {
                _listProgressValue = value;
                RaisePropertyChanged(() => ListProgressValue);
            }
        }

        private int _legendsProgressValue;
        public int LegendsProgressValue
        {
            get { return _legendsProgressValue; }
            set
            {
                _legendsProgressValue = value;
                RaisePropertyChanged(() => LegendsProgressValue);
            }
        }

        private bool _stickersProgressStatus;
        public bool StickersProgressStatus
        {
            get { return _stickersProgressStatus; }
            set
            {
                _stickersProgressStatus = value;
                RaisePropertyChanged(() => StickersProgressStatus);
            }
        }

        private int _canvasesProgressValue;
        public int CanvasesProgressValue
        {
            get { return _canvasesProgressValue; }
            set
            {
                _canvasesProgressValue = value;
                RaisePropertyChanged(() => CanvasesProgressValue);
            }
        }

        private bool _accountingProgressStatus;
        public bool AccountingProgressStatus
        {
            get { return _accountingProgressStatus; }
            set
            {
                _accountingProgressStatus = value;
                RaisePropertyChanged(() => AccountingProgressStatus);
            }
        }

        private bool _listStickersProgressStatus;
        public bool ListStickersProgressStatus
        {
            get { return _listStickersProgressStatus; }
            set
            {
                _listStickersProgressStatus = value;
                RaisePropertyChanged(() => ListStickersProgressStatus);
            }
        }

        public ICommand Start
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (!CheckMainPathes(true))
                    {
                        return;
                    }

                    if (!CheckAdditionalPathes(true) &&
                        MessageBox.Show("Продовжити виконання без введення цих шляхів?", "Продовжити?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        return;
                    }

                    diamonds = DiamondSettingsService.GetFromString(ListText, Paths.DiamondsFolderPath);
                    if (diamonds.Count == 0)
                    {
                        return;
                    }

                    if (IsListChecked && !listBgWorker.IsBusy)
                    {
                        listBgWorker.RunWorkerAsync();
                    }

                    if (IsLegendsChecked && !legendsBgWorker.IsBusy)
                    {
                        legendsBgWorker.RunWorkerAsync();
                    }

                    if (IsStickersChecked && !stickersBgWorker.IsBusy)
                    {
                        stickersBgWorker.RunWorkerAsync();
                    }

                    if (IsCanvasesChecked && !canvasesBgWorker.IsBusy)
                    {
                        canvasesBgWorker.RunWorkerAsync();
                    }

                    //CreatorService.Create(diamonds, IsListChecked, IsAccountingChecked, IsListStickersChecked, IsLegendsChecked, IsStickersChecked, IsCanvasesChecked);
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

        public ICommand EditTxtFile
        {
            get
            {
                return new DelegateCommand<string>((fileName) =>
                {
                    string fpath = $"{Environment.CurrentDirectory}\\Config\\{fileName}";
                    DateTime l_date = File.GetLastWriteTime(fpath);
                    Process proc = Process.Start("notepad.exe", fpath);
                    proc.WaitForExit();
                    proc.Close();
                    DateTime date = File.GetLastWriteTime(fpath);
                    if (date != l_date)
                    {
                        _ = MessageBox.Show("Зміни збережено!", "Редагування txt файлу");
                    }
                });
            }
        }

        /// <summary>
        /// Calls when the canvases worker`s run is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasesBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CanvasesProgressValue = 0;
        }

        /// <summary>
        /// Creates the canvases tif files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasesBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<DiamondSettings> diamonds = this.diamonds;
            PathSettings paths = Paths;

            FileService.SaveAllToNewFolder(paths.CanvasesSavePath, $"Old {DateTime.Now}".Replace(":", "_"));
            CanvasesService canvasesService = new CanvasesService();

            string diamondsListString = "";

            float percentCoef = 100f / diamonds.Count;
            for (int i = 0; i < diamonds.Count; i++)
            {
                CanvasesProgressValue = (int)(percentCoef * (i + 1));
                diamondsListString += canvasesService.CreateAndSaveCanvas(diamonds[i], paths) + "\n";
            }

            File.WriteAllText($"{paths.CanvasesSavePath}/Canvases {DateTime.Now:dd.MM.yyyy}.txt", diamondsListString.TrimEnd());
        }

        /// <summary>
        /// Calls when the stickers worker`s run is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StickersBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StickersProgressStatus = false;
        }

        /// <summary>
        /// Creates the stickers pdf file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StickersBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StickersProgressStatus = true;
            string savePath = Paths.FilesSavePath;
            PdfDocumentService document = new PdfDocumentService(2480, 3507);

            StickerCreator stickerCreator = new StickerCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));
            document.AddPagesReverse(stickerCreator.CreateStickersPage(diamonds));

            document.Save($"{savePath}/Stickers {DateTime.Now:dd.MM.yyyy}.pdf");
        }

        /// <summary>
        /// Calls when the legends worker`s run is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LegendsBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LegendsProgressValue = 0;
        }

        /// <summary>
        /// Creates the legends pdf file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LegendsBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine("\n\n\nLegendsBgWorker_DoWork\n\n");
            List<DiamondSettings> diamonds = this.diamonds;
            PathSettings paths = Paths;

            PdfDocumentService document = new PdfDocumentService(2480, 3507);

            LegendsService legendsService = new LegendsService();

            float percentCoef = 100f / diamonds.Count;
            for (int i = 0; i < diamonds.Count; i++)
            {
                if (i == diamonds.Count - 1)
                {
                    LegendsProgressValue = 100;
                }
                else
                {
                    LegendsProgressValue = (int)(percentCoef * (i + 1)); 
                }
                document.AddPagesReverse(legendsService.CreateLegends(diamonds[i], paths));
            }

            document.Save($"{paths.FilesSavePath}/Legends {DateTime.Now:dd.MM.yyyy}.pdf");
        }

        /// <summary>
        /// Calls when the list worker`s run is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ListProgressValue = 0;
        }

        /// <summary>
        /// Creates and saves the diamonds list Excel file and saves the colors accounting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            PathSettings paths = Paths;
            bool isSaveAccounting = IsAccountingChecked;
            bool isSaveListStickers = IsListStickersChecked;
            if (isSaveAccounting)
            {
                AccountingProgressStatus = true;
            }
            if (isSaveListStickers)
            {
                ListStickersProgressStatus = true;
            }

            List<DiamondSettings> diamonds = this.diamonds;
            List<DiamondColor> diamondsColors = new List<DiamondColor>();
            DiamondListService diamondListService = new DiamondListService(Paths);
            ColorsListCreator colorsListCreator = new ColorsListCreator();

            float percentCoef = 100f / diamonds.Count;
            for (int i = 0; i < diamonds.Count; i++)
            {
                ListProgressValue = (int)(percentCoef * (i + 1));
                List<DiamondColor> diamondColors = colorsListCreator.Create(diamonds[i]);
                diamondsColors.AddRange(diamondColors);

                diamondListService.AddDiamondColorsToWorkBook(diamondColors, diamonds[i].ShortName);
            }

            diamondListService.SaveWorkbook(paths.FilesSavePath, $"DiamondsList {DateTime.Now:dd.MM.yyyy}");

            if (isSaveAccounting)
            {
                diamondListService.SaveAccounting(paths.AccountingExcelFilePath);
                AccountingProgressStatus = false;
            }

            if (isSaveListStickers)
            {
                ListStickersService listStickersService = new ListStickersService();
                listStickersService.CreateListStickersPdf(diamondsColors, Paths.FilesSavePath);
                ListStickersProgressStatus = false;
            }
        }

        private bool CheckMainPathes(bool showMessageBox = false)
        {
            string pathNotFoundMessage = "";

            if (!Paths.IsFilesSavePathExists())
            {
                IsListChecked = false;
                IsLegendsChecked = false;
                IsStickersChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до збереження файлів пдф!\n";
            }

            if (!Paths.IsDiamondsFolderPathExists())
            {
                IsListChecked = false;
                IsLegendsChecked = false;
                IsStickersChecked = false;
                IsCanvasesChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до папки з алмазками!\n";
            }

            if (!Paths.IsCanvasesSavePathExists())
            {
                IsCanvasesChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до збереження холстів!\n";
            }

            if (pathNotFoundMessage != "")
            {
                if (showMessageBox)
                {
                    _ = MessageBox.Show(pathNotFoundMessage.TrimEnd(), "Перевірка прописаних шляхів"); 
                }
                return false;
            }

            return true;
        }
        
        private bool CheckAdditionalPathes(bool showMessageBox = false)
        {
            string pathNotFoundMessage = "";

            if (!Paths.IsAccountingExcelFilePathExists())
            {
                IsAccountingChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до файлу обліку!\n";
            }

            if (!Paths.IsSavedLegendsPathExists())
            {
                IsLegendsChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до збережених легенд!\n";
            }

            if (!Paths.IsSavedCanvasesPathExists())
            {
                IsCanvasesChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до збережених холстів!\n";
            }

            if (pathNotFoundMessage != "")
            {
                if (showMessageBox)
                {
                    _ = MessageBox.Show(pathNotFoundMessage.TrimEnd(), "Перевірка прописаних шляхів");
                }
                return false;
            }

            return true;
        }
    }
}
