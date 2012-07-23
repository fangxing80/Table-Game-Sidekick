using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.Views
{
    public interface IView<TViewModel> : IViewOutput<TViewModel>, IViewInput<TViewModel>
    {

    }


    public interface IViewInput<in TViewModel>
    {
        TViewModel ViewModel { set; }
    }


    public interface IViewOutput<out TViewModel>
    {
        TViewModel ViewModel { get;  }
    }

}
