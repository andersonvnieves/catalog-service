namespace br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();
}