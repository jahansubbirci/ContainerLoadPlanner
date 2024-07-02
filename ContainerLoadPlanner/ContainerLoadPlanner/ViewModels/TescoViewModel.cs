using Caliburn.Micro;
using ExcelDataExchange.Reader;
using ÉxcelDataExchange.Writer;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TescoClpBackend;
using TescoClpBackend.ClpLogics;
using TescoClpBackend.Combinators;

namespace ContainerLoadPlanner.ViewModels
{
    public class TescoViewModel : ClientViewModel
    {
        private readonly IExcelDataReader excelDataReader;//{ get; set; }
        private ClpPreparator clpPreparator;
        private readonly CfsDataRetriever cfsDataRetriever;
        private readonly PoUploadReportDataLoader poUploadReportDataLoader;
        private readonly ExcelDataWriter2<ClpItem> excelDataWriter;
        private readonly SimpleContainer container;
        private readonly Reporting reportingService;

        public TescoViewModel(SimpleContainer container) : base(container)
        {
            //  myMaerskReport = "";
            excelDataReader = container.GetInstance<IExcelDataReader>();
            //clpPreparator = container.GetInstance<ClpPreparator>();
            cfsDataRetriever = container.GetInstance<CfsDataRetriever>();
            poUploadReportDataLoader = container.GetInstance<PoUploadReportDataLoader>();
            excelDataWriter = container.GetInstance<ExcelDataWriter2<ClpItem>>();
            this.container = container;
            reportingService = container.GetInstance<Reporting>();
        }

        private string myMaerskReport;
        private bool cutOff;

        public bool CutOff
        {
            get { return cutOff; }
            set { cutOff = value; NotifyOfPropertyChange(() => CutOff); }
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
                clpPreparator = CutOff
                    ? container.GetInstance<ClpPreparator>(CombinatorConstants.CUT_OFF)
                    : container.GetInstance<ClpPreparator>(CombinatorConstants.REGULAR);

                var cfsData = await Task.Run(() => cfsDataRetriever.GetCfsData(this.CfsReport, this.CfsReportSheet, CfsReportRange, SelectedCfs));
                var poData = await Task.Run(() => poUploadReportDataLoader.GetPoReport(MyMaerskReport));
                cfsData = cfsData.Where(a => !a.Measurement.Equals("0X0X0"));
                var clp = clpPreparator.Create(cfsData, poData, false);
                string fileName = $"TESCO CLP {DateTime.Today.ToString("yyyy-MMM-dd")}.xlsx";
                reportingService.CreateReport(fileName, clp);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
