﻿using Final.Core.Repositories;

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
        public ISettingRepository settingsRepository { get; }
        public IWishlistRepository wishlistRepository { get; }
        public IWishlistGameRepository wishlistGameRepository { get; }
        public ICommentRepository commentRepository { get; }
        public IPromoRepository promoRepository { get; }
        public ICommentHistoryRepository commentHistoryRepository { get; }
        public IOrderRepository orderRepository { get; }
        public IUserCardRepository userCardRepository { get; }
        public IRatingRepository ratingRepository { get; }
        public ICommentReactionRepository commentReactionRepository { get; }
    }
}
