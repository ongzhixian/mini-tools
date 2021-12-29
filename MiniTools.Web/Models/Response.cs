using MiniTools.Web.DataEntities;

namespace MiniTools.Web.Models;

public interface IResponse
{
    bool Success { get; }
}

public interface IResponse<out T> : IResponse where T : class
{
    T Payload { get; }
}

public sealed class Response : IResponse
{
    public bool Success { get; private init; }

    public Response(bool success)
    {
        this.Success = success;
    }

    private static readonly Lazy<Response> invalid = new Lazy<Response>(() => new Response(false));
    private static readonly Lazy<Response> valid = new Lazy<Response>(() => new Response(true));

    public static Response Invalid { get { return invalid.Value; } }
    public static Response Valid { get { return valid.Value; } }
}

public sealed class Response<T> : IResponse<T> where T : class
{
    public bool Success { get; private init; }

    public T Payload { get; private init; }

    public Response(bool success, T payload)
    {
        this.Success = success;
        this.Payload = payload;
    }

    private static readonly Lazy<Response> invalid = new Lazy<Response>(() => new Response(false));
    private static readonly Lazy<Response> valid = new Lazy<Response>(() => new Response(true));

    public static Response Invalid { get { return invalid.Value; } }
    public static Response Valid { get { return valid.Value; } }
}

public sealed class RecordNotFound : IResponse
{
    public bool Success => false;

    private static readonly Lazy<RecordNotFound> singleInstance = new Lazy<RecordNotFound>(() => new RecordNotFound());

    public static RecordNotFound Value { get { return singleInstance.Value; } }

    private RecordNotFound()
    {
    }
}

public sealed class InvalidModel : IResponse
{
    public bool Success => false;

    private static readonly Lazy<InvalidModel> singleInstance = new Lazy<InvalidModel>(() => new InvalidModel());

    public static InvalidModel Value { get { return singleInstance.Value; } }

    private InvalidModel()
    {
    }
}


public sealed class InvalidModel<T> : IResponse<T> where T : class
{
    public T Payload { get; private init; }

    public bool Success => false;

    public InvalidModel(T payload)
    {
        Payload = payload;
    }

    public static IResponse<T> Value(T payload)
    {
        return new InvalidModel<T>(payload);
    }

}

// RecordFound


public sealed class RecordFound<T> : IResponse<T> where T : class
{
    public bool Success => true;

    public T Payload { get; private init; }

    public RecordFound(T payload)
    {
        Payload = payload;
    }

    public static IResponse<T> Value(T payload)
    {
        return new RecordFound<T>(payload);
    }
}


//public sealed class Response 
//{
//    private static readonly Lazy<PositiveResponse> positiveResponse = new Lazy<PositiveResponse>(() => new PositiveResponse());
//    private static readonly Lazy<NegativeResponse> negativeResponse = new Lazy<NegativeResponse>(() => new NegativeResponse());

//    //public static IResponse Positive 
//    //{ 
//    //    get 
//    //    { 
//    //        return (IResponse)positiveResponse; 
//    //    } 
//    //}

//    public static IResponse Negative
//    {
//        get
//        {
//            return (IResponse)negativeResponse;
//        }
//    }


//    public static IResponse<T> Positive<T>(T payload) where T : class
//    {
//        return new PositiveResponse<T>(payload);
//    }

//    static Response()
//    {

//    }

//    private Response()
//    {
//    }
//}


