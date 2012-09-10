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
using MVVMSidekick.ViewModels;
using System.Dynamic;
using MVVMSidekick.Commands;
using System.Runtime.CompilerServices;
#if NETFX_CORE
using Windows.UI.Xaml.Data;
#else
using System.Windows.Data;
#endif
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




namespace MVVMSidekick
{
    namespace ViewModels
    {

        /// <summary>
        /// 缺省的 ViewModel。可以用作最简单的字典绑定
        /// </summary>
        public class DefaultViewModel : ViewModelBase<DefaultViewModel>
        {

        }

        /// <summary>
        /// ViewModel 基类。用在所有不需要明确子类型属性定义的情形。
        /// </summary>
        [DataContract]
        public abstract class ViewModelBase
            : IDisposable, INotifyPropertyChanged, IDataErrorInfo
        {

            #region 索引与字段名
            /// <summary>
            /// 取得本VM实例已经定义的所有字段名。其中包括静态声明的和动态添加的。
            /// </summary>
            /// <returns></returns>
            public abstract string[] GetFieldNames();

            /// <summary>
            /// 使用索引方式取得字段值
            /// </summary>
            /// <param name="name">字段名</param>
            /// <returns>字段值</returns>
            public abstract object this[string name] { get; set; }


            #endregion

            #region Disposing 相关逻辑

            /// <summary>
            /// 当本VM被销毁的时候 需要做的若干动作
            /// </summary>
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

            #endregion

            #region Propery Changed 事件相关逻辑

            internal void RaisePropertyChanged(Lazy<PropertyChangedEventArgs> lazyEAFactory, string propertyName)
            {


                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, lazyEAFactory.Value);
                }
                //无论是否注册PropertyChanged 都要运行一次 OnValidate。确定OnValidate必须在末尾调用，并且不检查Error字段。
                if (propertyName != "Error")
                {

                    if (ValidateModel != null)
                    {
                        ValidateModel(lazyEAFactory.Value);
                    }
                }

            }

            /// <summary>
            /// VM属性任何绑定用值被修改后，触发此事件
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;


            #endregion

            #region 验证与错误相关逻辑

            /// <summary>
            /// 验证逻辑
            /// </summary>
            protected Action<PropertyChangedEventArgs> ValidateModel { get; set; }



            public IDataErrorInfo DataErrorInfo
            {
                get { return this; }

            }


            /// <summary>
            /// 验证错误内容
            /// </summary>
            public abstract string Error { get; protected set; }



            /// <summary>
            /// 对于每个字段，验证失败所产生的错误信息
            /// </summary>
            /// <param name="columnName"></param>
            /// <returns></returns>
            string IDataErrorInfo.this[string columnName]
            {
                get { return GetColumnError(columnName); }
            }

            protected abstract string GetColumnError(string columnName);



            #endregion


            public abstract bool IsUIBusy { get; set; }


        }

        /// <summary>
        /// 为ViewModel增加的一些关于Dispose的快捷方法
        /// </summary>
        public static class DisposableExtensions
        {
            /// <summary>
            /// 将IDisposable 对象注册到VM中的销毁对象列表。
            /// </summary>
            /// <typeparam name="T">VM的类型</typeparam>
            /// <param name="item">IDisposable实例</param>
            /// <param name="vm">VM实例</param>
            /// <returns></returns>
            public static T RegisterDisposeToViewModel<T>(this T item, ViewModelBase vm) where T : IDisposable
            {
                vm.AddDisposable(item);
                return item;
            }
        }


        /// <summary>
        /// 属性定义。一个属性定义包括一个创建/定位属性“值容器”的静态方法引用，和一个缓存该方法执行结果“值容器”的槽位
        /// </summary>
        /// <typeparam name="TProperty">属性的类型</typeparam>
        public class Property<TProperty>
        {
            public Property()
            {

            }

            /// <summary>
            /// 通过定位方法定位本VM实例中的值容器
            /// </summary>
            /// <param name="viewModel">VM实例</param>
            /// <returns>值容器</returns>
            public ValueContainer<TProperty> LocateValueContainer(ViewModelBase viewModel)
            {

                return LocatorFunc(viewModel);
            }


            /// <summary>
            /// 定位值容器用的方法。
            /// </summary>
            public Func<ViewModelBase, ValueContainer<TProperty>> LocatorFunc
            {
                private get;
                set;
            }

            /// <summary>
            /// 值容器实例
            /// </summary>
            public ValueContainer<TProperty> Container
            {
                get;
                set;
            }

        }

        /// <summary>
        /// 值容器
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        public class ValueContainer<TProperty> : IErrorInfo, IValueCanSet<TProperty>, IValueCanGet<TProperty>, IValueContainer
        {




            #region 构造器

            /// <summary>
            /// 创建属性值容器
            /// </summary>
            /// <param name="info">属性名</param>
            public ValueContainer(string info, ViewModelBase vm)
                : this(info, vm, (v1, v2) => v1.Equals(v2), default(TProperty))
            {
            }

            /// <summary>
            /// 创建属性值容器
            /// </summary>
            /// <param name="info">属性名</param>
            /// <param name="equalityComparer">判断两个值是否相等的比较器</param>
            public ValueContainer(string info, Func<TProperty, TProperty, bool> equalityComparer, ViewModelBase vm)
                : this(info, vm, equalityComparer, default(TProperty))
            {
            }

            /// <summary>
            /// 创建属性值容器
            /// </summary>
            /// <param name="info">属性名</param>
            /// <param name="initValue">初始值</param>
            public ValueContainer(string info, ViewModelBase vm, TProperty initValue)
                : this(info, vm, (v1, v2) => v1.Equals(v2), initValue)
            {
            }

            /// <summary>
            /// 创建属性值容器
            /// </summary>
            /// <param name="info">属性名</param>
            /// <param name="equalityComparer">判断两个值是否相等的比较器</param>
            /// <param name="initValue">初始值</param>
            public ValueContainer(string info, ViewModelBase vm, Func<TProperty, TProperty, bool> equalityComparer, TProperty initValue)
            {
                EqualityComparer = equalityComparer;
                PropertyName = info;
                _value = initValue;
                PropertyType = typeof(TProperty);
                ViewModel = vm;
                // _eventObject = new ValueChangedEventObject<TProperty>(this);
            }

            #endregion


            public event EventHandler<ValueChangedEventArgs<TProperty>> ValueChanged;

            /// <summary>
            /// 判断两个值是否相等的比较器
            /// </summary>
            public Func<TProperty, TProperty, bool> EqualityComparer { get; private set; }

            /// <summary>
            /// 属性名
            /// </summary>
            public string PropertyName { get; private set; }

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


                    objectInstance.RaisePropertyChanged(lz, message);
                    if (ValueChanged != null) ValueChanged(this, lz.Value as ValueChangedEventArgs<TProperty>);

                }
            }

            public ViewModelBase ViewModel { get; internal set; }





            /// <summary>
            /// 值，Object形式
            /// </summary>
            object IValueContainer.Value
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


            /// <summary>
            /// 值类型
            /// </summary>
            public Type PropertyType
            {
                get;
                private set;
            }

            /// <summary>
            /// 出现问题的时候保存错误的结构
            /// </summary>
            public ErrorEntity Error
            {
                get;
                set;
            }
        }


        /// <summary>
        /// 值变化事件参数
        /// </summary>
        /// <typeparam name="TProperty">变化属性的类型</typeparam>
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


        /// <summary>
        /// 具有子类详细类型定义的VM 使用方式为 class Class1:ViewModelBase<Class1>  用此模式将子类的信息传入ViewModelBase
        /// </summary>
        /// <typeparam name="TViewModel">子类类型</typeparam>
        [DataContract]
        public abstract class ViewModelBase<TViewModel> : ViewModelBase where TViewModel : ViewModelBase<TViewModel>
        {

            /// <summary>
            /// 每个属性类型独占的一个专门的类型缓存。
            /// </summary>
            /// <typeparam name="TProperty"></typeparam>
            protected static class TypeDic<TProperty>
            {
                public static Dictionary<string, Func<TViewModel, ValueContainer<TProperty>>> _propertyContainerGetters = new Dictionary<string, Func<TViewModel, ValueContainer<TProperty>>>();

            }

            /// <summary>
            /// 根据索引获取属性值
            /// </summary>
            /// <param name="colName">属性名</param>
            /// <returns>属性值</returns>
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

            private static Func<TViewModel, IValueContainer> GetOrCreatePlainLocator(string colName, ViewModelBase viewModel)
            {
                Func<TViewModel, IValueContainer> pf;
                if (!_plainPropertyContainerGetters.TryGetValue(colName, out pf))
                {
                    var p = new ValueContainer<object>(colName, viewModel);
                    pf = _ => p;
                    _plainPropertyContainerGetters[colName] = pf;
                }
                return pf;
            }





            protected static SortedDictionary<string, Func<TViewModel, IValueContainer>>
                _plainPropertyContainerGetters =
                new SortedDictionary<string, Func<TViewModel, IValueContainer>>(StringComparer.CurrentCultureIgnoreCase);

            /// <summary>
            /// 将一个VM引用特化为本子类型的引用
            /// </summary>
            /// <param name="vm"></param>
            /// <returns></returns>
            protected static TViewModel CastVM(ViewModelBase vm)
            {
                return (TViewModel)vm;

            }


            /// <summary>
            /// 验证错误
            /// </summary>
            public override string Error
            {
                get { return m_ErrorLocator(this).Value; }
                protected set { m_ErrorLocator(this).SetValueAndTryNotify(value); }
            }


            #region Property string Error Setup

            protected Property<string> m_Error =
              new Property<string> { LocatorFunc = m_ErrorLocator };
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            static Func<ViewModelBase, ValueContainer<string>> m_ErrorLocator =
                RegisterContainerLocator<string>(
                "Error",
                model =>
                {
                    model.m_Error =
                        model.m_Error
                        ??
                        new Property<string> { LocatorFunc = m_ErrorLocator };
                    return model.m_Error.Container =
                        model.m_Error.Container
                        ??
                        new ValueContainer<string>("Error", model);
                });

            #endregion

            /// <summary>
            /// 本UI是否处于忙状态
            /// </summary>
            public override bool IsUIBusy
            {
                get { return m_IsUIBusyLocator(this).Value; }
                set { m_IsUIBusyLocator(this).SetValueAndTryNotify(value); }
            }

            #region Property bool IsUIBusy Setup
            protected Property<bool> m_IsUIBusy =
              new Property<bool> { LocatorFunc = m_IsUIBusyLocator };
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            static Func<ViewModelBase, ValueContainer<bool>> m_IsUIBusyLocator =
                RegisterContainerLocator<bool>(
                    "IsUIBusy",
                    model =>
                    {
                        model.m_IsUIBusy =
                            model.m_IsUIBusy
                            ??
                            new Property<bool> { LocatorFunc = m_IsUIBusyLocator };
                        return model.m_IsUIBusy.Container =
                            model.m_IsUIBusy.Container
                            ??
                            new ValueContainer<bool>("IsUIBusy", model);
                    });
            #endregion





            /// <summary>
            /// 注册一个属性容器的定位器。
            /// </summary>
            /// <typeparam name="TProperty">属性类型</typeparam>
            /// <param name="propertyName">属性名</param>
            /// <param name="getOrCreateLocatorMethod">属性定位/创建方法 也就是定位器</param>
            /// <returns>注册后的定位器</returns>
            protected static Func<ViewModelBase, ValueContainer<TProperty>> RegisterContainerLocator<TProperty>(string propertyName, Func<TViewModel, ValueContainer<TProperty>> getOrCreateLocatorMethod)
            {


                TypeDic<TProperty>._propertyContainerGetters[propertyName] = getOrCreateLocatorMethod;
                _plainPropertyContainerGetters[propertyName] = (v) => getOrCreateLocatorMethod(v) as IValueContainer;
                return o => getOrCreateLocatorMethod((TViewModel)o);
            }


            /// <summary>
            /// 根据属性名取得一个值容器
            /// </summary>
            /// <typeparam name="TProperty">属性类型</typeparam>
            /// <param name="propertyName">属性名</param>
            /// <returns>值容器</returns>
            public ValueContainer<TProperty> GetValueContainer<TProperty>(string propertyName)
            {
                Func<TViewModel, ValueContainer<TProperty>> contianerGetterCreater;
                if (!TypeDic<TProperty>._propertyContainerGetters.TryGetValue(propertyName, out contianerGetterCreater))
                {
                    throw new Exception("Property Not Exists!");

                }

                return contianerGetterCreater((TViewModel)(Object)this);

            }

            /// <summary>
            /// 根据表达式树取得一个值容器
            /// </summary>
            /// <typeparam name="TProperty">属性类型</typeparam>
            /// <param name="expression">表达式树</param>
            /// <returns>值容器</returns>
            public ValueContainer<TProperty> GetValueContainer<TProperty>(Expression<Func<TViewModel, TProperty>> expression)
            {
                MemberExpression body = expression.Body as MemberExpression;
                var propName = (body.Member is PropertyInfo) ? body.Member.Name : string.Empty;
                return GetValueContainer<TProperty>(propName);

            }



            /// <summary>
            /// 根据属性名取得一个值容器
            /// </summary>
            /// <param name="propertyName">属性名</param>
            /// <returns>值容器</returns>
            public IValueContainer GetValueContainer(string propertyName)
            {
                Func<TViewModel, IValueContainer> contianerGetterCreater;
                if (_plainPropertyContainerGetters.TryGetValue(propertyName, out contianerGetterCreater))
                {
                    throw new Exception("Property Not Exists!");

                }

                return contianerGetterCreater((TViewModel)(Object)this);

            }




            /// <summary>
            /// 获取某一属性的验证错误信息
            /// </summary>
            /// <param name="columnName">属性名</param>
            /// <returns>错误信息字符串</returns>
            protected override string GetColumnError(string columnName)
            {
                return _plainPropertyContainerGetters[columnName]((TViewModel)this).Error.Message;
            }



            /// <summary>
            /// 获取所有属性名，包括静态声明和动态添加的
            /// </summary>
            /// <returns></returns>
            public override string[] GetFieldNames()
            {
                return _plainPropertyContainerGetters.Keys.ToArray();
            }


            /// <summary>
            /// 创建一个VM副本
            /// </summary>
            /// <returns>新引用</returns>
            public TViewModel Clone()
            {
                var x = (TViewModel)Activator.CreateInstance(typeof(TViewModel));
                CopyTo(x);
                return x;
            }

            public void CopyTo(TViewModel x)
            {
                foreach (var item in GetFieldNames())
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

        public interface IValueContainer : IErrorInfo
        {
            Type PropertyType { get; }
            Object Value { get; set; }
        }

        public interface ICommandModel<TCommand, TResource>:ICommand
        {
            TCommand CommandCore { get; }
            bool LastCanExecuteValue { get; set; }
            TResource Resource { get; set; }
        }

        /// <summary>
        /// 用于封装ICommand的ViewModel。一般包括一个Command实例和对应此实例的一组资源
        /// </summary>
        /// <typeparam name="TCommand">ICommand 详细类型</typeparam>
        /// <typeparam name="TResource">配合Command 的资源类型</typeparam>
        public class CommandModel<TCommand, TResource> : ViewModelBase<CommandModel<TCommand, TResource>>,ICommandModel<TCommand, TResource>
            where TCommand : ICommand
        {
            public CommandModel()
            { }
            /// <summary>
            /// 构造器
            /// </summary>
            /// <param name="commandCore">ICommand核心</param>
            /// <param name="resource">初始资源</param>
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


            /// <summary>
            /// ICommand核心
            /// </summary>
            public TCommand CommandCore
            {
                get;
                private set;

            }

            //public CommandModel<TCommand, TResource> ConfigCommandCore(Action<TCommand> commandConfigAction)
            //{
            //    commandConfigAction(CommandCore);
            //    return this;
            //}


            /// <summary>
            /// 上一次是否能够运行的值
            /// </summary>
            public bool LastCanExecuteValue
            {
                get { return m_LastCanExecuteValueLocator(this).Value; }
                set { m_LastCanExecuteValueLocator(this).SetValueAndTryNotify(value); }
            }


            #region Property bool LastCanExecuteValue Setup

            protected Property<bool> m_LastCanExecuteValue =
              new Property<bool> { LocatorFunc = m_LastCanExecuteValueLocator };
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            static Func<ViewModelBase, ValueContainer<bool>> m_LastCanExecuteValueLocator =
                RegisterContainerLocator<bool>(
                "LastCanExecuteValue",
                model =>
                {
                    model.m_LastCanExecuteValue =
                        model.m_LastCanExecuteValue
                        ??
                        new Property<bool> { LocatorFunc = m_LastCanExecuteValueLocator };
                    return model.m_LastCanExecuteValue.Container =
                        model.m_LastCanExecuteValue.Container
                        ??
                        new ValueContainer<bool>("LastCanExecuteValue", model);
                });

            #endregion



            /// <summary>
            /// 资源
            /// </summary>
            public TResource Resource
            {
                get { return m_ResourceLocator(this).Value; }
                set { m_ResourceLocator(this).SetValueAndTryNotify(value); }
            }


            #region Property TResource Resource Setup

            protected Property<TResource> m_Resource =
              new Property<TResource> { LocatorFunc = m_ResourceLocator };
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            static Func<ViewModelBase, ValueContainer<TResource>> m_ResourceLocator =
                RegisterContainerLocator<TResource>(
                "Resource",
                model =>
                {
                    model.m_Resource =
                        model.m_Resource
                        ??
                        new Property<TResource> { LocatorFunc = m_ResourceLocator };
                    return model.m_Resource.Container =
                        model.m_Resource.Container
                        ??
                        new ValueContainer<TResource>("Resource", model);
                });

            #endregion











            /// <summary>
            /// 判断是否可执行
            /// </summary>
            /// <param name="parameter">指定参数</param>
            /// <returns></returns>
            public bool CanExecute(object parameter)
            {
                var s = CommandCore.CanExecute(parameter);
                LastCanExecuteValue = s;
                return s;
            }

            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// 执行
            /// </summary>
            /// <param name="parameter">指定参数</param>
            public void Execute(object parameter)
            {
                CommandCore.Execute(parameter);
            }
        }

        /// <summary>
        /// 可绑定的CommandVM 扩展方法集
        /// </summary>
        public static class CommandModelExtensions
        {
            /// <summary>
            /// 根据ICommand实例创建CommandModel
            /// </summary>
            /// <typeparam name="TCommand">ICommand实例的具体类型</typeparam>
            /// <typeparam name="TResource">附加资源类型</typeparam>
            /// <param name="command">ICommand实例</param>
            /// <param name="resource">资源实例</param>
            /// <returns>CommandModel实例</returns>
            public static CommandModel<TCommand, TResource> CreateCommandModel<TCommand, TResource>(this TCommand command, TResource resource)
                where TCommand : ICommand
            {
                return new CommandModel<TCommand, TResource>(command, resource);
            }

            /// <summary>
            /// 据ICommand实例创建不具备/弱类型资源的CommandModel
            /// </summary>
            /// <typeparam name="TCommand">ICommand实例的具体类型</typeparam>
            /// <param name="command">ICommand实例</param>
            /// <param name="resource">资源实例</param>
            /// <returns>CommandModel实例</returns>
            public static CommandModel<TCommand, object> CreateCommandModel<TCommand>(this TCommand command, object resource = null)
            where TCommand : ICommand
            {
                return new CommandModel<TCommand, object>(command, null);
            }

            /// <summary>
            /// 为CommandModel指定ViewModel
            /// </summary>
            /// <typeparam name="TCommand">ICommand实例的具体类型</typeparam>
            /// <typeparam name="TResource">附加资源类型</typeparam>
            /// <param name="cmdModel">CommandModel具体实例</param>
            /// <param name="viewModel">ViewModel具体实例</param>
            /// <returns></returns>
            public static CommandModel<TCommand, TResource> WithViewModel<TCommand, TResource>(this CommandModel<TCommand, TResource> cmdModel, ViewModelBase viewModel)
                where TCommand : ICommand
            {
                //cmdModel.
                var cmd2 = cmdModel.CommandCore as ICommandWithViewModel;
                if (cmd2 != null)
                {
                    cmd2.ViewModel = viewModel;
                }
                return cmdModel;
            }
        }

    }


    namespace EventRouter
    {

        /// <summary>
        /// 全局事件根
        /// </summary>
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


            /// <summary>
            /// 触发事件    
            /// </summary>
            /// <typeparam name="TEventArgs">事件数据类型</typeparam>
            /// <param name="sender">事件发送者</param>
            /// <param name="eventArgs">事件数据</param>
            /// <param name="callerMemberName">发送事件名</param>
            public virtual void RaiseEvent<TEventArgs>(object sender, TEventArgs eventArgs, string callerMemberName = "") where TEventArgs : EventArgs
            {
                var eventObject = GetIEventObjectInstance(typeof(TEventArgs));
                eventObject.RaiseEvent(sender, callerMemberName, eventArgs);

                while (eventObject.BaseArgsTypeInstance != null)
                {
                    eventObject = eventObject.BaseArgsTypeInstance;
                }
            }


            /// <summary>
            /// 取得独立事件类
            /// </summary>
            /// <typeparam name="TEventArgs">事件数据类型</typeparam>
            /// <returns>事件独立类</returns>
            public virtual EventObject<TEventArgs> GetEventObject<TEventArgs>() where TEventArgs : EventArgs
            {
                var eventObject = (EventObject<TEventArgs>)GetIEventObjectInstance(typeof(TEventArgs));

                return eventObject;

            }

            /// <summary>
            /// 事件来源的代理对象实例
            /// </summary>
            static protected readonly ConcurrentDictionary<Type, IEventObject> EventObjects
                 = new ConcurrentDictionary<Type, IEventObject>();

            /// <summary>
            /// 创建事件代理对象
            /// </summary>
            /// <param name="argsType">事件数据类型</param>
            /// <returns>代理对象实例</returns>
            static protected IEventObject GetIEventObjectInstance(Type argsType)
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
                        rval.BaseArgsTypeInstance = GetIEventObjectInstance(baseT);
                    }

                }

                return rval;
            }


            /// <summary>
            /// 事件对象接口
            /// </summary>
            protected interface IEventObject
            {
                IEventObject BaseArgsTypeInstance { get; set; }
                void RaiseEvent(object sender, string eventName, EventArgs args);
            }

            /// <summary>
            ///事件对象
            /// </summary>
            /// <typeparam name="TEventArgs"></typeparam>
            public class EventObject<TEventArgs> : IEventObject
                where TEventArgs : EventArgs
            {
                public EventObject()
                {
                }

                public event EventHandler<RouterEventData<TEventArgs>> Event;




                IEventObject IEventObject.BaseArgsTypeInstance
                {
                    get;
                    set;
                }

                void IEventObject.RaiseEvent(object sender, string eventName, EventArgs args)
                {
                    RaiseEvent(sender, eventName, args as TEventArgs);
                }

                public void RaiseEvent(object sender, string eventName, TEventArgs args)
                {


                    var a = args;
                    if (a != null && Event != null)
                    {
                        Event(sender, new RouterEventData<TEventArgs>(sender, eventName, args));
                    }
                }
            }


        }
        /// <summary>
        /// 导航事件数据
        /// </summary>
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

        /// <summary>
        /// 保存状态事件数据
        /// </summary>
        public class SaveStateEventArgs : EventArgs
        {
            public string ViewKeyId { get; set; }
            public Dictionary<string, object> State { get; set; }
        }

        /// <summary>
        /// 事件路由的扩展方法集合
        /// </summary>
        public static class EventRouterHelper
        {
            /// <summary>
            /// 触发事件
            /// </summary>
            /// <typeparam name="TEventArgs">事件类型</typeparam>
            /// <param name="source">事件来源</param>
            /// <param name="eventArgs">事件数据</param>
            /// <param name="callerMemberName">事件名</param>
            public static void RaiseEvent<TEventArgs>(this ViewModelBase source, TEventArgs eventArgs, string callerMemberName = "")
                 where TEventArgs : EventArgs
            {
                EventRouter.Instance.RaiseEvent(source, eventArgs, callerMemberName);
            }

        }

        /// <summary>
        /// 事件信息
        /// </summary>
        /// <typeparam name="TEventArgs">事件数据类型</typeparam>
        public class RouterEventData<TEventArgs> : EventArgs
        {
            public RouterEventData(object sender, string eventName, TEventArgs eventArgs)
            {

                Sender = sender;
                EventName = eventName;
                EventArgs = eventArgs;
            }
            /// <summary>
            /// 事件发送者
            /// </summary>
            public Object Sender { get; private set; }
            /// <summary>
            /// 事件名
            /// </summary>
            public string EventName { get; private set; }
            /// <summary>
            /// 事件数据
            /// </summary>
            public TEventArgs EventArgs { get; private set; }
        }

    }


    namespace Commands
    {
        /// <summary>
        /// Command被运行触发的事件数据类型
        /// </summary>
        public class EventCommandEventArgs : EventArgs
        {
            public Object Parameter { get; set; }
            public Object ViewModel { get; set; }

            public static EventCommandEventArgs Create(Object parameter, Object viewModel)
            {

                return new EventCommandEventArgs { Parameter = parameter, ViewModel = viewModel };

            }
        }

        /// <summary>
        /// 事件Command的助手类
        /// </summary>
        public static class EventCommandHelper
        {
            /// <summary>
            /// 为一个事件Command制定一个VM
            /// </summary>
            /// <typeparam name="TCommand">事件Command具体类型</typeparam>
            /// <param name="cmd">事件Command实例</param>
            /// <param name="viewModel">VM实例</param>
            /// <returns>事件Command实例本身</returns>
            public static TCommand WithViewModel<TCommand>(this TCommand cmd, ViewModelBase viewModel)
                where TCommand : EventCommandBase
            {
                cmd.ViewModel = viewModel;
                return cmd;
            }

        }

        /// <summary>
        /// 带有VM的Command接口
        /// </summary>
        public interface ICommandWithViewModel : ICommand
        {
            ViewModelBase ViewModel { get; set; }
        }

        /// <summary>
        /// 事件Command,运行后马上触发一个事件，事件中带有Command实例和VM实例属性
        /// </summary>
        public abstract class EventCommandBase : ICommandWithViewModel
        {
            /// <summary>
            /// VM
            /// </summary>
            public ViewModelBase ViewModel { get; set; }

            /// <summary>
            /// 运行时触发的事件
            /// </summary>
            public event EventHandler<EventCommandEventArgs> CommandExecute;
            /// <summary>
            /// 执行时的逻辑
            /// </summary>
            /// <param name="args">执行时的事件数据</param>
            protected virtual void OnCommandExecute(EventCommandEventArgs args)
            {
                if (CommandExecute != null)
                {
                    CommandExecute(this, args);
                }
            }


            /// <summary>
            /// 该Command是否能执行
            /// </summary>
            /// <param name="parameter">判断参数</param>
            /// <returns>是否</returns>
            public abstract bool CanExecute(object parameter);

            /// <summary>
            /// 是否能执行的值产生变化的事件
            /// </summary>
            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// 是否能执行变化时触发事件的逻辑
            /// </summary>
            protected void OnCanExecuteChanged()
            {
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// 执行Command
            /// </summary>
            /// <param name="parameter">参数条件</param>
            public virtual void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    OnCommandExecute(EventCommandEventArgs.Create(parameter, ViewModel));
                }
            }
        }
    }


    namespace ValueConverters
    {


        public class GenericValueConverter<TSource, TTarget, TParemeter> : IValueConverter
        {
            public GenericValueConverter()
            {

            }

            public GenericValueConverter(
                Func<TSource, TParemeter, string, TTarget> converter,

                Func<TTarget, TParemeter, string, TSource> convertBacker
                )
            {
                Converter = converter;
                ConvertBacker = convertBacker;
            }
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                if (Converter == null)
                {
                    throw new NotImplementedException();
                }
                OnConvertCheckInputType(value, targetType);


                return Converter((TSource)value, (TParemeter)parameter, language);
            }

            public Func<TSource, TParemeter, string, TTarget> Converter { get; set; }

            public Func<TTarget, TParemeter, string, TSource> ConvertBacker { get; set; }




            public  object ConvertBack(object value, Type targetType, object parameter, string language)
            {

                if (ConvertBacker == null)
                {
                    throw new NotImplementedException();
                }

                OnConvertBackCheckInputType(value, targetType);
                return ConvertBacker((TTarget)value, (TParemeter)parameter, language);
            }



            private static void OnConvertCheckInputType(object sourceValue, Type targetType)
            {
                if (!targetType.GetTypeInfo().IsAssignableFrom(typeof(TTarget).GetTypeInfo()))
                {
                    throw new ArgumentOutOfRangeException(string.Format("Target type is not supported.  {0} and its base class type would be fine.", typeof(TTarget).FullName));
                }
                if (!(sourceValue is TSource))
                {
                    throw new ArgumentOutOfRangeException(string.Format("Source type is expected source type. A {0} reference is expected.", typeof(TSource).FullName));
                }
            }

            private static void OnConvertBackCheckInputType(object backingValue, Type backType)
            {
                if (typeof(TSource) != backType)
                {
                    throw new ArgumentOutOfRangeException(string.Format("Target type is not supported.  {0} is expected.", typeof(TSource).FullName));
                }
                if (!(backingValue is TTarget))
                {
                    throw new ArgumentOutOfRangeException(string.Format("Source type is expected source type. A {0} reference is expected.", typeof(TTarget).FullName));
                }
            }


#if NETFX_CORE
#else
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return Convert(value, targetType, parameter, culture.EnglishName);
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return Convert(value, targetType, parameter, culture.EnglishName);
            }
#endif
        }


        public enum ErrorInfoTextConverterOptions
        {
            ErrorOnly,
            ErrorWithFieldsErrors

        }



        public class ViewModelDataErrorInfoTextConverter : GenericValueConverter<ViewModelBase, string, ErrorInfoTextConverterOptions>
        {
            public ViewModelDataErrorInfoTextConverter()
            {
                Converter = (val, options, lan) =>
                    {
                        var dataError = val as IDataErrorInfo;
                        switch (options)
                        {


                            case ErrorInfoTextConverterOptions.ErrorWithFieldsErrors:
                                var sb = new StringBuilder();
                                sb.AppendLine(val.Error);
                                foreach (var fn in val.GetFieldNames().ToArray())
                                {
                                    sb.Append("\t").Append(fn).Append(":\t").AppendLine(dataError[fn]);
                                }
                                return sb.ToString();

                            case ErrorInfoTextConverterOptions.ErrorOnly:
                            default:
                                return val.Error;
                        }
                    };



            }

        }




    }
}