namespace tivitApi.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso não é encontrado no banco de dados
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    
    public NotFoundException(string entityName, object key) 
        : base($"{entityName} com identificador '{key}' não foi encontrado.") { }
}
