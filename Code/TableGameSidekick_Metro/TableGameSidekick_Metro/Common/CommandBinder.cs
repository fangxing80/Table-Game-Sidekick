using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Windows.Input;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Controls;
namespace TableGameSidekick_Metro.Common
{
        //Code Sample
        //<Button>
        //    <common:CommandBinder.CommandBinder>
        //        <common:CommandBinder 
        //            Parameter="{Binding ElementName=textBox}" 
        //            Command="{StaticResource command}"  
        //            EventName="Click"  />
        //    </common:CommandBinder.CommandBinder>            
        //</Button>

    public class CommandBinderParameter
    {
        public DependencyObject SourceObject { get; set; }
        public Object Paremeter { get; set; }
        public string EventName { get; set; }
        public object EventArgs { get; set; }
    
    }
    public class CommandBinder : DependencyObject
    {


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandBinder), new PropertyMetadata(null));




        public Object Parameter
        {
            get { return (Object)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Parameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof(Object), typeof(CommandBinder), new PropertyMetadata(null));



        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EventName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(CommandBinder), new PropertyMetadata(""));


        static Dictionary<Type, Dictionary<string, EventInfo>> TypeEventDic
            = new Dictionary<Type, Dictionary<string, EventInfo>>();


        public static CommandBinder GetCommandBinder(DependencyObject obj)
        {
            return (CommandBinder)obj.GetValue(CommandBinderProperty);
        }

        public static void SetCommandBinder(DependencyObject obj, CommandBinder value)
        {
            obj.SetValue(CommandBinderProperty, value);
        }

        // Using a DependencyProperty as the backing store for CommandBinder.  This enables animation, styling, binding, etc...


        public static readonly DependencyProperty CommandBinderProperty =
            DependencyProperty.RegisterAttached("CommandBinder", typeof(CommandBinder), typeof(DependencyObject), new PropertyMetadata(
                null,
                    (d, v) =>
                    {

                        TryBind(d);
                    }
                ));


        public static void TryBind(DependencyObject d)
        {
            var t = d.GetType();
            var cb = (CommandBinder)d.GetValue(CommandBinderProperty);
            while (t != null)
            {
                Dictionary<string, EventInfo> es;
                if (!TypeEventDic.TryGetValue(t, out es))
                {
                    es = t.GetRuntimeEvents()
                        .ToDictionary(x => x.Name, x => x);
                    TypeEventDic[t] = es;
                }
                EventInfo eventInfo;
                if (es.TryGetValue(cb.EventName, out eventInfo))
                {
                    var r = new RoutedEventHandler(
                        (o, e) =>
                            (
                                (ICommand)cb.GetValue(CommandProperty))
                                    .Execute(
                                       new CommandBinderParameter { EventArgs = e, EventName = cb.EventName , Paremeter =cb.Parameter, SourceObject =d}
                                    )
                            );

                    WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>
                        (e => (EventRegistrationToken)eventInfo.AddMethod.Invoke(d, new object[] { e }),
                          et => eventInfo.RemoveMethod.Invoke(d, new object[] { et }),
                          r);



                    ////eventInfo.AddEventHandler(
                    ////    d,

                    ////    );
                    return;
                }
                t = t.GetTypeInfo().BaseType;
            }



        }
    }
}
