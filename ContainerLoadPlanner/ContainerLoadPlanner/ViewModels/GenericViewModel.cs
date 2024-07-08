using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerLoadPlanner.ViewModels
{
    public class GenericViewModel<T>:Screen
    {
		private T _model;

		public T Model
		{
			get { return _model; }
			set { _model = value;
				NotifyOfPropertyChange(()=>Model);
			}
		}
        public GenericViewModel(T model)
        {
            Model = model;
        }
		public GenericViewModel()
		{

		}

    }
}
