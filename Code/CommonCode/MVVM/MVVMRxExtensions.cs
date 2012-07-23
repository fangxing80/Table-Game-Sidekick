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

        public TEventArgs  EventArgs { get; set; }

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
                        x => EventPair.Create (source, x.NewValue)

                    );

        }



        public static IObservable<EventPair<PropertyContainer<TValue>, ValueChangedEventArgs<TValue>>> GetValueChangeEventArgObservable<TValue>
        (
            this PropertyContainer<TValue> source

        )
        {

            var eventArgSeq = Observable.FromEvent<EventHandler<ValueChangedEventArgs<TValue>>, ValueChangedEventArgs<TValue>>(
                    eh => source.ValueChanged += eh,
                    eh => source.ValueChanged -= eh);
            return eventArgSeq.Select(
                        x => EventPair.Create(source, x)
                    );
            ;
        }


        public static IObservable<EventPair<Object, TEventArgs>> GetRouterEvent<TEventArgs>
     
        (
            this MVVM.EventRouter.EventRouter.EventObject<TEventArgs> source

        )
                   where TEventArgs : EventArgs
        {

            var eventArgSeq = Observable.FromEvent<EventHandler<TEventArgs>, TEventArgs>(
                    eh => source.Event  += eh,
                    eh => source.Event -= eh);
            return eventArgSeq.Select(x=>EventPair.Create (source as object  ,x) );
        }
    }

}