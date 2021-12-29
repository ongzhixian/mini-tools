
namespace MiniTools.Web.Models
{

    public class OperationResult<TResult> where TResult : new()
    {
        private static class On
        {
            internal static readonly EventId SUCCESS = new EventId(1, "SUCCESS");
            internal static readonly EventId FAIL = new EventId(5, "FAIL");
        }

        public bool Success { get; private init; }

        public TResult Payload { get; private init; }

        public EventId EventId { get; private init; }

        public OperationResult(bool success, TResult payload) : this(success)
        {
            Payload = payload;
        }

        public OperationResult(bool success, EventId eventId) : this(success)
        {
            EventId = eventId;
        }

        public OperationResult(bool success)
        {
            Success = success;
            EventId = success ? On.SUCCESS : On.FAIL;
        }

        public OperationResult(bool success, EventId eventId, TResult resultObj) : this(success, eventId)
        {
            this.Payload = resultObj;
        }

        private static readonly Lazy<OperationResult<TResult>> fail =
            new Lazy<OperationResult<TResult>>(() => new OperationResult<TResult>(false));

        public static OperationResult<TResult> Fail()
        {
            return fail.Value;
        }

        public static OperationResult<TResult> Fail(EventId eventId)
        {
            return new OperationResult<TResult>(false, eventId);
        }


        private static readonly Lazy<OperationResult<TResult>> success =
            new Lazy<OperationResult<TResult>>(() => new OperationResult<TResult>(true));

        public static OperationResult<TResult> Ok()
        {
            return success.Value;
        }

        public static OperationResult<TResult> Ok(EventId eventId)
        {
            return new OperationResult<TResult>(true, eventId);
        }

        public static OperationResult<TResult> Ok(EventId eventId, TResult resultObj)
        {
            return new OperationResult<TResult>(true, eventId, resultObj);
        }
    }
}