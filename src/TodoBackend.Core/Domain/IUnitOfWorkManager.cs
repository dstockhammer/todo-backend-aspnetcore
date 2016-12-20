namespace TodoBackend.Core.Domain
{
    public interface IUnitOfWorkManager
    {
        IUnitOfWork Start();
    }
}
