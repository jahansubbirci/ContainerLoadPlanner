using Caliburn.Micro;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using TescoClpBackend.ClpLogics;

namespace ContainerLoadPlanner.ViewModels
{
    public class ContainerViewModel<T>:Screen where T:class//GenericViewModel<Container<ClpItem>>
    {

        public Container<T> Container { get;  }
        
        public ContainerViewModel(Container<T> container)
        {
            Container = container;
            this.Items=container.Items;
            TotalCbm = container.UsedCbm;
            

        }

        public  T SetSummary(List<T> items) 
        {
            if (items == null || items.Count == 0)
                return default;

            T summaryInstance = Activator.CreateInstance<T>();

            // Get all properties of the type T
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (IsCustomObjectType(property.PropertyType))
                {
                    Activator.CreateInstance(property.PropertyType);
                    
                }
                // Check if the property is numeric
                if (IsNumericType(property.PropertyType))
                {
                    // Sum the values of this property for all items
                    var sum = items.Sum(item => Convert.ToDouble(property.GetValue(item) ?? 0));

                    // Set the summed value to the property of the summary instance
                    property.SetValue(summaryInstance, Convert.ChangeType(sum, property.PropertyType));
                }
            }

            return summaryInstance;
        }

        private  bool IsCustomObjectType(Type type)
        {
            if( type.IsClass && type != typeof(string))
            {
                return true;   
            }return false;
        }

       public void GetNumericProperty(Type t)
        {
            if (IsCustomObjectType(t))
            {
                var x=Activator.CreateInstance(t);
                foreach (var property in x.GetType().GetProperties())
                {
                    GetNumericProperty(property.PropertyType);
                }
            }
           
        }
        private  bool IsNumericType(Type type)
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

        public List<T> Items { get; private set; }
        public double TotalCbm { get; private set; }
      
    }
}
