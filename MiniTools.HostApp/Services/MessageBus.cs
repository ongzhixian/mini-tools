namespace MiniTools.HostApp.Services;


internal sealed class Subscriber<T> : IDisposable 
    where T : notnull
{
    private readonly List<IObserver<T>> _observers;
    private readonly IObserver<T> _observer;

    public Subscriber(List<IObserver<T>> observers, IObserver<T> observer)
    {
        _observers = observers;
        _observer = observer;
    }

    public void Dispose()
    {
        if (_observer != null)
            _observers.Remove(_observer);
    }
}

/// <summary>
/// This approach creates a bus for each event type.
/// </summary>
/// <typeparam name="T">T is some event type. (example: PriceUpdateEvent)</typeparam>
public class MessageBus<T> where T : notnull
{
    private readonly List<IObserver<T>> observers;

    private static readonly Lazy<MessageBus<T>> lazy = new Lazy<MessageBus<T>>(() => new MessageBus<T>());

    public static MessageBus<T> Instance { get { return lazy.Value; } }

    public MessageBus()
    {
        observers = new List<IObserver<T>>();
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);

        return new Subscriber<T>(observers, observer);

        //return new Unsubscriber(observers, observer)
    }

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

    public void Send(T message)
    {
        foreach (var observer in observers.ToArray())
            observer.OnNext(message);
    }
}


public class MessageBusSubscriber<T> : IObserver<T> where T : notnull
{
    /// <summary>
    /// Action to carry out when new value is received
    /// </summary>
    public Action<T>? NewValueAction { get; set; }

    public MessageBusSubscriber(MessageBus<T> messageBus)
    {
        messageBus.Subscribe(this);
    }

    public virtual void OnCompleted()
    {
    }

    public virtual void OnError(Exception error)
    {
    }

    public void OnNext(T value)
    {
        NewValueAction?.Invoke(value);
    }

}