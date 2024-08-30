using Final.Core.Repositories;

namespace Final.Data.Implementations
{
    public interface IUnitOfWork
    {
        void Commit();
        public ICategoryRepository categoryRepository { get; }
        public IGameRepository gameRepository { get; }
    }
}
