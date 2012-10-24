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
        public Task FrameNavigate(string targetViewName, Dictionary<string, object> parameters = null)
        {

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
            TResult result = default(TResult);

            var arg = new NavigateCommandEventArgs()
            {
                ParameterDictionary = parameters,
                TargetViewId = targetViewName,
                TargetFrame = m_Frame,
            };

            Task<TResult> task = new Task<TResult>(() => result);
            Action<LayoutAwarePage> finishNavigateAction =
                page =>
                {
                    result = page.GetResult<TResult>();
                    task.Start();
                };
            arg.ParameterDictionary[NavigateParameterKeys.FinishedCallback] = finishNavigateAction;
            m_EventRouter.RaiseEvent<NavigateCommandEventArgs>(arg.ViewModel, arg);

            return task;

        }
    }
}
