namespace CashFlow.Api.Exceptions;

public class ConflictException(string message) : AppException(message)
{
    public override int StatusCode => 409;
}