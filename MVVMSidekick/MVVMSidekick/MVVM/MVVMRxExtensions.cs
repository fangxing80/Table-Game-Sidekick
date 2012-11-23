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
using MVVMSidekick.ViewModels;
using System.Windows.Input;
using System.Reactive.Subjects;
using System.Reactive;
using MVVMSidekick.Commands;
using MVVMSidekick.EventRouter;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace MVVMSidekick.Reactive
{


    public static class EventTuple
    {
        public static EventTuple<TSource, TEventArgs> Create<TSource, TEventArgs>(TSource source, TEventArgs eventArgs)
        {
            return new EventTuple<TSource, TEventArgs> { Source = source, EventArgs = eventArgs };
        }

    }
    public struct EventTuple<TSource, TEventArgs>
    {
        public TSource Source { get; set; }
        public TEventArgs EventArgs { get; set; }
    }

    public static class MVVMRxExtensions
    {
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> GetCollectionChangedObservable<T>(this ObservableCollection<T> source)
        {
            var rval = Observable
              .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
                  (
                      ev => source.CollectionChanged += ev,
                      ev => source.CollectionChanged -= ev
                  );
            return rval;
        }
        public static IObservable<EventTuple<ValueContainer<TValue>, TValue>> GetValueChangedObservable<TValue>
            (
                this ValueContainer<TValue> source

            )
        {

            return Observable.FromEventPattern<EventHandler<ValueChangedEventArgs<TValue>>, ValueChangedEventArgs<TValue>>(
                    eh => source.ValueChanged += eh,
                    eh => source.ValueChanged -= eh)
                    .Select(
                        x => EventTuple.Create(source, x.EventArgs.NewValue)

                    );

        }

        public static IObservable<EventTuple<ValueContainer<TValue>, ValueChangedEventArgs<TValue>>>
            GetValueChangedEventArgObservable<TValue>(this ValueContainer<TValue> source)
        {

            var eventArgSeq = Observable.FromEventPattern<EventHandler<ValueChangedEventArgs<TValue>>, ValueChangedEventArgs<TValue>>(
                    eh => source.ValueChanged += eh,
                    eh => source.ValueChanged -= eh);
            return eventArgSeq.Select(
                        x => EventTuple.Create(source, x.EventArgs)
                    );
            ;
        }


        public static IObservable<object> GetValueChangedObservableWithoutArgs<TValue>(this ValueContainer<TValue> source)
        {

            var eventArgSeq = Observable.FromEventPattern<EventHandler<ValueChangedEventArgs<TValue>>, ValueChangedEventArgs<TValue>>(
                    eh => source.ValueChanged += eh,
                    eh => source.ValueChanged -= eh);
            return eventArgSeq.Select(
                        x => null as object
                    );
            ;
        }


        public static IObservable<RouterEventData<TEventArgs>>
            GetRouterEventObservable<TEventArgs>(this MVVMSidekick.EventRouter.EventRouter.EventObject<TEventArgs> source)
                   where TEventArgs : EventArgs
        {
            var eventArgSeq = Observable.FromEventPattern<EventHandler<RouterEventData<TEventArgs>>, RouterEventData<TEventArgs>>(
                eh => source.Event += eh,
                eh => source.Event -= eh)
                .Select(e =>
                    e.EventArgs);
            ;
            return eventArgSeq;
        }




    }



    public class ReactiveCommand : EventCommandBase, ICommand, IObservable<EventPattern<EventCommandEventArgs>>
    {



        protected Lazy<IObservable<EventPattern<EventCommandEventArgs>>> _LazyObservableExecute;
        protected Lazy<IObserver<bool>> _LazyObserverCanExecute;
        protected bool _CurrentCanExecuteObserverValue;

        protected ReactiveCommand()
        {
            ConfigReactive();

        }

        public ReactiveCommand(bool canExecute = false)
            : this()
        {
            _CurrentCanExecuteObserverValue = canExecute;
        }


        virtual protected void ConfigReactive()
        {
            _LazyObservableExecute = new Lazy<IObservable<EventPattern<EventCommandEventArgs>>>
            (
                () =>
                {
                    var ob = Observable.FromEventPattern<EventHandler<EventCommandEventArgs>, EventCommandEventArgs>
                (
                    eh =>
                    {
                        this.CommandExecute += eh;
                    },
                    eh =>
                    {
                        this.CommandExecute -= eh;
                    }
                );

                    return ob;
                }
            );

            _LazyObserverCanExecute = new Lazy<IObserver<bool>>
            (
                () =>
                    Observer.Create<bool>(
                    canExe =>
                    {
                        var oldv = this._CurrentCanExecuteObserverValue;
                        _CurrentCanExecuteObserverValue = canExe;
                        if (oldv != canExe)
                        {
                            OnCanExecuteChanged();
                        }
                    }
                    )

            );
        }
        public IObserver<bool> CanExecuteObserver { get { return _LazyObserverCanExecute.Value; } }

        public override bool CanExecute(object parameter)
        {
            return _CurrentCanExecuteObserverValue;
        }






        public IDisposable Subscribe(IObserver<EventPattern<EventCommandEventArgs>> observer)
        {
            return _LazyObservableExecute
                  .Value
                  .Subscribe(observer);
        }
    }


    public enum ExeuteBehavior
    {

        CannotExecute,
        CanExecuteCancelRunningTask

    }


    public class ReactiveAsyncCommand : ReactiveAsyncCommand<object>
    {
        public ReactiveAsyncCommand(ExeuteBehavior behavior)
            : base(behavior)
        {

        }

        public ReactiveAsyncCommand(bool canExecute = false, ExeuteBehavior behavior = Reactive.ExeuteBehavior.CannotExecute)
            : base(canExecute, behavior)
        {
        }

    }


    public class ReactiveAsyncCommand<TProgress> : EventCommandBase, ICommand, IObservable<EventTuple<Func<ReactiveAsyncCommand<TProgress>.AsyncRunningDisposableContext>, object>>
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
            _CurrentCanExecuteObserverValue = canExecute;
        }

        protected Lazy<IObservable<EventTuple<Func<ReactiveAsyncCommand<TProgress>.AsyncRunningDisposableContext>, object>>> _LazyObservableExecute;
        protected Lazy<IObserver<bool>> _LazyObserverCanExecute;
        protected bool _CurrentCanExecuteObserverValue;
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public ExeuteBehavior ExeuteBehavior { get; private set; }
        public IProgress<TProgress> Progress { get; set; }

        bool IsExecuting { get { return _ExecutingCount != 0; } }
        internal int _ExecutingCount = 0;
        public IDisposable Subscribe(IObserver<EventTuple<Func<ReactiveAsyncCommand<TProgress>.AsyncRunningDisposableContext>, object>> observer)
        {
            return _LazyObservableExecute.Value.Subscribe(
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
            _LazyObservableExecute = new Lazy<IObservable<EventTuple<Func<ReactiveAsyncCommand<TProgress>.AsyncRunningDisposableContext>, object>>>
            (
                () =>
                    Observable.FromEventPattern<EventHandler<EventCommandEventArgs>, EventCommandEventArgs>
                (
                    eh => this.CommandExecute += eh,
                    eh => this.CommandExecute -= eh
                )
                .Select(e =>
                    EventTuple.Create(
                      new Func<AsyncRunningDisposableContext>
                      (
                          () =>
                            new AsyncRunningDisposableContext(this)
                            {
                                CancellationToken = CancellationTokenSource == null ? CancellationToken.None : CancellationTokenSource.Token,
                                Parameter = e.EventArgs.Parameter,
                                Progress = this.Progress

                            }
                      ),
                      e.EventArgs.Parameter
                      )
                )
            );

            _LazyObserverCanExecute = new Lazy<IObserver<bool>>
            (
                () =>
                    Observer.Create<bool>(
                    canExe =>
                    {
                        var oldv = this._CurrentCanExecuteObserverValue;
                        _CurrentCanExecuteObserverValue = canExe;
                        if (oldv != canExe)
                        {
                            OnCanExecuteChanged();
                        }
                    }
                    )

            );
        }
        public IObserver<bool> CanExecuteObserver { get { return _LazyObserverCanExecute.Value; } }

        public override bool CanExecute(object parameter)
        {
            switch (this.ExeuteBehavior)
            {
                case ExeuteBehavior.CannotExecute:
                    return _CurrentCanExecuteObserverValue && (!IsExecuting);

                default:
                    return _CurrentCanExecuteObserverValue;
            }
        }


        public class AsyncRunningDisposableContext : IDisposable
        {
            internal AsyncRunningDisposableContext(ReactiveAsyncCommand<TProgress> command)
            {
                Command = command;
                Interlocked.Increment(ref Command._ExecutingCount);
                Command.OnCanExecuteChanged();

            }
            public ReactiveAsyncCommand<TProgress> Command { get; set; }

            public CancellationToken CancellationToken { get; set; }
            public IProgress<TProgress> Progress { get; set; }
            public Object Parameter { get; set; }
            public void Dispose()
            {
                Interlocked.Decrement(ref Command._ExecutingCount);
                Command.OnCanExecuteChanged();
            }
        }



    }

}