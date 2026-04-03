namespace FGV.Domain.Sorting;

/// <summary>
/// Exceção lançada quando ocorre um erro durante a ordenação
/// </summary>
public class SortingException : Exception
{
    public SortingException() : base()
    {
    }

    public SortingException(string message) : base(message)
    {
    }

    public SortingException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
