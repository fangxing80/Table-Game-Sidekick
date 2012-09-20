using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MVVMSidekick.Reactive;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using TableGameSidekick_Metro.Common;
using System.Reactive.Concurrency;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TableGameSidekick_Metro.Controls
{
    /// <summary>
    /// 一个通过扩展属性附加到界面上 且与Textbox绑定的数字编辑器
    /// </summary>
    public sealed partial class CalcNumberPad : UserControl
    {
        public CalcNumberPad()
        {
            this.InitializeComponent();
            ViewModel.GetValueContainer<Visibility>("Visibility")
                .GetValueChangeObservable()
                .Subscribe(x =>
                    Visibility = x.EventArgs);
            ViewModel.InputFinished += ViewModel_InputFinished;
        }

        void ViewModel_InputFinished(object sender, EventArgs e)
        {
            if (e != null)
            {
                ValueTarget.Text = Double.Parse(this.ViewModel.ShowString).ToString();
            }

            this.Reset();
        }

        public void Reset()
        {

            ViewModel.Visibility = Visibility.Collapsed;
            // ViewModel.ActualInputChars.Clear();

        }

        public TextBox ValueTarget { get; set; }

        public CalcNumberPad_Model ViewModel
        {
            get
            {
                return m_InputPanel.DataContext as CalcNumberPad_Model;
            }
            set
            {
                m_InputPanel.DataContext = value;
            }
        }
        public ICommand ShowInputCommand
        {
            get { return ViewModel.ShowInputCommand; }

        }



        public static CalcNumberPad GetCalcNumberPad(DependencyObject obj)
        {
            return (CalcNumberPad)obj.GetValue(CalcNumberPadProperty);
        }

        public static void SetCalcNumberPad(DependencyObject obj, CalcNumberPad value)
        {
            obj.SetValue(CalcNumberPadProperty, value);
        }


        public static readonly DependencyProperty CalcNumberPadProperty =
            DependencyProperty.RegisterAttached("CalcNumberPad", typeof(CalcNumberPad), typeof(CalcNumberPad),new PropertyMetadata(null,
                (o, e) =>
                {
                    var itm = o as Panel;
                    if (itm != null)
                    {
                        var np = e.NewValue as CalcNumberPad;
                        itm.Children.Add(np);
                    }
                }

                ));


        public static bool GetUseCalcPad(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseCalcPadProperty);
        }

        public static void SetUseCalcPad(DependencyObject obj, bool value)
        {
            obj.SetValue(UseCalcPadProperty, value);
        }

        // Using a DependencyProperty as the backing store for UseCalcPad.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseCalcPadProperty =
            DependencyProperty.RegisterAttached("UseCalcPad", typeof(bool), typeof(CalcNumberPad), new PropertyMetadata(false,
                  (o, e) =>
                  {
                      if (e.OldValue != null || (bool)e.NewValue == true)
                      {
                          var tb = o as TextBox;
                          if (tb != null)
                          {
                              // var pad = (e.NewValue as Panel).GetValue(CalcNumberPadProperty) as CalcNumberPad;
                              tb.GotFocus +=
                                  (object sender, RoutedEventArgs ev) =>
                                  {
                                      CalcNumberPad_Model.ShowCommand(tb);

                                  };

                          }

                      }

                  }));







    }



    public class CalcNumberPad_Model : ViewModelBase<CalcNumberPad_Model>
    {
        static readonly string DefaultValue = "0";
        public CalcNumberPad_Model()
        {
            ConfigProperties();
            ConfigCommands();
        }

        void ConfigProperties()
        {
            //每次有内容修改
            var obsCol = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
                (
                    ev => this.ActualInputChars.CollectionChanged += ev,
                    ev => this.ActualInputChars.CollectionChanged -= ev
                );

            obsCol

                .Do //进行验证并且把结果输出到 ShowString属性
                (
                     e =>
                     {
                         //TODO:验证逻辑
                         var str = new string(ActualInputChars.ToArray());
                         double v;
                         if (!double.TryParse(str, out v))
                         {
                             if (this.ActualInputChars.Count > 0)
                             {
                                 this.ActualInputChars.RemoveAt(this.ActualInputChars.Count - 1);
                             }
                             else
                             {
                                 str = DefaultValue;
                             }
                         }
                         str = new string(ActualInputChars.ToArray());
                         str = str.TrimStart('0');
                         if (string.IsNullOrEmpty(str))
                         {
                             str = DefaultValue;
                         }
                         if (str.StartsWith("."))
                         {
                             str = "0" + str;
                         }
                         //验证后显示

                         GetValueContainer(x => x.ShowString).SetValueAndTryNotify(str);

                     }
                )
                .Subscribe()
                .RegisterDisposeToViewModel(this);


        }

        void ConfigCommands()
        {

            #region 所有输入按钮

            var btnClick = this.ButtonPushCommand.CommandCore
                .Select(e => e.EventArgs.Parameter as string)
                .Where(s => s != null);

            btnClick.Where(x => x.Length == 1)
                .Subscribe
                (
                    input =>
                    {
                        ActualInputChars.Add(input[0]);
                    }

                ).RegisterDisposeToViewModel(this);

            btnClick.Where(x => x.Length != 1)
                .Subscribe
                (
                    input =>
                    {
                        switch (input)
                        {

                            case "Back":
                                if (ActualInputChars.Count > 0)
                                {
                                    ActualInputChars.RemoveAt(ActualInputChars.Count - 1);
                                }
                                break;
                            case "Cancel":

                                InputFinished(null, null);
                                break;

                            case "Clear":
                                this.ActualInputChars.Clear();
                                foreach (var c in DefaultValue)
                                {
                                    ActualInputChars.Add(c);
                                }
                                break;
                            case "Enter":

                                if (InputFinished != null)
                                {
                                    InputFinished(null, EventArgs.Empty);
                                }

                                break;
                            default:
                                break;
                        }
                    }

                ).RegisterDisposeToViewModel(this);
            #endregion

            #region 绑定到 Textbox上的命令
            this.ShowInputCommand.CommandCore
                .Subscribe(
                  e =>
                  {

                      // var pm = e.EventArgs.Parameter as CommandBinderParameter;

                      var fel = e.EventArgs.Parameter as FrameworkElement;

                      ShowCommand(fel as TextBox);

                  }
                ).RegisterDisposeToViewModel(this);
            #endregion
        }

        public static void ShowCommand(TextBox eventSource)
        {
            CalcNumberPad calc = null;
            FrameworkElement elem = eventSource;
            while (elem != null)
            {
                calc = elem.GetValue(CalcNumberPad.CalcNumberPadProperty) as CalcNumberPad;
                if (calc != null)
                {
                    break;
                }
                elem = elem.Parent as FrameworkElement;
            }


            if (calc != null)
            {
                calc.ValueTarget = eventSource;
                calc.ViewModel.ActualInputChars.Clear();
                foreach (var item in calc.ValueTarget.Text)
                {
                    calc.ViewModel.ActualInputChars.Add(item);
                }
                calc.ViewModel.Visibility = Visibility.Visible;
            }
        }

        // bool _raiseCollectionChangedEvent = true;

        public String ShowString
        {
            get { return m_ShowStringLocator(this).Value; }
            set
            {
                //   _raiseCollectionChangedEvent = false;
                m_ShowStringLocator(this).Value = value;
                ActualInputChars.Clear();
                foreach (var c in DefaultValue)
                {
                    ActualInputChars.Add(c);
                }
                //  _raiseCollectionChangedEvent = true;
            }
        }

        #region Property String ShowString Setup
        protected Property<String> m_ShowString =
          new Property<String> { LocatorFunc = m_ShowStringLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<ViewModelBase, ValueContainer<String>> m_ShowStringLocator =
            RegisterContainerLocator<String>(
                "ShowString",
                model =>
                {
                    model.m_ShowString =
                        model.m_ShowString
                        ??
                        new Property<String> { LocatorFunc = m_ShowStringLocator };
                    return model.m_ShowString.Container =
                        model.m_ShowString.Container
                        ??
                        new ValueContainer<String>("ShowString", model, DefaultValue);
                });
        #endregion




        public ObservableCollection<Char> ActualInputChars
        {
            get { return m_ActualInputCharsLocator(this).Value; }

        }

        #region Property  ObservableCollection<Char> ActualInputChars Setup
        protected Property<ObservableCollection<Char>> m_ActualInputChars =
          new Property<ObservableCollection<Char>> { LocatorFunc = m_ActualInputCharsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<ViewModelBase, ValueContainer<ObservableCollection<Char>>> m_ActualInputCharsLocator =
            RegisterContainerLocator<ObservableCollection<Char>>(
                "ActualInputChars",
                model =>
                {
                    model.m_ActualInputChars =
                        model.m_ActualInputChars
                        ??
                        new Property<ObservableCollection<Char>> { LocatorFunc = m_ActualInputCharsLocator };
                    return model.m_ActualInputChars.Container =
                        model.m_ActualInputChars.Container
                        ??
                        new ValueContainer<ObservableCollection<Char>>("ActualInputChars", model, new ObservableCollection<char>(DefaultValue.ToCharArray()));
                });
        #endregion


        public Visibility Visibility
        {
            get { return m_VisibilityLocator(this).Value; }
            set { m_VisibilityLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property Visibility Visibility Setup
        protected Property<Visibility> m_Visibility =
          new Property<Visibility> { LocatorFunc = m_VisibilityLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<ViewModelBase, ValueContainer<Visibility>> m_VisibilityLocator =
            RegisterContainerLocator<Visibility>(
                "Visibility",
                model =>
                {
                    model.m_Visibility =
                        model.m_Visibility
                        ??
                        new Property<Visibility> { LocatorFunc = m_VisibilityLocator };
                    return model.m_Visibility.Container =
                        model.m_Visibility.Container
                        ??
                        new ValueContainer<Visibility>("Visibility", model, Visibility.Collapsed);
                });
        #endregion






        /// <summary>
        /// 任意的Button Push，按钮的CommandName用 Parameter传递
        /// </summary>
        public ICommandModel<ReactiveCommand, String> ButtonPushCommand
        {
            get { return m_ButtonPushCommand.WithViewModel(this); }
            protected set { m_ButtonPushCommand = (CommandModel<ReactiveCommand, String>)value; }
        }

        #region ButtonPushCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_ButtonPushCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel(default(String));
        #endregion





        public ICommandModel<ReactiveCommand, String> ShowInputCommand
        {
            get { return m_ShowInputCommand.WithViewModel(this); }
            protected set { m_ShowInputCommand = (CommandModel<ReactiveCommand, String>)value; }
        }

        #region ShowInputCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_ShowInputCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel(default(String));
        #endregion



        public event EventHandler InputFinished;

    }
}
