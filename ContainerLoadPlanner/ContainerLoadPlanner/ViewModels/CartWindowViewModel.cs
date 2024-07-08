using Caliburn.Micro;
using ContainerLoadPlanner.Models;
using ContainerLoadPlanner.Views;
using SharedEntities;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerLoadPlanner.ViewModels
{
    public class CartWindowViewModel<T> : Screen where T : class
    {

        //    public readonly Dictionary<string, List<Container<T>>> data;

        //    private string selectedDestination;
        //    private BindableCollection<CartViewModel<T>> _selectedData;

        //    public BindableCollection<string> Destinations { get; }

        //    public BindableCollection<CartViewModel<T>> SelectedData
        //    {
        //        get => _selectedData; private set
        //        {
        //            _selectedData = value;
        //            NotifyOfPropertyChange(() => SelectedData);
        //        }
        //    }



        //    public string SelectedDestination
        //    {
        //        get { return selectedDestination; }
        //        set
        //        {
        //            selectedDestination = value; NotifyOfPropertyChange(() => SelectedDestination);
        //            LoadContainersForDestination(selectedDestination);
        //        }
        //    }

        //    private void LoadContainersForDestination(string selectedDestination)
        //    {
        //        if (data.ContainsKey(selectedDestination))
        //        {

        //            Carts = new BindableCollection<CartViewModel<T>>();
        //            var destinationContainers=data[selectedDestination];
        //            SelectedData = new BindableCollection<CartViewModel<T>>();
        //            SelectedData.Add(new CartViewModel<T>(destinationContainers))   ;

        //        }
        //        else
        //        {

        //        }
        //    }

        //    public BindableCollection<CartViewModel<T>> Carts { get; set; }


        //    public CartWindowViewModel(Dictionary<string, List<Container<T>>> data)
        //    {
        //        this.data = data;
        //        this.Destinations = new BindableCollection<string>(data.Keys);
        //        //Carts = new BindableCollection<CartViewModel<T>>();

        //        //foreach (var key in data.Keys)
        //        //{
        //        //    Carts.Add(new CartViewModel<T>(key, data[key]));
        //        //}
        //    }
        //}
    }
}
