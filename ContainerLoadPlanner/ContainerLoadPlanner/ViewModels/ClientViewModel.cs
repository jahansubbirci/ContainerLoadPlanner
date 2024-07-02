using Caliburn.Micro;
using ÉxcelDataExchange;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerLoadPlanner.ViewModels
{
    public class ClientViewModel : Screen
    {

        public ClientViewModel(SimpleContainer container)
        {
            this.container = container;
           // CfsReport = "";
            excelUtilities = container.GetInstance<ExcelUtilities>();
            CfsCollection = Enum.GetValues(typeof(CFS)).Cast<CFS>().ToList();

        }
        //private string cfsReportFile;
        private readonly SimpleContainer container;
        private readonly ExcelUtilities excelUtilities;
        //private List<string> cfsReportSheets;

        private CFS selectedCfs;

        public CFS SelectedCfs
        {
            get { return selectedCfs; }
            set { selectedCfs = value; NotifyOfPropertyChange(() => SelectedCfs); }
        }

        private IReadOnlyCollection<CFS> cfsCollection;

        public IReadOnlyCollection<CFS> CfsCollection
        {
            get { return cfsCollection; }
            set { cfsCollection = value; NotifyOfPropertyChange(() => CfsCollection); }
        }


       

        //public List<string> CfsReportSheets
        //{
        //    get { return cfsReportSheets; }
        //    set
        //    {
        //        cfsReportSheets = value;
        //        NotifyOfPropertyChange(() => CfsReportSheets);
        //    }
        //}

        //public string CfsReportFile
        //{
        //    get { return cfsReportFile; }
        //    set { cfsReportFile = value; NotifyOfPropertyChange(() => CfsReportFile); }
        //}
        //public void BrowseCfsReportButton()
        //{
        //    if (!string.IsNullOrEmpty(CfsReportFile))
        //    {
        //        CfsReportSheets = excelUtilities
        //            .GetSheetNames(CfsReportFile)
        //            .ToList();
        //    }
        //}
    }
}
