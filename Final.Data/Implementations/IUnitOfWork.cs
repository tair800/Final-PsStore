using Final.Core.Repositories;

namespace Final.Data.Implementations
{
    public interface IUnitOfWork
    {
        void Commit();
        public ICategoryRepository categoryRepository { get; }
        public IGameRepository gameRepository { get; }
        public IDlcRepository dlcRepository { get; }
        public IBasketRepository basketRepository { get; }
        public IUserRepository userRepository { get; }
        public IBasketGameRepository basketGameRepository { get; }
    }
}
