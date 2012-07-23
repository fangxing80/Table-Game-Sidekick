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
using System.Reactive.Linq;
using MVVM.ViewModels;
using System.Windows.Input;
using System.Reactive.Subjects;
using System.Reactive;
using MVVM.Commands;


namespace MVVM.Reactive
{


    public static class EventPair
    {
        public static EventPair<TSource, TEventArgs> Create<TSource, TEventArgs>(TSource source, TEventArgs eventArgs)
        {
            return new EventPair<TSource, TEventArgs> { Source = source, EventArgs = eventArgs };
        }

    }
    public struct EventPair<TSource, TEventArgs>
    {
        public TSource Source { get; set; }

        public TEventArgs EventArgs { get; set; }

    }

    public static class MVVMRxExtensions
    {
        public static IObservable<EventPair<PropertyContainer<TValue>, TValue>> GetValueChangeObservable<TValue>
            (
                this PropertyContainer<TValue> source

            )
        {

            return Observable.FromEvent<EventHandler<ValueChangedEventArgs<TValue>>, ValueChangedEventArgs<TValue>>(
                    eh => source.ValueChanged += eh,
                    eh => source.ValueChanged -= eh)
                    .Select(
                        x => EventPair.Create(source, x.NewValue)

                    );

        }



        public static IObservable<EventPair<PropertyContainer<TValue>, ValueChangedEventArgs<TValue>>>
            GetValueChangeEventArgObservable<TValue>(this PropertyContainer<TValue> source)
        {

            var eventArgSeq = Observable.FromEvent<EventHandler<ValueChangedEventArgs<TValue>>, ValueChangedEventArgs<TValue>>(
                    eh => source.ValueChanged += eh,
                    eh => source.ValueChanged -= eh);
            return eventArgSeq.Select(
                        x => EventPair.Create(source, x)
                    );
            ;
        }


        public static IObservable<EventPair<Object, TEventArgs>> GetRouterEventObservable<TEventArgs>

        (
            this MVVM.EventRouter.EventRouter.EventObject<TEventArgs> source

        )
                   where TEventArgs : EventArgs
        {

            var eventArgSeq = Observable.FromEvent<EventHandler<TEventArgs>, TEventArgs>(
                    eh => source.Event += eh,
                    eh => source.Event -= eh);
            return eventArgSeq.Select(x => EventPair.Create(source as object, x));
        }
    }


    public class ReactiveCommand : EventCommandBase, ICommand, IObservable<EventPair<ReactiveCommand, Object>>
    {



        protected Lazy<IObservable<EventPair<ReactiveCommand, object>>> m_LazyObservableExecute;
        protected Lazy<IObserver<bool>> m_LazyObserverCanExecute;
        protected bool m_CurrentCanExecuteObserverValue;

        protected ReactiveCommand()
        {
            ConfigReactive();

        }

        public ReactiveCommand(bool canExecute = false)
            : this()
        {
            m_CurrentCanExecuteObserverValue = canExecute;
        }


        virtual protected void ConfigReactive()
        {
            m_LazyObservableExecute = new Lazy<IObservable<EventPair<ReactiveCommand, object>>>
            (
                () =>
                    Observable.FromEvent<Action<EventCommandBase, object>, object>
                (
                    eh => this.CommandExecute += eh,
                    eh => this.CommandExecute -= eh
                )
                .Select(e => EventPair.Create(this, e))
            );

            m_LazyObserverCanExecute = new Lazy<IObserver<bool>>
            (
                () =>
                    Observer.Create<bool>(
                    canExe =>
                    {
                        var oldv = this.m_CurrentCanExecuteObserverValue;
                        m_CurrentCanExecuteObserverValue = canExe;
                        if (oldv != canExe)
                        {
                            OnCanExecuteChanged();
                        }
                    }
                    )

            );
        }
        public IObserver<bool> CanExecuteObserver { get { return m_LazyObserverCanExecute.Value; } }

        public override bool CanExecute(object parameter)
        {
            return m_CurrentCanExecuteObserverValue;
        }


        public virtual IDisposable Subscribe(IObserver<EventPair<ReactiveCommand, object>> observer)
        {
            return m_LazyObservableExecute
                .Value
                .Subscribe(observer);
        }



    }


    public enum ExeuteBehavior
    {

        CannotExecute,
        CanExecuteCancelRunningTask,
        CanExecuteIgnoreRuningTask

    }


    public class ReactiveAsyncCommand : ReactiveAsyncCommand<object>
    {
        protected ReactiveAsyncCommand(ExeuteBehavior behavior )
            : base(behavior)
        {

        }

        public ReactiveAsyncCommand(bool canExecute = false, ExeuteBehavior behavior = Reactive.ExeuteBehavior.CannotExecute)
            : base(canExecute, behavior)
        {
        }

    }


    public class ReactiveAsyncCommand<TProgress> : EventCommandBase, ICommand, IObservable<EventPair<Func<ReactiveAsyncCommand<TProgress>.AsyncRunningDisposableContext>, object>>
    {
        public ReactiveAsyncCommand(ExeuteBehavior behavior = Reactive.ExeuteBehavior.CannotExecute)
        {
            ExeuteBehavior = behavior;
            CancellationTokenSource = new System.Threading.CancellationTokenSource();
            ConfigReactive();
        }

        public ReactiveAsyncCommand(bool canExecute = false, ExeuteBehavior behavior = Reactive.ExeuteBehavior.CannotExecute)
            : this(behavior)
        {
            m_CurrentCanExecuteObserverValue = canExecute;
        }

        protected Lazy<IObservable<EventPair<Func<ReactiveAsyncCommand<TProgress>.AsyncRunningDisposableContext>, object>>> m_LazyObservableExecute;
        protected Lazy<IObserver<bool>> m_LazyObserverCanExecute;
        protected bool m_CurrentCanExecuteObserverValue;
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public ExeuteBehavior ExeuteBehavior { get; private set; }
        public IProgress<TProgress> Progress { get; set; }

        bool IsExecuting { get { return m_ExecutingCount != 0; } }
        internal int m_ExecutingCount = 0;
        public IDisposable Subscribe(IObserver<EventPair<Func<ReactiveAsyncCommand<TProgress>.AsyncRunningDisposableContext>, object>> observer)
        {
            return m_LazyObservableExecute.Value.Subscribe(
                    fac =>
                    {
                        if (IsExecuting && ExeuteBehavior == Reactive.ExeuteBehavior.CanExecuteCancelRunningTask)
                        {
                            if (CancellationTokenSource != null)
                            {
                                CancellationTokenSource.Cancel();
                            }

                        }
                        observer.OnNext(fac);

                    }
                );
        }



        virtual protected void ConfigReactive()
        {
            m_LazyObservableExecute = new Lazy<IObservable<EventPair<Func<ReactiveAsyncCommand<TProgress>.AsyncRunningDisposableContext>, object>>>
            (
                () =>
                    Observable.FromEvent<Action<EventCommandBase, object>, object>
                (
                    eh => this.CommandExecute += eh,
                    eh => this.CommandExecute -= eh
                )
                .Select(e =>
                    EventPair.Create(
                      new Func<AsyncRunningDisposableContext>
                      (
                          () =>
                            new AsyncRunningDisposableContext(this)
                            {
                                CancellationToken = CancellationTokenSource == null ? CancellationToken.None : CancellationTokenSource.Token,
                                Parameter = e,
                                Progress = this.Progress

                            }
                      ),
                      e
                      )
                )
            );

            m_LazyObserverCanExecute = new Lazy<IObserver<bool>>
            (
                () =>
                    Observer.Create<bool>(
                    canExe =>
                    {
                        var oldv = this.m_CurrentCanExecuteObserverValue;
                        m_CurrentCanExecuteObserverValue = canExe;
                        if (oldv != canExe)
                        {
                            OnCanExecuteChanged();
                        }
                    }
                    )

            );
        }
        public IObserver<bool> CanExecuteObserver { get { return m_LazyObserverCanExecute.Value; } }

        public override bool CanExecute(object parameter)
        {
            switch (this.ExeuteBehavior)
            {
                case ExeuteBehavior.CannotExecute:
                    return m_CurrentCanExecuteObserverValue && (!IsExecuting);

                default:
                    return m_CurrentCanExecuteObserverValue;
            }
        }


        public class AsyncRunningDisposableContext : IDisposable
        {
            internal AsyncRunningDisposableContext(ReactiveAsyncCommand<TProgress> command)
            {
                Command = command;
                Interlocked.Increment(ref Command.m_ExecutingCount);
                Command.OnCanExecuteChanged();

            }
            public ReactiveAsyncCommand<TProgress> Command { get; set; }

            public CancellationToken CancellationToken { get; set; }
            public IProgress<TProgress> Progress { get; set; }
            public Object Parameter { get; set; }
            public void Dispose()
            {
                Interlocked.Decrement(ref Command.m_ExecutingCount);
                Command.OnCanExecuteChanged();
            }
        }



    }

}