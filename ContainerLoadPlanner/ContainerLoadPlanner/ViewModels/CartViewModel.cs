using Caliburn.Micro;
using ContainerLoadPlanner.Models;
using SharedEntities;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerLoadPlanner.ViewModels
{
    public class CartViewModel<T> : Screen where T : class
    {
        private readonly Dictionary<string, List<Container<T>>> _data;
        private string _selectedDestination;
        private BindableCollection<ContainerViewModel<T>> _selectedContainers;

        public BindableCollection<ContainerViewModel<T>> SelectedContainers
        {
            get { return _selectedContainers; }
            set { _selectedContainers = value; NotifyOfPropertyChange(() => SelectedContainers); }
        }

        public CartViewModel(Dictionary<string,List<Container<T>>> data)
        {
            _data = data;
            Destinations = new BindableCollection<string>(data.Keys);
        }
        public BindableCollection<string> Destinations { get; }

        public string SelectedDestination
        {
            get { return _selectedDestination; }
            set { _selectedDestination = value; 
                NotifyOfPropertyChange(() => SelectedDestination);
                LoadContainersForDestination(_selectedDestination);
            }
            
        }

        private void LoadContainersForDestination(string destination)
        {
            if (_data.ContainsKey(destination))
            {
                SelectedContainers = new BindableCollection<ContainerViewModel<T>>(
                    _data[destination].Select(container => new ContainerViewModel<T>(container)));
            }
            else
            {
                SelectedContainers = null;
            }
        }

    }
}
