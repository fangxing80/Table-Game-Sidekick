using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System.Collections.Concurrent;
using MVVM.ViewModels;
using System.Dynamic;

using MVVM.Commands;

#if NETFX_CORE
// Summary:
//     Provides the functionality to offer custom error information that a user
//     interface can bind to.
namespace System.ComponentModel
{
    public interface IDataErrorInfo
    {
        // Summary:
        //     Gets an error message indicating what is wrong with this object.
        //
        // Returns:
        //     An error message indicating what is wrong with this object. The default is
        //     an empty string ("").
        string Error { get; }

        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // Parameters:
        //   columnName:
        //     The name of the property whose error message to get.
        //
        // Returns:
        //     The error message for the property. The default is an empty string ("").
        string this[string columnName] { get; }
    }
}
#endif





namespace MVVM.ViewModels
{


    public class DefaultViewModel : ViewModelBase<DefaultViewModel>
    {

    }

    [DataContract]
    public abstract class ViewModelBase
        : IDisposable, INotifyPropertyChanged, IDataErrorInfo
    {


        public abstract IEnumerable<string> FieldNames { get; }

        public abstract object this[string colName] { get; set; }

        private List<Action> _disposeActions = new List<Action>();

        /// <summary>
        /// 注册在销毁时需要做的操作
        /// </summary>
        /// <param name="newAction">新操作</param>
        protected void AddDisposeAction(Action newAction)
        {
            List<Action> disposeActions;

            if ((disposeActions = _disposeActions) != null)
            {
                disposeActions.Add(newAction);
            }

        }


        /// <summary>
        /// 注册销毁
        /// </summary>
        /// <param name="newAction">新操作</param>
        internal protected void AddDisposable(IDisposable item)
        {
            AddDisposeAction(() => item.Dispose());
        }



        /// <summary>
        /// 销毁，尝试运行所有注册的销毁操作
        /// </summary>
        public void Dispose()
        {

            var lst = Interlocked.Exchange(ref _disposeActions, null);
            if (lst != null)
            {
                var exlst =
                    lst.Select
                    (
                      action =>
                      {

                          if (action != null)
                              try
                              {
                                  action();
                              }
                              catch (Exception ex)
                              {

                                  return ex;
                              }
                          return null;
                      }
                    )
                    .ToList();

                OnDisposeExceptions(exlst);
            }


        }

        /// <summary>
        /// 指定如何处理在dispose时出现的错误
        /// </summary>
        /// <param name="exceptions"></param>
        protected virtual void OnDisposeExceptions(IList<Exception> exceptions)
        {

        }



        public event PropertyChangedEventHandler PropertyChanged;




        public abstract string Error { get; protected set; }




        string IDataErrorInfo.this[string columnName]
        {
            get { return GetColumnError(columnName); }
        }

        protected abstract string GetColumnError(string columnName);




        internal void RaisePropertyChanged(Lazy<PropertyChangedEventArgs> lazyEAFactory)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, lazyEAFactory.Value);
            }

        }
    }
    public static class DisposableExtensions
    {
        public static void RegisterDispose(this IDisposable item, ViewModelBase vm)
        {
            vm.AddDisposable(item);
        }
    }
    public class Property<TProperty>
    {
        public Property(Func<ViewModelBase, ValueContainer<TProperty>> locatorFunc)
        {
            m_LocatorFunc = locatorFunc;
        }

        public ValueContainer<TProperty> Locate(ViewModelBase viewModel)
        {

            return m_LocatorFunc(viewModel);
        }


        Func<ViewModelBase, ValueContainer<TProperty>> m_LocatorFunc;


        public ValueContainer<TProperty> Container
        {
            get;
            set;
        }

    }
    public class ValueContainer<TProperty> : IErrorInfo, IValueCanSet<TProperty>, IValueCanGet<TProperty>, IPropertyContainer
    {



        public event EventHandler<ValueChangedEventArgs<TProperty>> ValueChanged = (o, e) => { };

        /// <summary>
        /// 创建属性值容器
        /// </summary>
        /// <param name="info">属性名</param>
        public ValueContainer(string info, ViewModelBase vm)
            : this(info, (v1, v2) => v1.Equals(v2), default(TProperty), vm)
        {
        }

        /// <summary>
        /// 创建属性值容器
        /// </summary>
        /// <param name="info">属性名</param>
        /// <param name="equalityComparer">判断两个值是否相等的比较器</param>
        public ValueContainer(string info, Func<TProperty, TProperty, bool> equalityComparer, ViewModelBase vm)
            : this(info, equalityComparer, default(TProperty), vm)
        {
        }

        /// <summary>
        /// 创建属性值容器
        /// </summary>
        /// <param name="info">属性名</param>
        /// <param name="initValue">初始值</param>
        public ValueContainer(string info, TProperty initValue, ViewModelBase vm)
            : this(info, (v1, v2) => v1.Equals(v2), initValue, vm)
        {
        }

        /// <summary>
        /// 创建属性值容器
        /// </summary>
        /// <param name="info">属性名</param>
        /// <param name="equalityComparer">判断两个值是否相等的比较器</param>
        /// <param name="initValue">初始值</param>
        public ValueContainer(string info, Func<TProperty, TProperty, bool> equalityComparer, TProperty initValue, ViewModelBase vm)
        {
            EqualityComparer = equalityComparer;
            PropertyName = info;
            _value = initValue;

            PropertyType = typeof(TProperty);
            ViewModel = vm;
            // _eventObject = new ValueChangedEventObject<TProperty>(this);
        }

        /// <summary>
        /// 判断两个值是否相等的比较器
        /// </summary>
        public Func<TProperty, TProperty, bool> EqualityComparer { get; private set; }

        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 内部值
        /// </summary>
        TProperty _value;

        /// <summary>
        /// 值
        /// </summary>

        public TProperty Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// 保存值并且触发更改事件
        /// </summary>
        /// <param name="objectInstance">属性所在的ViewModel</param>
        /// <param name="value">属性值</param>
        public void SetValueAndTryNotify(TProperty value)
        {
            InternalPropertyChange(this.ViewModel, value, ref _value, PropertyName);
        }

        /// <summary>
        /// 单纯保存值
        /// </summary>
        /// <param name="value">新值</param>
        public void SetValue(TProperty value)
        {
            _value = value;
        }


        //private ValueChangedEventObject<TProperty> _eventObject;

        //public ValueChangedEventObject<TProperty> EventObject
        //{
        //    get { return _eventObject; }

        //}



        /// <summary>
        /// 保存值并且触发更改事件
        /// </summary>
        /// <param name="objectInstance">属性所在的ViewModel</param>
        /// <param name="newValue">新值</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="message">属性名</param>
        void InternalPropertyChange(ViewModelBase objectInstance, TProperty newValue, ref TProperty currentValue, string message)
        {
            var changing = (this.EqualityComparer == null) ?
                !this.EqualityComparer(newValue, currentValue) :
                !Object.Equals(newValue, currentValue);


            if (changing)
            {
                var oldvalue = currentValue;
                currentValue = newValue;


                Lazy<PropertyChangedEventArgs> lz
                    = new Lazy<PropertyChangedEventArgs>(() => new ValueChangedEventArgs<TProperty>(message, oldvalue, newValue));


                objectInstance.RaisePropertyChanged(lz);
                if (ValueChanged != null) ValueChanged(this, lz.Value as ValueChangedEventArgs<TProperty>);

            }
        }

        public ViewModelBase ViewModel { get; internal set; }






        object IPropertyContainer.Value
        {
            get
            {
                return Value;
            }
            set
            {
                SetValueAndTryNotify((TProperty)value);
            }
        }



        public Type PropertyType
        {
            get;
            private set;
        }

        public ErrorEntity Error
        {
            get;
            set;
        }
    }

    public class ValueChangedEventArgs<TProperty> : PropertyChangedEventArgs
    {
        public ValueChangedEventArgs(string propertyName, TProperty oldValue, TProperty newValue)
            : base(propertyName)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }


        public TProperty NewValue { get; private set; }

        public TProperty OldValue { get; private set; }
    }

    //public class ValueChangedEventObject<TProperty>
    //{
    //    public ValueChangedEventObject(PropertyContainer<TProperty> core)
    //    {
    //        _core = core;
    //    }
    //    PropertyContainer<TProperty> _core;

    //    public event EventHandler<ValueChangedEventArgs<TProperty>> ValueChanged
    //    {
    //        add
    //        {
    //            _core.ValueChanged += value;
    //        }
    //        remove
    //        {
    //            _core.ValueChanged -= value;
    //        }
    //    }

    //}
    [DataContract]
    public abstract class ViewModelBase<TViewModel> : ViewModelBase where TViewModel : ViewModelBase<TViewModel>
    {
        protected static Task _EmptyStartedTask = Task.Factory.StartNew(() => { });
        public override object this[string colName]
        {
            get
            {
                var lc = GetOrCreatePlainLocator(colName, this);
                return lc((TViewModel)this).Value;
            }
            set
            {

                var lc = GetOrCreatePlainLocator(colName, this);
                lc((TViewModel)this).Value = value;
            }
        }

        private static Func<TViewModel, IPropertyContainer> GetOrCreatePlainLocator(string colName, ViewModelBase viewModel)
        {
            Func<TViewModel, IPropertyContainer> pf;
            if (!_plainPropertyContainerGetters.TryGetValue(colName, out pf))
            {
                var p = new ValueContainer<object>(colName, viewModel);
                pf = _ => p;
                _plainPropertyContainerGetters[colName] = pf;
            }
            return pf;
        }

        protected static class TypeDic<TProperty>
        {
            public static Dictionary<string, Func<TViewModel, ValueContainer<TProperty>>> _propertyContainerGetters = new Dictionary<string, Func<TViewModel, ValueContainer<TProperty>>>();

        }



        protected static SortedDictionary<string, Func<TViewModel, IPropertyContainer>>
            _plainPropertyContainerGetters =
            new SortedDictionary<string, Func<TViewModel, IPropertyContainer>>(StringComparer.CurrentCultureIgnoreCase);




        public override string Error
        {
            get { return m_Error.Locate(this).Value; }
            protected set { m_Error.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property string Error Setup
        protected Property<string> m_Error = new Property<string>(m_ErrorLocator);
        static Func<ViewModelBase, ValueContainer<string>> m_ErrorLocator =
            RegisterContainerLocator<string>(
                "Error",
                model =>
                    model.m_Error.Container =
                        model.m_Error.Container
                        ??
                        new ValueContainer<string>("Error", model));
        #endregion








        //public static void AddIfNotIn<TK, TV>(this IDictionary<TK, TV> dic, TK key, TV value)
        //{
        //    if (!dic.ContainsKey(key))
        //    {
        //        dic.Add(key, value);
        //    }

        //}

        protected static Func<ViewModelBase, ValueContainer<TProperty>> RegisterContainerLocator<TProperty>(string propertyName, Func<TViewModel, ValueContainer<TProperty>> getOrCreateLocatorMethod)
        {

            //_names[propertyName] = propertyName;
            TypeDic<TProperty>._propertyContainerGetters[propertyName] = getOrCreateLocatorMethod;
            _plainPropertyContainerGetters[propertyName] = (v) => getOrCreateLocatorMethod(v) as IPropertyContainer;
            return o => getOrCreateLocatorMethod((TViewModel)o);
        }




        //public ValueChangedEventObject<TProperty> GetPropertyValueChangeEventObject<TProperty>(string propertyName)
        //{
        //    Func<TViewModel, PropertyContainer<TProperty>> contianerGetterCreater;
        //    if (!TypeDic<TProperty>._propertyContainerGetters.TryGetValue(propertyName, out contianerGetterCreater))
        //    {
        //        throw new Exception("Property Not Exists!");

        //    }

        //    return contianerGetterCreater((TViewModel)(Object)this).EventObject;

        //}

        //public ValueChangedEventObject<TProperty> GetPropertyValueChangeEventObject<TProperty>(Expression<Func<TViewModel, TProperty>> expression)
        //{

        //    var propName = GetExpressionMemberName(expression);
        //    return GetPropertyValueChangeEventObject<TProperty>(propName);

        //}


        public ValueContainer<TProperty> GetPropertyContainer<TProperty>(string propertyName)
        {
            Func<TViewModel, ValueContainer<TProperty>> contianerGetterCreater;
            if (!TypeDic<TProperty>._propertyContainerGetters.TryGetValue(propertyName, out contianerGetterCreater))
            {
                throw new Exception("Property Not Exists!");

            }

            return contianerGetterCreater((TViewModel)(Object)this);

        }
        public ValueContainer<TProperty> GetPropertyContainer<TProperty>(Expression<Func<TViewModel, TProperty>> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;
            var propName = (body.Member is PropertyInfo) ? body.Member.Name : string.Empty;
            return GetPropertyContainer<TProperty>(propName);

        }

        protected override string GetColumnError(string columnName)
        {
            return _plainPropertyContainerGetters[columnName]((TViewModel)this).Error.Message;
        }




        public override IEnumerable<string> FieldNames
        {
            get { return _plainPropertyContainerGetters.Keys; }
        }

        public TViewModel Clone()
        {
            var x = (TViewModel)Activator.CreateInstance(typeof(TViewModel));
            CopyTo(x);
            return x;
        }

        public void CopyTo(TViewModel x)
        {
            foreach (var item in FieldNames)
            {
                x[item] = this[item];
            }
        }


    }



    public struct ErrorEntity
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
    public interface IErrorInfo
    {
        ErrorEntity Error { get; set; }
    }

    public interface IValueCanSet<in T>
    {
        T Value { set; }
    }

    public interface IValueCanGet<out T>
    {
        T Value { get; }
    }

    public interface IPropertyContainer : IErrorInfo
    {
        Type PropertyType { get; }
        Object Value { get; set; }
    }



    public static class CommandModelExtensions
    {
        public static CommandModel<TCommand, TResource> CreateCommandModel<TCommand, TResource>(this TCommand command, TResource resource)
            where TCommand : ICommand
        {
            return new CommandModel<TCommand, TResource>(command, resource);
        }

        public static CommandModel<TCommand, object> CreateCommandModel<TCommand>(this TCommand command, object resource = null)
        where TCommand : ICommand
        {
            return new CommandModel<TCommand, object>(command, null);
        }

        public static CommandModel<TCommand, TResource> WithViewModel<TCommand, TResource>(this CommandModel<TCommand, TResource> cmdModel, Object viewModel)
            where TCommand : ICommand
        {
            cmdModel.ConfigCommandCore
                (
                    cmd =>
                    {
                        var cmd2 = cmd as ICommandWithViewModel;
                        if (cmd2 != null)
                        {
                            cmd2.ViewModel = viewModel;
                        }

                    }
                );
            return cmdModel;
        }
    }


    //public class CommandModel<TCommand> : CommandModel<TCommand, Object>
    //         where TCommand : ICommand
    //{
    //    public CommandModel(TCommand commandCore, object resource)
    //        : base(commandCore, resource)
    //    { }

    //}
    public class CommandModel<TCommand, TResource> : ViewModelBase<CommandModel<TCommand, TResource>>, ICommand
        where TCommand : ICommand
    {
        public CommandModel()
        { }

        public CommandModel(TCommand commandCore, TResource resource)
        {
            CommandCore = commandCore;
            commandCore.CanExecuteChanged += commandCore_CanExecuteChanged;
            Resource = resource;
        }

        void commandCore_CanExecuteChanged(object sender, EventArgs e)
        {
            if (CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, e);
            }

        }



        protected TCommand CommandCore
        {
            get;
            set;

        }

        public CommandModel<TCommand, TResource> ConfigCommandCore(Action<TCommand> commandConfigAction)
        {
            commandConfigAction(CommandCore);
            return this;
        }


        public bool LastCanExecuteValue
        {
            get { return m_LastCanExecuteValue.Locate(this).Value; }
            set { m_LastCanExecuteValue.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property bool LastCanExecuteValue Setup
        protected Property<bool> m_LastCanExecuteValue = new Property<bool>(m_LastCanExecuteValueLocator);
        static Func<ViewModelBase, ValueContainer<bool>> m_LastCanExecuteValueLocator =
            RegisterContainerLocator<bool>(
                "LastCanExecuteValue",
                model =>
                    model.m_LastCanExecuteValue.Container =
                        model.m_LastCanExecuteValue.Container
                        ??
                        new ValueContainer<bool>("LastCanExecuteValue", model));
        #endregion





        public TResource Resource
        {
            get { return m_Resource.Locate(this).Value; }
            set { m_Resource.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property TResource Resource Setup
        protected Property<TResource> m_Resource = new Property<TResource>(m_ResourceLocator);
        static Func<ViewModelBase, ValueContainer<TResource>> m_ResourceLocator =
            RegisterContainerLocator<TResource>(
                "Resource",
                model =>
                    model.m_Resource.Container =
                        model.m_Resource.Container
                        ??
                        new ValueContainer<TResource>("Resource", model));
        #endregion











        public bool CanExecute(object parameter)
        {
            var s = CommandCore.CanExecute(parameter);
            LastCanExecuteValue = s;
            return s;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            CommandCore.Execute(parameter);
        }
    }
}


namespace MVVM.EventRouter
{
    public static class EventRouterHelper
    {

        public static void RaiseEvent<TEventArgs>(this ViewModelBase source, TEventArgs eventArgs)
            where TEventArgs : EventArgs
        {
            EventRouter.Instance.RaiseEvent(source, eventArgs);
        }

    }

    public class EventRouter
    {
        protected EventRouter()
        {

        }
        static EventRouter()
        {
            Instance = new EventRouter();
        }

        public static EventRouter Instance { get; protected set; }



        public virtual void RaiseEvent<TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            var eventObject = GetInstance(typeof(TEventArgs));
            eventObject.RaiseEvent(sender, eventArgs);

            while (eventObject.BaseArgsTypeInstance != null)
            {
                eventObject = eventObject.BaseArgsTypeInstance;
                eventObject.RaiseEvent(sender, eventArgs);
            }
        }

        public virtual EventObject<TEventArgs> GetEventObject<TEventArgs>() where TEventArgs : EventArgs
        {
            var eventObject = (EventObject<TEventArgs>)GetInstance(typeof(TEventArgs));

            return eventObject;

        }

        static protected readonly ConcurrentDictionary<Type, IEventObject> EventObjects
             = new ConcurrentDictionary<Type, IEventObject>();
        static protected IEventObject GetInstance(Type argsType)
        {

            var rval = EventObjects.GetOrAdd(
                argsType,
                t =>
                    Activator.CreateInstance(typeof(EventObject<>).MakeGenericType(t)) as IEventObject
                );

            if (rval.BaseArgsTypeInstance == null)
            {
#if NETFX_CORE
                var baseT = argsType.GetTypeInfo().BaseType;
#else
                var baseT = argsType.BaseType;
#endif
                if (baseT != typeof(object))
                {
                    rval.BaseArgsTypeInstance = GetInstance(baseT);
                }

            }

            return rval;
        }



        protected interface IEventObject
        {
            IEventObject BaseArgsTypeInstance { get; set; }
            void RaiseEvent(object sender, EventArgs args);
        }


        public class EventObject<TEventArgs> : IEventObject
            where TEventArgs : EventArgs
        {
            public EventObject()
            {
            }

            public event EventHandler<TEventArgs> Event;




            IEventObject IEventObject.BaseArgsTypeInstance
            {
                get;
                set;
            }

            public void RaiseEvent(object sender, EventArgs args)
            {
                RaiseEvent(sender, args as TEventArgs);
            }

            public void RaiseEvent(object sender, TEventArgs args)
            {
                var a = args;
                if (a != null && Event != null)
                {
                    Event(sender, a);
                }
            }
        }


    }

    public class NavigateCommandEventArgs : EventArgs
    {
        public NavigateCommandEventArgs()
        {
            ParameterDictionary = new Dictionary<string, object>();
        }
        public NavigateCommandEventArgs(IDictionary<string, object> dic)
            : this()
        {
            foreach (var item in dic)
            {

                (ParameterDictionary as IDictionary<string, object>)[item.Key] = item.Value;
            }

        }
        public Dictionary<string, object> ParameterDictionary { get; set; }

        public string SourceViewId { get; set; }

        public string TargetViewId { get; set; }
    }

    public class SaveStateEventArgs : EventArgs
    {
        public string ViewKeyId { get; set; }
        public Dictionary<string, object> State { get; set; }
    }


}


namespace MVVM.Commands
{

    public class EventCommandEventArgs : EventArgs
    {
        public Object Parameter { get; set; }
        public Object ViewModel { get; set; }

        public static EventCommandEventArgs Create(Object parameter, Object viewModel)
        {

            return new EventCommandEventArgs { Parameter = parameter, ViewModel = viewModel };

        }
    }

    public static class EventCommandHelper
    {
        public static TCommand WithViewModel<TCommand>(this TCommand cmd, Object viewModel)
            where TCommand : EventCommandBase
        {
            cmd.ViewModel = viewModel;
            return cmd;
        }

    }
    public interface ICommandWithViewModel : ICommand
    {
        Object ViewModel { get; set; }
    }
    public abstract class EventCommandBase : ICommandWithViewModel
    {
        public Object ViewModel { get; set; }

        public event EventHandler<EventCommandEventArgs> CommandExecute;
        protected void OnCommandExecute(EventCommandEventArgs args)
        {
            if (CommandExecute != null)
            {
                CommandExecute(this, args);
            }
        }



        public abstract bool CanExecute(object parameter);


        public event EventHandler CanExecuteChanged;
        protected void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                OnCommandExecute(EventCommandEventArgs.Create(parameter, ViewModel));
            }
        }
    }
}