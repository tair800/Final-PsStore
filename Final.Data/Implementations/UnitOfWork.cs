using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FinalDbContext _context;


        public ICategoryRepository categoryRepository { get; private set; }
        public IGameRepository gameRepository { get; private set; }
        public IDlcRepository dlcRepository { get; private set; }
        public IBasketRepository basketRepository { get; private set; }
        public IUserRepository userRepository { get; private set; }
        public IBasketGameRepository basketGameRepository { get; private set; }


        public ICategoryRepository CategoryRepository => throw new NotImplementedException();
        public IGameRepository GameRepository => throw new NotImplementedException();
        public IDlcRepository DlcRepository => throw new NotImplementedException();
        public IBasketRepository BasketRepository => throw new NotImplementedException();
        public IUserRepository UserRepository => throw new NotImplementedException();
        public IBasketGameRepository BasketGameRepository => throw new NotImplementedException();


        public UnitOfWork(FinalDbContext context)
        {
            _context = context;
            categoryRepository = new CategoryRepository(_context);
            gameRepository = new GameRepository(_context);
            dlcRepository = new DlcRepository(_context);
            basketRepository = new BasketRepository(_context);
            userRepository = new UserRepository(_context);
            basketGameRepository = new BasketGameRepository(_context);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
