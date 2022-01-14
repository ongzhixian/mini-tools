namespace MiniTools.HostApp.Services;

/// <summary>
/// This approach creates a bus for each event type.
/// </summary>
/// <typeparam name="T">T is some event type. (example: PriceUpdateEvent)</typeparam>
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
