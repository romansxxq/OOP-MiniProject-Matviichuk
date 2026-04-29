namespace MedCore.Application.Common;
public class Result
{
    public bool IsSuccess { get; }
    public string Message { get; }
    private Result(bool success, string msg) { IsSuccess = success; Message = msg; }
    public static Result Success(string msg) => new(true, msg);
    public static Result Failure(string msg) => new(false, msg);
}