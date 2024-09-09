using Caliburn.Micro;
using ExcelDataExchange.Reader;
using ÉxcelDataExchange.Writer;
using LoggerService;
using Microsoft.Win32;
using SharedEntities;
using SharedEntities.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TescoClpBackend;
using TescoClpBackend.ClpLogics;
using TescoClpBackend.Combinators;
using TescoClpBackend.Models;

namespace ContainerLoadPlanner.ViewModels
{
    public class TescoViewModel : ClientViewModel
    {
        private readonly IExcelDataReader excelDataReader;//{ get; set; }
        private IClpEngine clpPreparator;
        private readonly CfsDataRetriever cfsDataRetriever;
        private readonly PoUploadReportDataLoader poUploadReportDataLoader;
        private readonly ExcelDataWriter2<ClpItem> excelDataWriter;
        private readonly ClpReporting<ClpDto> clpReporting;
        private readonly SimpleContainer container;
        private readonly Reporting reportingService;
        private readonly ILoggerManager loggerManager;

        public TescoViewModel(SimpleContainer container) : base(container)
        {
            this.container = container;
            //  myMaerskReport = "";
            excelDataReader = container.GetInstance<IExcelDataReader>();
            clpPreparator = container.GetInstance<IClpEngine>();
            cfsDataRetriever = container.GetInstance<CfsDataRetriever>();
            poUploadReportDataLoader = container.GetInstance<PoUploadReportDataLoader>();
            excelDataWriter = container.GetInstance<ExcelDataWriter2<ClpItem>>();

            reportingService = container.GetInstance<Reporting>();
            clpReporting = container.GetInstance<ClpReporting<ClpDto>>();
            _settings = new ClpEngineSettings();
            loggerManager=container.GetInstance<ILoggerManager>();
        }


        private string myMaerskReport;
        private bool cutOff;
         private ClpEngineSettings _settings;

        


        public bool IgnorePoWithoutDocs
        {
            get => _settings.IgnorePoWithoutDocs;
            set
            {
                if (_settings.IgnorePoWithoutDocs != value)
                {
                    _settings.IgnorePoWithoutDocs = value;
                    NotifyOfPropertyChange(() => IgnorePoWithoutDocs);
                }
            }
        }

        public bool IsCutOff
        {
            get => _settings.IsCutOff;
            set
            {
                if (_settings.IsCutOff != value)
                {
                    _settings.IsCutOff = value;
                    NotifyOfPropertyChange(() => IsCutOff);
                }
            }
        }

        public bool PlanCVPoSeperately
        {
            get => _settings.PlanCVPoSeperately;
            set
            {
                if (_settings.PlanCVPoSeperately != value)
                {
                    _settings.PlanCVPoSeperately = value;
                    NotifyOfPropertyChange(() => PlanCVPoSeperately);
                }
            }
        }



        private Dictionary<string, List<Container<ClpItem>>> cartData;

        public Dictionary<string, List<Container<ClpItem>>> CartData
        {
            get { return cartData; }
            set { cartData = value; NotifyOfPropertyChange(() => CartData); }
        }


        public bool CutOff
        {
            get { return cutOff; }
            set { cutOff = value; NotifyOfPropertyChange(() => CutOff); }
        }
        private string cfsReportLoaderTitle = "Hello";

        public string CfsReportLoaderTitle
        {
            get { return cfsReportLoaderTitle; }
            set { cfsReportLoaderTitle = value; NotifyOfPropertyChange(() => CfsReportLoaderTitle); }
        }


        private string cfsReportHint;

        public string CfsReportHint
        {
            get { return cfsReportHint; }
            set { cfsReportHint = value; NotifyOfPropertyChange(() => CfsReportHint); }
        }

        public string MyMaerskReport
        {
            get { return myMaerskReport; }
            set { myMaerskReport = value; NotifyOfPropertyChange(() => MyMaerskReport); }
        }
        private string myMaerskReportSheet;

        public string MyMaerskReportSheet
        {
            get { return myMaerskReportSheet; }
            set { myMaerskReportSheet = value; NotifyOfPropertyChange(() => MyMaerskReportSheet); }
        }

        private string myMaerskReportRange;

        public string MyMaerskReportRange
        {
            get { return myMaerskReportRange; }
            set { myMaerskReportRange = value; NotifyOfPropertyChange(() => MyMaerskReportRange); }
        }
        private string cfsReportSheet;
        private string cfsReport;

        private string cfsReportRange;

        public string CfsReportRange
        {
            get { return cfsReportRange; }
            set { cfsReportRange = value; NotifyOfPropertyChange(() => CfsReportRange); }
        }

        public string CfsReport
        {
            get { return cfsReport; }
            set { cfsReport = value; NotifyOfPropertyChange(() => CfsReport); }
        }

        public string CfsReportSheet
        {
            get { return cfsReportSheet; }
            set
            {
                cfsReportSheet = value;
                NotifyOfPropertyChange(() => CfsReportSheet);

            }
        }
        public async void CreateClp()
        {
            try
            {
                //clpPreparator = CutOff
                //    ? container.GetInstance<ClpPreparator>(CombinatorConstants.CUT_OFF)
                //    : container.GetInstance<ClpPreparator>(CombinatorConstants.REGULAR);

                var cfsData = await Task.Run(() => cfsDataRetriever.GetCfsData(this.CfsReport, this.CfsReportSheet, CfsReportRange, SelectedCfs));


                cfsData.OrderByDescending(x => x.Lot).ToList().ForEach(a => loggerManager.LogDebug($"\tINITIAL CFS DATA\t{a.Destination}\t{a.Lot}"));
                var poData = await Task.Run(() => poUploadReportDataLoader.GetPoReport(MyMaerskReport,this.myMaerskReportSheet,this.MyMaerskReportRange));
                cfsData = cfsData.Where(a => !a.Measurement.Equals("0X0X0"));
                cfsData. OrderByDescending(x => x.Lot).ToList().ForEach(a => loggerManager.LogDebug($"\tREMOVED 0X0X0\t{a.Destination}\t{a.Lot}"));
                var clp = clpPreparator.Create(cfsData, poData, _settings);
                CartData = clp;

                OpenCartWindow(CartData);

                CreateReport(clp);
                var dtoData = TransformData(cartData);
               // ClpReporting<ClpDto> clpReporting = new ClpReporting<ClpDto>();
              
            }
            catch (Exception ex)
            {
                loggerManager.LogError(ex, ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateReport(Dictionary<string, List<Container<ClpItem>>> clp)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".xlsx";
            var result = saveFileDialog.ShowDialog();
            if ((bool)result)
            {
                string fileName = saveFileDialog.FileName;

                var transformed=TransformData(clp);
                clpReporting.CreateReport(fileName, transformed);
                //reportingService.CreateReport(fileName, clp);

            }
        }

        //public void OpenCartWindow(Dictionary<string,List<Container<ClpItem>>>dataToPass)
        //{
        //    var _windowManager=container.GetInstance<IWindowManager>();
        //    //string dataToPass = "Hello from MainViewModel!";
        //  //  _windowManager.ShowWindowAsync(new CartWindowViewModel<ClpItem>(dataToPass));
        //   _windowManager.ShowWindowAsync(new CartViewModel<ClpItem>(dataToPass),null, new Dictionary<string, object> { { "WindowState", WindowState.Normal } });
        //}

        public void OpenCartWindow(Dictionary<string, List<Container<ClpItem>>> dataToPass)
        {
            var transformedDataToPass = TransformData(dataToPass);
            var _windowManager = container.GetInstance<IWindowManager>();
            //string dataToPass = "Hello from MainViewModel!";
            //  _windowManager.ShowWindowAsync(new CartWindowViewModel<ClpItem>(dataToPass));

            _windowManager.ShowWindowAsync(new CartViewModel<ClpDto>(transformedDataToPass), null, new Dictionary<string, object> { { "WindowState", WindowState.Minimized } });
            DeactivateItemAsync(this,false,CancellationToken.None);
        }

        public Dictionary<string, List<Container<ClpDto>>> TransformData(Dictionary<string, List<Container<ClpItem>>> dataToPass)
        {
            Dictionary<string, List<Container<ClpDto>>> clpDtoDictionary = new Dictionary<string, List<Container<ClpDto>>>();

            foreach (var kvp in dataToPass)
            {
                string key = kvp.Key;
                List<Container<ClpDto>> clpDtoList = new List<Container<ClpDto>>();

                foreach (var container in kvp.Value)
                {
                    Container<ClpDto> clpDtoContainer = new Container<ClpDto>(container.Label)
                    {
                        //Label = container.Label,
                        MaxCapacity = container.MaxCapacity,
                        MinAcceptableVolume = container.MinAcceptableVolume,
                        ContainerId = container.ContainerId,
                       // RemainingCapacity = container.RemainingCapacity,
                        UsedCbm = container.UsedCbm,
                        UnitCost = container.UnitCost,
                        Items = new List<ClpDto>()
                    };

                    foreach (var clpItem in container.Items.OrderBy(a=>a.CfsReportItem.Split)
                        .ThenByDescending(a=>a.CfsReportItem.Lot))
                    {
                        ClpDto clpDto = new ClpDto(clpItem.CfsReportItem, clpItem.PoUploadReportItem);
                        clpDtoContainer.Items.Add(clpDto);
                    }
                    
                    clpDtoList.Add(clpDtoContainer);
                }

                clpDtoDictionary.Add(key, clpDtoList);
            }

            return clpDtoDictionary;
        }


    }
}
