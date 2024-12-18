﻿using Final.Core.Repositories;
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
        public ISettingRepository settingsRepository { get; private set; }
        public IWishlistRepository wishlistRepository { get; private set; }
        public IWishlistGameRepository wishlistGameRepository { get; private set; }
        public ICommentRepository commentRepository { get; private set; }
        public IPromoRepository promoRepository { get; private set; }
        public ICommentHistoryRepository commentHistoryRepository { get; private set; }
        public IOrderRepository orderRepository { get; private set; }
        public IUserCardRepository userCardRepository { get; private set; }
        public IRatingRepository ratingRepository { get; private set; }
        public ICommentReactionRepository commentReactionRepository { get; private set; }


        public ICategoryRepository CategoryRepository => throw new NotImplementedException();
        public IGameRepository GameRepository => throw new NotImplementedException();
        public IDlcRepository DlcRepository => throw new NotImplementedException();
        public IBasketRepository BasketRepository => throw new NotImplementedException();
        public IUserRepository UserRepository => throw new NotImplementedException();
        public IBasketGameRepository BasketGameRepository => throw new NotImplementedException();
        public ISettingRepository SettingsRepository => throw new NotImplementedException();
        public IWishlistRepository WishlistRepository => throw new NotImplementedException();
        public IWishlistGameRepository WishlistGameRepository => throw new NotImplementedException();
        public ICommentRepository CommentRepository => throw new NotImplementedException();
        public IPromoRepository PromoRepository => throw new NotImplementedException();
        public ICommentHistoryRepository CommentHistoryRepository => throw new NotImplementedException();
        public IOrderRepository OrderRepository => throw new NotImplementedException();
        public IUserCardRepository UserCardRepository => throw new NotImplementedException();
        public IRatingRepository RatingRepository => throw new NotImplementedException();
        public ICommentReactionRepository CommentReactionRepository => throw new NotImplementedException();


        public UnitOfWork(FinalDbContext context)
        {
            _context = context;
            categoryRepository = new CategoryRepository(_context);
            gameRepository = new GameRepository(_context);
            dlcRepository = new DlcRepository(_context);
            basketRepository = new BasketRepository(_context);
            userRepository = new UserRepository(_context);
            basketGameRepository = new BasketGameRepository(_context);
            settingsRepository = new SettingRepository(_context);
            wishlistRepository = new WishlistRepository(_context);
            wishlistGameRepository = new WishlistGameRepository(_context);
            commentRepository = new CommentRepository(_context);
            promoRepository = new PromoRepository(_context);
            commentHistoryRepository = new CommentHistoryRepository(_context);
            orderRepository = new OrderRepository(_context);
            userCardRepository = new UserCardRepository(_context);
            ratingRepository = new RatingRepository(_context);
            commentReactionRepository = new CommentReactionRepository(_context);
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
