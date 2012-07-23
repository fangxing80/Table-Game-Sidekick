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
    [DataContract]
    public abstract class ViewModelBase
        : IDisposable, INotifyPropertyChanged, IDataErrorInfo
    {




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




        public abstract string Error { get; }




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


    public class PropertyContainer<TProperty> : IErrorInfo, IValueCanSet<TProperty>, IValueCanGet<TProperty>
    {
        public event EventHandler<ValueChangedEventArgs<TProperty>> ValueChanged = (o, e) => { };

        /// <summary>
        /// 创建属性值容器
        /// </summary>
        /// <param name="info">属性名</param>
        public PropertyContainer(string info)
            : this(info, (v1, v2) => v1.Equals(v2), default(TProperty))
        {
        }

        /// <summary>
        /// 创建属性值容器
        /// </summary>
        /// <param name="info">属性名</param>
        /// <param name="equalityComparer">判断两个值是否相等的比较器</param>
        public PropertyContainer(string info, Func<TProperty, TProperty, bool> equalityComparer)
            : this(info, equalityComparer, default(TProperty))
        {
        }

        /// <summary>
        /// 创建属性值容器
        /// </summary>
        /// <param name="info">属性名</param>
        /// <param name="initValue">初始值</param>
        public PropertyContainer(string info, TProperty initValue)
            : this(info, (v1, v2) => v1.Equals(v2), initValue)
        {
        }

        /// <summary>
        /// 创建属性值容器
        /// </summary>
        /// <param name="info">属性名</param>
        /// <param name="equalityComparer">判断两个值是否相等的比较器</param>
        /// <param name="initValue">初始值</param>
        public PropertyContainer(string info, Func<TProperty, TProperty, bool> equalityComparer, TProperty initValue)
        {
            EqualityComparer = equalityComparer;
            PropertyName = info;
            _value = initValue;
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


        /// <summary>
        /// 该容器出现的错误
        /// </summary>

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

        protected static class TypeDic<TProperty>
        {
            public static Dictionary<string, Func<TViewModel, PropertyContainer<TProperty>>> _propertyContainerGetters = new Dictionary<string, Func<TViewModel, PropertyContainer<TProperty>>>();

        }
        protected static Dictionary<string, string> _names = new Dictionary<string, string>();
        protected static Dictionary<string, Func<TViewModel, IPropertyContainerForRead<Object>>> _propertyContainerGettersForRead = new Dictionary<string, Func<TViewModel, IPropertyContainerForRead<object>>>();
        protected static String GetExpressionMemberName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;
            return (body.Member is PropertyInfo) ? body.Member.Name : string.Empty;
        }



        public override string Error
        {
            get { return _ErrorContainerLocator(this).Value; }

        }
        #region Property string Error Setup
        protected PropertyContainer<string> _Error;
        protected static Func<object, PropertyContainer<string>> _ErrorContainerLocator =
            RegisterContainerLocator<string>(
                "Error",
                model =>
                    model._Error =
                        model._Error
                        ??
                        new PropertyContainer<string>("Error"));
        #endregion



        protected static Func<object, PropertyContainer<TProperty>> RegisterContainerLocator<TProperty>(string propertyName, Func<TViewModel, PropertyContainer<TProperty>> getOrCreateLocatorMethod)
        {
            Func<TViewModel, PropertyContainer<TProperty>> rval =
                model =>
                {
                    var r = getOrCreateLocatorMethod(model); //让PropertyContainer具有ViewModel引用 此机制很重要
                    r.ViewModel = model;
                    return r;
                };
            _names.Add(propertyName, propertyName);
            _propertyContainerGettersForRead.Add(propertyName, m => rval(m) as IPropertyContainerForRead<object>);
            TypeDic<TProperty>._propertyContainerGetters[propertyName] = rval;
            return o => rval((TViewModel)o);
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


        public PropertyContainer<TProperty> GetPropertyContainer<TProperty>(string propertyName)
        {
            Func<TViewModel, PropertyContainer<TProperty>> contianerGetterCreater;
            if (!TypeDic<TProperty>._propertyContainerGetters.TryGetValue(propertyName, out contianerGetterCreater))
            {
                throw new Exception("Property Not Exists!");

            }

            return contianerGetterCreater((TViewModel)(Object)this);

        }
        public PropertyContainer<TProperty> GetPropertyContainer<TProperty>(Expression<Func<TViewModel, TProperty>> expression)
        {

            var propName = GetExpressionMemberName(expression);
            return GetPropertyContainer<TProperty>(propName);

        }


        protected override string GetColumnError(string columnName)
        {
            return _propertyContainerGettersForRead[columnName]((TViewModel)this).Error.Message;
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

    public interface IPropertyContainerForRead<T> : IErrorInfo, IValueCanGet<T>
    {
    }



    public static class CommandModelExtensions
    {
        public static CommandModel<TCommand, TResource> CreateCommandModel<TCommand, TResource>(this TCommand command, TResource resource)
            where TCommand : ICommand
        {
            return new CommandModel<TCommand, TResource>(command, resource);
        }

        public static CommandModel<TCommand> CreateCommandModel<TCommand>(this TCommand command, object resource = null)
        where TCommand : ICommand
        {
            return new CommandModel<TCommand>(command, null);
        }


    }


    public class CommandModel<TCommand> : CommandModel<TCommand, Object>
             where TCommand : ICommand
    {
        public CommandModel(TCommand commandCore, object resource)
            : base(commandCore, resource)
        { }

    }
    public class CommandModel<TCommand, TResource> : ViewModelBase<CommandModel<TCommand, TResource>>
        where TCommand : ICommand
    {
        public CommandModel(TCommand commandCore, TResource resource)
        {
            CommandCore = commandCore;
            Resource = resource;
        }


        public TCommand CommandCore
        {
            get { return m_CommandCoreContainerLocator(this).Value; }
            private set { m_CommandCoreContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property TCommand CommandCore Setup
        protected PropertyContainer<TCommand> m_CommandCore;
        protected static Func<object, PropertyContainer<TCommand>> m_CommandCoreContainerLocator =
            RegisterContainerLocator<TCommand>(
                "CommandCore",
                model =>
                    model.m_CommandCore =
                        model.m_CommandCore
                        ??
                        new PropertyContainer<TCommand>("CommandCore"));
        #endregion


        public TResource Resource
        {
            get { return m_ResourceContainerLocator(this).Value; }
            set { m_ResourceContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property TResource Resource Setup
        protected PropertyContainer<TResource> m_Resource;
        protected static Func<object, PropertyContainer<TResource>> m_ResourceContainerLocator =
            RegisterContainerLocator<TResource>(
                "Resource",
                model =>
                    model.m_Resource =
                        model.m_Resource
                        ??
                        new PropertyContainer<TResource>("Resource"));
        #endregion











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
        public static EventRouter Instance { get; private set; }



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

        static protected readonly ConcurrentDictionary<Type, IEventObject<EventArgs>> EventObjects
             = new ConcurrentDictionary<Type, IEventObject<EventArgs>>();
        static protected IEventObject<EventArgs> GetInstance(Type argsType)
        {

            var rval = EventObjects.GetOrAdd(
                argsType,
                t =>
                    Activator.CreateInstance(typeof(EventObject<>).MakeGenericType(t)) as IEventObject<EventArgs>
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



        protected interface IEventObject<in TEventArgs>
            where TEventArgs : EventArgs
        {
            IEventObject<EventArgs> BaseArgsTypeInstance { get; set; }
            void RaiseEvent(object sender, TEventArgs args);
        }


        public class EventObject<TEventArgs> : IEventObject<TEventArgs>
            where TEventArgs : EventArgs
        {
            protected EventObject()
            {
            }

            public event EventHandler<TEventArgs> Event;


            void IEventObject<TEventArgs>.RaiseEvent(object sender, TEventArgs args)
            {
                var a = args as TEventArgs;
                if (a != null && Event != null)
                {
                    Event(sender, a);
                }
            }

            IEventObject<EventArgs> IEventObject<TEventArgs>.BaseArgsTypeInstance
            {
                get;
                set;
            }
        }


    }

    public class NavigateEventArgs : EventArgs
    {
        public NavigateEventArgs()
        {
            ParameterDictionary = new System.Dynamic.ExpandoObject();
        }
        public NavigateEventArgs(IDictionary<string, object> dic)
            : this()
        {
            foreach (var item in dic)
            {

                (ParameterDictionary as IDictionary<string, object>)[item.Key] = item.Value;
            }

        }
        public dynamic ParameterDictionary { get; set; }

        public string SourceViewId { get; set; }

        public string TargetViewId { get; set; }
    }

}