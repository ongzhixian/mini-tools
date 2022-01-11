namespace MiniTools.HostApp.Models;

public class DataSubscription<T> : IObservable<T>
{
    private readonly List<IObserver<T>> observers;

    public List<IObserver<T>> Observers => observers;

    public DataSubscription()
    {
        observers = new List<IObserver<T>>();
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);

        return new Unsubscriber(observers, observer);
    }

    private sealed class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
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
}

public class WeatherForecastSubscription : IObservable<WeatherForecast>
{
    private readonly List<IObserver<WeatherForecast>> observers;

    public List<IObserver<WeatherForecast>> Observers => observers;

    public WeatherForecastSubscription()
    {
        observers = new List<IObserver<WeatherForecast>>();
    }


    public IDisposable Subscribe(IObserver<WeatherForecast> observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);

        return new Unsubscriber(observers, observer);
    }

    private sealed class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<WeatherForecast>> _observers;
        private readonly IObserver<WeatherForecast> _observer;

        public Unsubscriber(List<IObserver<WeatherForecast>> observers, IObserver<WeatherForecast> observer)
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
}
