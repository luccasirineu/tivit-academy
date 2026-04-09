namespace tivitApi.Exceptions;

/// <summary>
/// Exceção lançada quando há erro de validação de dados
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
