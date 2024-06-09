using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerLoadPlanner.ViewModels
{
    public class MainViewModel : Conductor<Screen>
    {
        public MainViewModel(SimpleContainer container)
        {
            this.container = container;
            Clients = new List<string>() { "Tesco", "Target" };
        }
        private string selectedClient;
        private readonly SimpleContainer container;

        public string SelectedClient
        {
            get { return selectedClient; }
            set { selectedClient = value; NotifyOfPropertyChange(() => SelectedClient); }
        }
        private List<string> clients;

        public List<string> Clients
        {
            get { return clients; }
            set { clients = value; NotifyOfPropertyChange(() => Clients); }
        }


        public void LoadClpProcessor()
        {
            if(SelectedClient != null)
            {
                ActivateItemAsync(container.GetInstance<ClientViewModel>(SelectedClient));
            }
            
        }
    }
}
