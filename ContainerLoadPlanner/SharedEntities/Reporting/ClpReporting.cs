using ExcelWriterNetFramework;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedEntities.Reporting
{
    public class ClpReporting<T> : BaseExcelWriter where T : class
    {
        public void CreateReport(string fileName, Dictionary<string, List<Container<T>>> data)
        {
            foreach (var destination in data)
            {
                CreateOrOpenWorkbook(fileName, destination.Key, overWrite: true);

                CreateDestinationSummary(destination.Value);
                int i = 10;
                foreach (Container<T> container in destination.Value)
                {

                    CreateHeader(ref i);
                    foreach (var item in container.Items)
                    {
                        CreateData(item, ref i);
                    }
                    CreateSummary(ref i, container);
                    i++;
                }
                Save(fileName, true);
            }
            Save(fileName, true);
        }

        private void CreateDestinationSummary(List<Container<T>> value)
        {
            var groups=value.GroupBy(a => a.Label);
            int i = 2;
            foreach (var label in groups)
            {
                var row = sheet.CreateRow(i);
                var cell=row.CreateCell(0);
                cell.SetCellValue($"{label.Count()}X{label.Key}");
            }
        }

        private void CreateSummary(ref int i, Container<T> container)
        {
           var summaryRow=sheet.CreateRow(i);
            int cellNo = 0;
            foreach (var property in typeof(T).GetProperties())
            {
                if (IsNumericType(property.PropertyType)){
                    var sum=GetSumOfDoubleProperties(container.Items, property.Name);
                    CreateCell(sheet,i, cellNo, sum);
                }
                cellNo++;
            }
            i++;
        }

        public double GetSumOfDoubleProperties(List<T>items,string name)
        {
            double sum = 0.0;

            // Iterate through each item in Items
            foreach (T item in items)
            {
                // Use reflection to get all properties of type double
                PropertyInfo[] properties = typeof(T).GetProperties()
                                                    .Where(p => p.Name == name)
                                                    .ToArray();

                // Sum up the values of all double properties
                foreach (PropertyInfo prop in properties)
                {
                    var value = prop.GetValue(item);
                    double.TryParse(value.ToString(),out var v);
                    sum += v;
                }
            }

            return sum;
        }
        private static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        private void CreateData(T item, ref int j)
        {
            var doubleCellStyle = SetDoubleCellStyle();
            var intCellStyle = SetIntCellStyle();

            var dateCellStyle = SetDateCellStyle();
            var stringCellStyle = SetDataCellStyle();

            int rowIndex = j;// dataStartingIndex;
            IRow row = sheet.CreateRow(rowIndex);
            int i = 0;
            foreach (var property in item.GetType().GetProperties())
            {
                ICell cell;
                var type = property.PropertyType;
                var value = property.GetValue(item, null);
                if (type.Equals(typeof(int)))
                {
                    cell = row.CreateCell(i, CellType.Numeric);
                    cell.CellStyle = intCellStyle;
                    cell.SetCellValue(Convert.ToInt32(value));
                }
                if (type.Equals(typeof(double)))
                {
                    cell = row.CreateCell(i, CellType.Numeric);
                    cell.CellStyle = doubleCellStyle;
                    cell.SetCellValue(Convert.ToDouble(value));
                }
                else if (type.Equals(typeof(DateTime)))
                {
                    cell = row.CreateCell(i, CellType.Numeric);
                    cell.CellStyle = dateCellStyle;
                    if (!Convert.ToDateTime(value).Equals(DateTime.MinValue))
                    { cell.SetCellValue(Convert.ToDateTime(value)); }
                }
                else
                {
                    cell = row.CreateCell(i, CellType.String);
                    cell.CellStyle = stringCellStyle;
                    cell.SetCellValue(Convert.ToString(value));
                }



                i++;
            }
            j++;
        }

        private void CreateHeader(ref int j)
        {
            var style = SetHeaderCellStyle();
            var row = sheet.CreateRow(j);
            int i = 0;
            
            foreach (var property in typeof(T).GetProperties())
            {
                ICell cell = row.CreateCell(i, CellType.String);
                cell.CellStyle = style;
                Attribute[] attributes = Attribute.GetCustomAttributes(property);
                if (attributes.Length > 0)
                {
                    var attribute = attributes[0];
                    if (attribute is ColumnHeaderAttribute)
                    {
                        var header = ((ColumnHeaderAttribute)attribute).Header;
                        cell.SetCellValue(header);
                    }
                    else
                    {
                        cell.SetCellValue(property.Name);
                    }
                }
                else
                {
                    cell.SetCellValue(property.Name);
                }
                i++;
            }
            j++;
        }
    }
}
