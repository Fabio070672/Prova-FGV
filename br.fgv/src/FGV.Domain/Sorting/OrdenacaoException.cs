namespace FGV.Domain.Sorting;

/// <summary>
/// Exceção lançada quando há problemas na ordenação de livros
/// </summary>
public class OrdenacaoException : Exception
{
    public OrdenacaoException()
        : base("Erro ao ordenar livros")
    {
    }

    public OrdenacaoException(string message)
        : base(message)
    {
    }

    public OrdenacaoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
