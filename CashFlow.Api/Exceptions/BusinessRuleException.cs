namespace CashFlow.Api.Exceptions;

public class BusinessRuleException(string message) : AppException(message)
{
    public override int StatusCode => 422;
}