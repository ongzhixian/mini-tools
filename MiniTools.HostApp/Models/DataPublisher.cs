namespace MiniTools.HostApp.Models;

public interface IDataPublisher<T> where T : new()
{
    Task PublishDataAsync(CancellationToken stopToken);
}

public interface IStructDataPublisher<T> where T : struct
{
    Task PublishDataAsync(CancellationToken stopToken);
}
