using MVVMSidekick.EventRouter;
using MVVMSidekick.ViewModels;
using MVVMSidekick.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace TableGameSidekick_Metro.Common
{
    public class FrameNavigator : IFrameNavigator
    {
        public FrameNavigator(Frame frame, EventRouter eventRouter)
        {
            m_Frame = frame;
            m_EventRouter = eventRouter;
        }
        Frame m_Frame;
        EventRouter m_EventRouter;
        void CheckParametersNull(ref  Dictionary<string, object> parameters)
        {
            if (parameters==null)
            {
                parameters = new Dictionary<string, object>();
            }
        
        }

        public Task FrameNavigate(string targetViewName, Dictionary<string, object> parameters = null)
        {
            CheckParametersNull(ref parameters);
            var arg = new NavigateCommandEventArgs()
            {
                ParameterDictionary = parameters,
                TargetViewId = targetViewName,
                TargetFrame = m_Frame,
            };

            Task task = new Task(() => { });
            Action<LayoutAwarePage> finishNavigateAction =
                page =>
                {

                    task.Start();
                };
            arg.ParameterDictionary[NavigateParameterKeys.FinishedCallback] = finishNavigateAction;
            m_EventRouter.RaiseEvent<NavigateCommandEventArgs>(arg.ViewModel, arg);

            return task;

        }


        public Task<TResult> FrameNavigate<TResult>(string targetViewName, Dictionary<string, object> parameters = null)
        {
            CheckParametersNull(ref parameters);

            var arg = new NavigateCommandEventArgs()
            {
                ParameterDictionary = parameters,
                TargetViewId = targetViewName,
                TargetFrame = m_Frame,
            };

            TResult result = default(TResult);

            Task<TResult> taskR = new Task<TResult>(() => result);
            Action<LayoutAwarePage> finishNavigateAction =
                page =>
                {
                    result = page.GetResult<TResult>();
                    taskR.Start();
                };
            arg.ParameterDictionary[NavigateParameterKeys.FinishedCallback] = finishNavigateAction;



            m_EventRouter.RaiseEvent<NavigateCommandEventArgs>(arg.ViewModel, arg);

            return taskR;

        }


        public Task<TViewModel> FrameNavigateAndGetViewModel<TViewModel>(string targetViewName, Dictionary<string, object> parameters = null) where TViewModel : IViewModelBase
        {
            CheckParametersNull(ref parameters);
            var arg = new NavigateCommandEventArgs()
            {
                ParameterDictionary = parameters,
                TargetViewId = targetViewName,
                TargetFrame = m_Frame,
            };


            TViewModel viewModel = default(TViewModel);

   
            Task<TViewModel> taskVm = new Task<TViewModel>(() => viewModel);
            Action<LayoutAwarePage> navigateToAction =
                page =>
                {
                    viewModel =(TViewModel) page.DefaultViewModel ;
                    taskVm.Start();
                };
            arg.ParameterDictionary[NavigateParameterKeys.NavigateToCallback] = navigateToAction;
   

            
            
            m_EventRouter.RaiseEvent<NavigateCommandEventArgs>(arg.ViewModel, arg);
            return taskVm;
        }

        public NavigateResult<TViewModel, TResult> FrameNavigateAndGetViewModel<TViewModel, TResult>(string targetViewName, Dictionary<string, object> parameters = null)
        {
            CheckParametersNull(ref parameters);
            var arg = new NavigateCommandEventArgs()
            {
                ParameterDictionary = parameters,
                TargetViewId = targetViewName,
                TargetFrame = m_Frame,
            };


            TViewModel viewModel = default(TViewModel);
            Task<TViewModel> taskVm = new Task<TViewModel>(() => viewModel);
            Action<LayoutAwarePage> navigateToAction =
                page =>
                {
                    viewModel = (TViewModel)page.DefaultViewModel;
                    taskVm.Start();
                };
            arg.ParameterDictionary[NavigateParameterKeys.NavigateToCallback] = navigateToAction;


            TResult result = default(TResult);
            Task<TResult> taskR = new Task<TResult>(() => result);
            Action<LayoutAwarePage> finishNavigateAction =
                page =>
                {
                    result = page.GetResult<TResult>();
                    taskR.Start();
                };
            arg.ParameterDictionary[NavigateParameterKeys.FinishedCallback] = finishNavigateAction;



            m_EventRouter.RaiseEvent<NavigateCommandEventArgs>(arg.ViewModel, arg);
            return new NavigateResult<TViewModel, TResult> {
                 ViewModel=taskVm ,
                 Result=taskR            
            };
        }


        public bool GoForward()
        {
            var can = m_Frame.CanGoForward;
            if (can)
            {
                m_Frame.GoForward();
            }
            return can;
        }

        public bool GoBack()
        {
            var can = m_Frame.CanGoBack ;
            if (can)
            {
                m_Frame.GoBack();
            }
            return can;
        }

    
    }
}
