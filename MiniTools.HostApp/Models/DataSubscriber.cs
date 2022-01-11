namespace MiniTools.HostApp.Models;

public interface IDataSubscriber<in T> : IObserver<T> where T : new()
{
    void Subscribe(IObservable<T> provider);

    void Unsubscribe();
}

public interface IStructDataSubscriber<in T> : IObserver<T> where T : struct
{
    void Subscribe(IObservable<T> provider);

    void Unsubscribe();
}