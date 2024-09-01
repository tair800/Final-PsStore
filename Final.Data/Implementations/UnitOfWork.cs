using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FinalDbContext _context;
        public ICategoryRepository categoryRepository { get; private set; }
        public IGameRepository gameRepository { get; private set; }
        public IGamePlatformRepository gamePlatformRepository { get; private set; }


        public ICategoryRepository CategoryRepository => throw new NotImplementedException();
        public IGameRepository GameRepository => throw new NotImplementedException();
        public IGamePlatformRepository GamePlatformRepository => throw new NotImplementedException();


        public UnitOfWork(FinalDbContext context)
        {
            _context = context;
            categoryRepository = new CategoryRepository(_context);
            gameRepository = new GameRepository(_context);
            gamePlatformRepository = new GamePlatformRepository(_context);
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
