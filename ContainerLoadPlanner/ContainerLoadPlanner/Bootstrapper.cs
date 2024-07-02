using Caliburn.Micro;
using ContainerLoadPlanner.Utilities;
using ContainerLoadPlanner.ViewModels;
using LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TescoClpBackend.ClpLogics;

namespace ContainerLoadPlanner
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;
        public Bootstrapper()
        {
            Initialize();
        }
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            DisplayRootViewForAsync<MainViewModel>();
        }
        protected override void Configure()
        {
            _container = new SimpleContainer();
            _container.Instance(_container);
            _container.Singleton<ClientViewModel, TescoViewModel>("Tesco");
            _container.Singleton<ClientViewModel, TargetViewModel>("Target");
            _container.Singleton<LoggerManager>();
            _container.AddExcelServices();
            _container.AddSharedServices();
            _container.AddExcelWriterService<ClpItem>();
            _container.AddTescoServices();
            _container.Singleton<IWindowManager, WindowManager>();
            _container.RegisterPerRequest(typeof(MainViewModel), null, typeof(MainViewModel));

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }


    }
}
