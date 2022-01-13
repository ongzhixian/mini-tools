namespace MiniTools.HostApp.Services;

public sealed class EventBus<T> where T : notnull
{
    public event EventHandler<T>? NewData;

    private static readonly Lazy<EventBus<T>> lazy = new Lazy<EventBus<T>>(() => new EventBus<T>());

    public static EventBus<T> Instance { get { return lazy.Value; } }

    private EventBus()
    {
    }

    public void Send(object sender, T data) => NewData?.Invoke(sender, data);
}