public class FailedResponse : IFailedResponse
{
    public string Error { get; set; }

    public string Code { get; set; }

    string IFailedResponse.ErrorMessage => Error;

    string IFailedResponse.ErrorCode => Code;
}
