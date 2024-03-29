﻿namespace NCScanner.ViewModels
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Reflection;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;
    using System.Runtime.CompilerServices;

    using NCScanner.Services.Interfaces;
    using NCScanner.Commands;
    using NCScanner.Resources;
    using NCScanner.DataTypes;
    using NCScanner.Views;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly IFileService fileService = null;

        private readonly INCFileScannerService ncFileScanner = null;

        private readonly IExcelService excelService = null;

        private string ncFilePath = string.Empty;

        private string toolList = string.Empty;

        private string workOffsetList = string.Empty;

        private double xMin;

        private double yMin;

        private double zMin;

        private double xMax;

        private double yMax;

        private double zMax;

        #endregion

        #region Public Properties

        public string NCFilePath
        {
            get => this.ncFilePath;
            set
            {
                this.ncFilePath = value;
                OnPropertyChanged();
            }
        }

        public string ToolList
        {
            get => this.toolList;
            set
            {
                this.toolList = value;
                OnPropertyChanged();
            }
        }

        public string WorkOffsetList
        {
            get => this.workOffsetList;
            set
            {
                this.workOffsetList = value;
                OnPropertyChanged();
            }
        }

        public double XMin
        {
            get => this.xMin;
            set
            {
                this.xMin = value;
                OnPropertyChanged();
            }
        }

        public double YMin
        {
            get => this.yMin;
            set
            {
                this.yMin = value;
                OnPropertyChanged();
            }
        }

        public double ZMin
        {
            get => this.zMin;
            set
            {
                this.zMin = value;
                OnPropertyChanged();
            }
        }

        public double XMax
        {
            get => this.xMax;
            set
            {
                this.xMax = value;
                OnPropertyChanged();
            }
        }

        public double YMax
        {
            get => this.yMax;
            set
            {
                this.yMax = value;
                OnPropertyChanged();
            }
        }

        public double ZMax
        {
            get => this.zMax;
            set
            {
                this.zMax = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Construction

        public MainWindowViewModel(IFileService fileService, INCFileScannerService ncFileScanner, IExcelService excelService)
        {
            this.fileService = fileService;
            this.ncFileScanner = ncFileScanner;
            this.excelService = excelService;

            SettingsCommand = new DelegateCommand(OnSettingsCommand);
            BrowseCommand = new DelegateCommand(OnBrowseCommand);
            ReportCommand = new DelegateCommand(OnReportCommand);
            ScanCommand = new DelegateCommand(OnScanCommand);
        }

        #endregion

        #region Commands

        public ICommand SettingsCommand { get; }

        public ICommand BrowseCommand { get; }

        public ICommand ReportCommand { get; }

        public ICommand ScanCommand { get; }

        #endregion

        #region Private Methods  

        private void OnSettingsCommand(object parameter)
        {
            var view = new SettingsWindow()
            {
                DataContext = new SettingsWindowViewModel(),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = (Window)parameter
            };
            view.ShowDialog();
        }

        private void OnBrowseCommand(object parameter)
        {
            NCFilePath = fileService.BrowseForFile(Strings.BrowseForNCTitle,
                                                   Strings.BrowseForNCFilter,
                                                   false);
        }

        private void OnReportCommand(object parameter)
        {
            if (fileService.FileExists(NCFilePath))
            {
                var ncData = ncFileScanner.ScanNCFile(NCFilePath);
                SetNCData(ncData);

                var reportPath = fileService.SaveFileAs(Strings.SaveReportTitle,
                                                        Strings.SaveReportFilter,
                                                        true,
                                                        fileService.GetFileName(NCFilePath));
                if (reportPath.Result)
                {
                    excelService.CreateReport(ncData, reportPath.Path);
                    ResetNCData();
                }          
            }
        }

        private void OnScanCommand(object parameter)
        {
            if (fileService.FileExists(NCFilePath))
            {
                SetNCData(ncFileScanner.ScanNCFile(NCFilePath));
            }
        }

        private void SetNCData(NCData ncData)
        {
            ToolList = ncData.ToolString;
            WorkOffsetList = ncData.WorkOffsetString;
            XMin = ncData.XMin;
            YMin = ncData.YMin;
            ZMin = ncData.ZMin;
            XMax = ncData.XMax;
            YMax = ncData.YMax;
            ZMax = ncData.ZMax;
        }

        private void ResetNCData()
        {
            ToolList = string.Empty;
            WorkOffsetList = string.Empty;
            XMin = 0;
            YMin = 0;
            ZMin = 0;
            XMax = 0;
            YMax = 0;
            ZMax = 0;
        }

        #endregion

        #region Notify Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}