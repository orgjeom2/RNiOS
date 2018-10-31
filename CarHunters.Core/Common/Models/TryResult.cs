namespace CarHunters.Core.Common.Models
{
	public static class TryResult
	{
		public static TryResult<TResult> Create<TResult> (bool operationSucceeded, TResult result)
		{
			return new TryResult<TResult> (operationSucceeded, result);
		}

        public static TryResult<TResult> Unsucceed<TResult>()
        {
            return new TryResult<TResult>(false);
        }
	}

	public static class TryDetailResult
	{
		public static TryDetailResult<TResult> Create<TResult> (bool operationSucceeded, TResult result, string message = null)
		{
			return new TryDetailResult<TResult> (operationSucceeded, result, message);
		}

		public static TryDetailResult<TResult> Unsucceed<TResult> (string message = null)
		{
			return new TryDetailResult<TResult> (false, default (TResult));
		}
	}

    public static class TryApiResult
    {
        public static TryApiResult<TResult> Create<TResult>(bool operationSucceeded, TResult result, string errorKey="") 
        {
            return new TryApiResult<TResult>(operationSucceeded, result, errorKey);
        }

        public static TryApiResult<TResult> Unsucceed<TResult>(string errorKey = "")
        {
            return new TryApiResult<TResult>(false, default(TResult), errorKey);
        }
    }

	public class TryResult<TResult>
	{
		public bool OperationSucceeded { get; private set; }
		public TResult Result { get; private set; }

		public TryResult (bool operationSucceeded, TResult result = default (TResult))
		{
			OperationSucceeded = operationSucceeded;
			Result = result;
		}
	}

	public class TryApiResult<TResult> : TryResult<TResult>
	{
		public TryApiResult (bool operationSucceeded, TResult result, string errorKey) : base (operationSucceeded, result)
		{
		    ErrorKey = errorKey;
		}

		public string ErrorKey { get; private set; }
	}

	public class TryDetailResult<TResult> : TryResult<TResult>
	{
		public TryDetailResult (bool operationSucceeded, TResult result, string message = null) : base (operationSucceeded, result)
		{
			ExceptionMessage = message;
		}

		public string ExceptionMessage { get; private set; }
	}
}
