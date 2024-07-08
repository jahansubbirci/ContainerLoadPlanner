using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerLoadPlanner.ViewModels
{
    public class TargetViewModel : ClientViewModel
    {
        private readonly SimpleContainer container;

        public TargetViewModel(SimpleContainer container) : base(container)
        {
            this.container = container;
        }
        private string _textBoxText;

        public string TextBoxText
        {
            get { return _textBoxText; }
            set
            {
                _textBoxText = value;
                NotifyOfPropertyChange(() => TextBoxText);
            }
        }
        public void Create()
        {
            var x = TextBoxText;
        }
    }
}
