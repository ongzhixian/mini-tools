namespace MiniTools.HostApp.Services;

public record struct PriceUpdateEvent;

public interface ISubscription<T> where T : notnull
{
    Action<T>? OnNewData { get; }
}

public sealed class Subscription<T> : IDisposable, ISubscription<T> where T : notnull
{
    readonly IList<Subscription<T>> subscriptions;
    public Action<T>? OnNewData { get; internal set; }

    public Subscription(IList<Subscription<T>> subscriptions)
    {
        this.subscriptions = subscriptions;
        this.subscriptions.Add(this);
    }

    public void Dispose()
    {
        if (subscriptions.Contains(this))
            subscriptions.Remove(this);
    }
}

public class SubscriptionDictionary
{
    readonly Dictionary<string, object> subListDictionary = new Dictionary<string, object>();

    public object this[string index]
    {
        get { return subListDictionary[index]; }
        set { subListDictionary[index] = value; }
    }

    public SubscriptionDictionary()
    {
    }

    internal bool ContainsKey(string key)
    {
        return subListDictionary.ContainsKey(key);
    }

    private List<Subscription<T>> GetSubscriptionList<T>() where T : notnull
    {
        string key = typeof(T).GUID.ToString();

        if (!ContainsKey(key))
            subListDictionary.Add(key, new List<Subscription<T>>());

        return (List<Subscription<T>>)subListDictionary[key];
    }

    internal void Send<T>(T data) where T : notnull
    {
        foreach (Subscription<T> subscription in GetSubscriptionList<T>())
        {
            subscription.OnNewData?.Invoke(data);
        }
    }

    internal Subscription<T> GetNewSubscription<T>() where T : notnull
    {
        var newSubscription = new Subscription<T>(GetSubscriptionList<T>());

        return newSubscription;
    }
}


public class MessageBus22
{
    private static readonly Lazy<MessageBus22> lazy = new Lazy<MessageBus22>(() => new MessageBus22());

    public static MessageBus22 Instance { get { return lazy.Value; } }

    private readonly SubscriptionDictionary subscriptions;

    public MessageBus22()
    {
        subscriptions = new SubscriptionDictionary();
    }

    public Subscription<T> Subscribe<T>() where T : notnull
    {
        return subscriptions.GetNewSubscription<T>();
    }

    internal void Send<T>(T data) where T : notnull
    {
        subscriptions.Send<T>(data);
    }
}


public class MessageBus22Draft
{
    private static readonly Lazy<MessageBus22Draft> lazy = new Lazy<MessageBus22Draft>(() => new MessageBus22Draft());

    public static MessageBus22Draft Instance { get { return lazy.Value; } }

    private readonly SubscriptionDictionary subscriptions;

    public MessageBus22Draft()
    {
        subscriptions = new SubscriptionDictionary();
    }

    public Subscription<T> Subscribe<T>() where T : notnull
    {
        //string key = typeof(T).GUID.ToString();

        //if (!subscriptions.ContainsKey(key))
        //    subscriptions.AddNewList<T>();

        //var subscription = CreateNewSubscription<T>(subscriptions[key] as List<Subscription<T>>);

        //subscriptions.Add<T>(subscription);

        Subscription<T> subscription2 = subscriptions.GetNewSubscription<T>();

        return subscription2;


        //var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
        //await(Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });

        //string key = typeof(T).GUID.ToString();

        //if (!subscriptions.ContainsKey(key))
        //{
        //    subscriptions.Add(key, new List<IObserver<T>>());
        //}

        //Observer<T> obs = new Observer<T>();
        //Subscription<T> subscription = new Subscription<T>(subscriptions[key], obs);
        //subscriptions[key].Add(subscription);
        //return subscription;
    }

    private Subscription<T> CreateNewSubscription<T>(List<Subscription<T>> list) where T : notnull
    {
        var newSubscription = new Subscription<T>(list);
        return newSubscription;
    }

    //private Subscription CreateNewSubscription<T>(IList<Subscription> subscriptions) where T : notnull
    //{
    //    var newSubscription = new Subscription(subscriptions);
    //    return newSubscription;
    //}

    internal void Send<T>(T data) where T : notnull
    {
        //string key = typeof(T).GUID.ToString();

        //if (!subscriptions.ContainsKey(key))
        //    return; // do nothing

        // subscriptions[key].Each(Send(data));
        //foreach (var subscription in subscriptions[key])
        //{
        //    subscription.Send(data);
        //}

        subscriptions.Send<T>(data);
    }


    //public void Send<T>(T data) where T : notnull
    //{
    //    string key = typeof(T).GUID.ToString();

    //    foreach (Subscription<T> subscription in subscriptions[key])
    //    {
    //        subscription.Send(data);
    //    }
    //}

    //public IDisposable Subscribe(IObserver<T> observer)
    //{
    //    if (!observers.Contains(observer))
    //        observers.Add(observer);

    //    return new Subscriber<T>(observers, observer);

    //    //return new Unsubscriber(observers, observer)
    //}

    //private sealed class Unsubscriber : IDisposable
    //{
    //    private readonly List<IObserver<T>> _observers;
    //    private readonly IObserver<T> _observer;

    //    public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
    //    {
    //        _observers = observers;
    //        _observer = observer;
    //    }

    //    public void Dispose()
    //    {
    //        if (_observer != null)
    //            _observers.Remove(_observer);
    //    }
    //}

    //public void Send(T message)
    //{
    //    foreach (var observer in observers.ToArray())
    //        observer.OnNext(message);
    //}
}


//public class MessageBusSubscriber<T> : IObserver<T> where T : notnull
//{
//    /// <summary>
//    /// Action to carry out when new value is received
//    /// </summary>
//    public Action<T>? NewValueAction { get; set; }

//    public MessageBusSubscriber(MessageBus<T> messageBus)
//    {
//        messageBus.Subscribe(this);
//    }

//    public virtual void OnCompleted()
//    {
//    }

//    public virtual void OnError(Exception error)
//    {
//    }

//    public void OnNext(T value)
//    {
//        NewValueAction?.Invoke(value);
//    }

//}




//internal sealed class Subscriber<T> : IDisposable 
//    where T : notnull
//{
//    private readonly List<IObserver<T>> _observers;
//    private readonly IObserver<T> _observer;

//    public Subscriber(List<IObserver<T>> observers, IObserver<T> observer)
//    {
//        _observers = observers;
//        _observer = observer;
//    }

//    public void Dispose()
//    {
//        if (_observer != null)
//            _observers.Remove(_observer);
//    }
//}



//public class Observer<T> : IObserver<T> where T : notnull
//{
//    public event EventHandler? CompletedEvent;
//    public event EventHandler<Exception>? ErrorEvent;
//    public event EventHandler<T>? NextEvent;

//    public void OnCompleted()
//    {
//        CompletedEvent?.Invoke(this, EventArgs.Empty);
//    }

//    public void OnError(Exception error)
//    {
//        ErrorEvent?.Invoke(this, error);
//    }

//    public void OnNext(T value)
//    {
//        NextEvent?.Invoke(this, value);
//    }
//}


//public sealed class Subscription<T> : IDisposable
//    where T : notnull
//{
//    private readonly System.Collections.IList _observers;
//    private readonly IObserver<T> _observer;

//    public Subscription(System.Collections.IList observers, IObserver<T> observer)
//    {
//        _observers = observers;
//        _observer = observer;
//    }

//    public void Dispose()
//    {
//        if (_observer != null)
//            _observers.Remove(_observer);
//    }

//    internal void Send<T>(T data)
//    {
//        _observer.OnNext(data);
//    }
//}

//public interface IIntegrationEventHandler
//{
//}

//public interface IEventHandler<in T> where T : notnull
//{
//    Task Send(T data);
//}



//public sealed class Subscription : IDisposable
//{
//    readonly IList<Subscription> subscriptions;

//    public Subscription(IList<Subscription> subscriptions)
//    {
//        this.subscriptions = subscriptions;
//    }

//    public Action<object> OnNewData { get; internal set; }

//    public void Dispose()
//    {
//        if (subscriptions.Contains(this))
//            subscriptions.Remove(this);
//    }

//    internal void Send<T>(T data) where T : notnull
//    {
//        OnNewData?.Invoke(data);
//    }
//}
