using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContainerLoadPlanner.Views
{
    /// <summary>
    /// Interaction logic for ExcelLoader.xaml
    /// </summary>
    public partial class ExcelLoader : UserControl
    {
        public ExcelLoader()
        {
            InitializeComponent();
           // DataContext = this;
            Range = "A1:BZ";
        }

        public string FilePath
        {
            get { return (string)this.GetValue(FilePathProperty); }
            set { this.SetValue(FilePathProperty, value); }
        }
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(ExcelLoader), new PropertyMetadata(string.Empty));


        public string SelectedSheet
        {
            get { return (string)this.GetValue(SelectedSheetProperty); }
            set { this.SetValue(SelectedSheetProperty, value); }
        }
        public static readonly DependencyProperty SelectedSheetProperty = DependencyProperty.Register("SelectedSheet", typeof(string), typeof(ExcelLoader), new PropertyMetadata(string.Empty));


        public string Range
        {
            get { return (string)this.GetValue(RangeProperty); }
            set { this.SetValue(RangeProperty, value); }
        }
        public static readonly DependencyProperty RangeProperty = DependencyProperty.Register("Range", typeof(string), typeof(ExcelLoader), new PropertyMetadata(string.Empty));


        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ExcelLoader), new PropertyMetadata(string.Empty));


        public string FileNameHint
        {
            get { return (string)this.GetValue(FileNameHintProperty); }
            set { this.SetValue(FileNameHintProperty, value); }
        }
        public static readonly DependencyProperty FileNameHintProperty = DependencyProperty.Register("FileNameHint", typeof(string), typeof(string));

        public string SheetNameHint
        {
            get { return (string)this.GetValue(SheetNameHintProperty); }
            set { this.SetValue(SheetNameHintProperty, value); }
        }
        public static readonly DependencyProperty SheetNameHintProperty = DependencyProperty.Register("SheetNameHint", typeof(string), typeof(string));
        public string RangeHint
        {
            get { return (string)this.GetValue(RangeHintProperty); }
            set { this.SetValue(RangeHintProperty, value); }
        }
        public static readonly DependencyProperty RangeHintProperty = DependencyProperty.Register("RangeHint", typeof(string), typeof(string));

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel|*.xls;*.xlsx";
            try
            {
                if ((bool)openFileDialog.ShowDialog())
                {
                    FilePathTextBox.Text = openFileDialog.FileName;
                    var sheets = GetExcelSheetNames(openFileDialog.FileName);
                    SheetNameCombo.ItemsSource = sheets;
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public List<string> GetExcelSheetNames(string fileName)
        {
            List<string> sheets = new List<string>();
            IWorkbook workBook;
            ISheet sheet;
            IFormulaEvaluator formula;
            try
            {
                using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    try
                    {
                        if (fileName.EndsWith(".xls", StringComparison.InvariantCultureIgnoreCase))
                        {
                            workBook = new HSSFWorkbook(file);
                            formula = workBook.GetCreationHelper().CreateFormulaEvaluator();//new (workBook);
                        }
                        else
                        {
                            workBook = new XSSFWorkbook(file);
                            formula = workBook.GetCreationHelper().CreateFormulaEvaluator(); // new XSSFFormulaEvaluator(workBook);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("File is Exclusively opened by the user. Please close the excel before using the application");
                    }
                }

                for (int i = 0; i < workBook.NumberOfSheets; i++)
                {
                    sheets.Add(workBook.GetSheetName(i));
                }

                return sheets;
            }
            catch (Exception ex)
            {
                throw;
            }




        }
    }
}
