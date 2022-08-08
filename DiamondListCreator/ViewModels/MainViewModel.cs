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

        List<DiamondSettings> diamonds;

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
            Debug.WriteLine("CanvasesBgWorker_RunWorkerCompleted");
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
            for (int i = 0; i < diamonds.Count; i++)
            {
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
            Debug.WriteLine("StickersBgWorker_RunWorkerCompleted");
        }

        /// <summary>
        /// Creates the stickers pdf file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StickersBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
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
            Debug.WriteLine("LegendsBgWorker_RunWorkerCompleted");
        }

        /// <summary>
        /// Creates the legends pdf file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LegendsBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<DiamondSettings> diamonds = this.diamonds;
            PathSettings paths = Paths;

            PdfDocumentService document = new PdfDocumentService(2480, 3507);

            LegendsService legendsService = new LegendsService();

            for (int i = 0; i < diamonds.Count; i++)
            {
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
            Debug.WriteLine("ListBgWorker_RunWorkerCompleted");
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

            List<DiamondSettings> diamonds = this.diamonds;
            List<DiamondColor> diamondsColors = new List<DiamondColor>();
            DiamondListService diamondListService = new DiamondListService(Paths);
            ColorsListCreator colorsListCreator = new ColorsListCreator();

            for (int i = 0; i < diamonds.Count; i++)
            {
                List<DiamondColor> diamondColors = colorsListCreator.Create(diamonds[i]);
                diamondsColors.AddRange(diamondColors);

                diamondListService.AddDiamondColorsToWorkBook(diamondColors, diamonds[i].ShortName);
            }

            diamondListService.SaveWorkbook(paths.FilesSavePath, $"DiamondsList {DateTime.Now:dd.MM.yyyy}");

            if (isSaveAccounting)
            {
                diamondListService.SaveAccounting(paths.AccountingExcelFilePath);
            }

            if (isSaveListStickers)
            {
                ListStickersService listStickersService = new ListStickersService();
                listStickersService.CreateListStickersPdf(diamondsColors, Paths.FilesSavePath);
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
