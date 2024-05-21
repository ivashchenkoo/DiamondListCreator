using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using DiamondListCreator.Models;
using DiamondListCreator.Services;
using DiamondListCreator.Services.ConsumablesCreators;

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
            ListText = string.Empty;

            SaveAsWordChecked = false;

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

        private bool _saveAsWordChecked = true;
        public bool SaveAsWordChecked
        {
            get { return _saveAsWordChecked; }
            set
            {
                _saveAsWordChecked = value;
                RaisePropertyChanged(() => SaveAsWordChecked);
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

        private int _stickersProgressValue;
        public int StickersProgressValue
        {
            get { return _stickersProgressValue; }
            set
            {
                _stickersProgressValue = value;
                RaisePropertyChanged(() => StickersProgressValue);
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

                    try
                    {
                        diamonds = DiamondSettingsService.GetFromString(ListText, Paths.DiamondsFolderPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Start button - get list from string");
                        return;
                    }

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
                },
                () => ListText != string.Empty && (IsListChecked || IsLegendsChecked || IsStickersChecked || IsCanvasesChecked));
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
                    string fpath = Path.Combine(Environment.CurrentDirectory, "Config", fileName);
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
            List<DiamondSettings> diamonds = this.diamonds.OrderBy(x => x.Name).ToList();
            PathSettings paths = Paths;

            FileService.SaveAllToNewFolder(paths.CanvasesSavePath, $"Old {DateTime.Now}".Replace(":", "_"));

            string diamondsListString = string.Empty;
            using (CanvasesService canvasesService = new CanvasesService())
            {
                float percentCoef = 100f / diamonds.Count;
                for (int i = 0; i < diamonds.Count; i++)
                {
                    if (i == diamonds.Count - 1)
                    {
                        CanvasesProgressValue = 100;
                    }
                    else
                    {
                        CanvasesProgressValue = (int)(percentCoef * (i + 1));
                    }
                    diamondsListString += canvasesService.CreateAndSaveCanvas(diamonds[i], paths) + "\n";
                }
            }

            File.WriteAllText(Path.Combine(paths.CanvasesSavePath, $"Canvases {DateTime.Now:dd.MM.yyyy}.txt"), diamondsListString.TrimEnd());
        }

        /// <summary>
        /// Calls when the stickers worker`s run is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StickersBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StickersProgressValue = 0;
        }

        /// <summary>
        /// Creates the stickers pdf file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StickersBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string savePath = Paths.FilesSavePath;
            List<DiamondSettings> diamonds = this.diamonds;

            using (PdfDocumentService document = new PdfDocumentService(2480, 3507))
            {
                using (StickerCreator stickerCreator = new StickerCreator())
                {
                    float percentCoef = 100f / diamonds.Count;
                    Bitmap stickersPage = new Bitmap(2480, 3507);
                    for (int i = 0, j = 0; i < diamonds.Count; i++)
                    {
                        if (i == diamonds.Count - 1)
                        {
                            StickersProgressValue = 100;
                        }
                        else
                        {
                            StickersProgressValue = (int)(percentCoef * (i + 1));
                        }

                        if ((i % 12 == 0 && i > 0))
                        {
                            document.AddPage(stickersPage);
                            stickersPage = new Bitmap(2480, 3507);
                            j = 0;
                        }

                        int row = j / 3;
                        int column = j++ % 3;

                        stickersPage = stickerCreator.AppendStickerOnPage(stickersPage, diamonds[i], row, column);

                        if (i == diamonds.Count - 1)
                        {
                            document.AddPage(stickersPage);
                        }
                    }
                }

                document.Save(savePath, $"Stickers {DateTime.Now:dd.MM.yyyy}");
            }
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
            List<DiamondSettings> diamonds = this.diamonds.OrderBy(x => x.Name).ToList();
            PathSettings paths = Paths;

            using (PdfDocumentService document = new PdfDocumentService(2480, 3507))
            {
                using (LegendsService legendsService = new LegendsService())
                {
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
                        document.AddPagesReverse(legendsService.CreateLegends(diamonds[i], paths.SavedLegendsPath));
                    }
                }

                document.Save(paths.FilesSavePath, $"Legends {DateTime.Now:dd.MM.yyyy}");
            }
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            PathSettings paths = Paths;
            List<DiamondSettings> diamonds = this.diamonds;
            List<DiamondColor> diamondsColors = new List<DiamondColor>();
            string textList = string.Empty;

            if (IsAccountingChecked)
            {
                AccountingProgressStatus = true;
            }
            if (IsListStickersChecked)
            {
                ListStickersProgressStatus = true;
            }

            using (ExcelDiamondsListService excelService = new ExcelDiamondsListService(paths))
            {
                float percentCoef = 100f / diamonds.Count;
                for (int i = 0; i < diamonds.Count; i++)
                {
                    if (i == diamonds.Count - 1)
                    {
                        ListProgressValue = 100;
                    }
                    else
                    {
                        ListProgressValue = (int)(percentCoef * (i + 1));
                    }
                    List<DiamondColor> diamondColors = ColorsListCreator.Create(diamonds[i]);
                    diamondsColors.AddRange(diamondColors);

                    textList += $"{i + 1} - {diamonds[i].Name}";
                    textList += diamonds[i].IsStretchedCanvas ? "P\n" : "\n";
                    excelService.AddDiamondColorsToWorkBook(diamondColors, (i + 1).ToString());
                }

                if (AccountingProgressStatus)
                {
                    excelService.SaveAccounting(paths.AccountingExcelFilePath);
                    AccountingProgressStatus = false;
                }

                File.WriteAllText(Path.Combine(paths.FilesSavePath, $"DiamondsList {DateTime.Now:dd.MM.yyyy}.txt"), textList.TrimEnd());
                excelService.SaveWorkbook(paths.FilesSavePath, $"DiamondsList {DateTime.Now:dd.MM.yyyy}", SaveAsWordChecked, textList.TrimEnd());
            }
            stopwatch.Stop();
            MessageBox.Show($"{stopwatch.Elapsed.Minutes}:{stopwatch.Elapsed.Seconds}");

            if (ListStickersProgressStatus)
            {
                ListStickersService.CreateListStickersPdf(diamondsColors, paths.FilesSavePath);
                ListStickersProgressStatus = false;
            }
        }

        /// <summary>
        /// Checks for existence by main paths
        /// </summary>
        /// <param name="showMessageBox">Shows a message box with an error message if errors exist</param>
        /// <returns>True if there are no problems with main paths, otherwise false</returns>
        private bool CheckMainPathes(bool showMessageBox = false)
        {
            string pathNotFoundMessage = string.Empty;

            if (!Paths.IsDiamondsFolderPathExists())
            {
                IsListChecked = false;
                IsLegendsChecked = false;
                IsStickersChecked = false;
                IsCanvasesChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до папки з алмазками!\n";
            }

            if ((IsListChecked || IsLegendsChecked || IsStickersChecked) && !Paths.IsFilesSavePathExists())
            {
                IsListChecked = false;
                IsLegendsChecked = false;
                IsStickersChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до збереження файлів пдф!\n";
            }

            if (IsCanvasesChecked && !Paths.IsCanvasesSavePathExists())
            {
                IsCanvasesChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до збереження холстів!\n";
            }

            if (pathNotFoundMessage != string.Empty)
            {
                if (showMessageBox)
                {
                    _ = MessageBox.Show(pathNotFoundMessage.TrimEnd(), "Перевірка прописаних шляхів");
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for existence by additional paths
        /// </summary>
        /// <param name="showMessageBox">Shows a message box with an error message if errors exist</param>
        /// <returns>True if there are no problems with additional paths, otherwise false</returns>
        private bool CheckAdditionalPathes(bool showMessageBox = false)
        {
            string pathNotFoundMessage = string.Empty;

            if (IsAccountingChecked && !Paths.IsAccountingExcelFilePathExists())
            {
                IsAccountingChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до файлу обліку!\n";
            }

            if (IsLegendsChecked && !Paths.IsSavedLegendsPathExists())
            {
                IsLegendsChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до збережених легенд!\n";
            }

            if (IsCanvasesChecked && !Paths.IsSavedCanvasesPathExists())
            {
                IsCanvasesChecked = false;
                pathNotFoundMessage += "Не знайдено шлях до збережених холстів!\n";
            }

            if (pathNotFoundMessage != string.Empty)
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
