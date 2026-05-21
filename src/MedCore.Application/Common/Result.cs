namespace MedCore.Application.Common;
public class Result
{
    public bool IsSuccess { get; }
    public string Message { get; }
    protected Result(bool success, string msg) { IsSuccess = success; Message = msg; }
    public static Result Success(string msg) => new(true, msg);
    public static Result Failure(string msg) => new(false, msg);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool success, string msg, T? value) : base(success, msg)
    {
        Value = value;
    }

    public static Result<T> Success(T value, string msg) => new(true, msg, value);
    public new static Result<T> Failure(string msg) => new(false, msg, default);
}