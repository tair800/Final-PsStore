using AutoMapper;
using Final.Application.Dtos.CommentDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Final.Application.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Create(CommentCreateDto commentCreateDto)
        {
            var comment = _mapper.Map<Comment>(commentCreateDto);

            try
            {
                await _unitOfWork.commentRepository.Create(comment);
                _unitOfWork.Commit();
                return comment.Id;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Error while adding comment: {dbEx.InnerException?.Message ?? dbEx.Message}");
                throw new Exception("An error occurred while saving the comment. Please check the database constraints and ensure the data is valid.");
            }
        }

        public async Task Delete(int id)
        {
            var comment = await _unitOfWork.commentRepository.GetEntity(c => c.Id == id);

            if (comment == null)
                throw new CustomExceptions(404, "Comment", "Comment not found.");

            await _unitOfWork.commentRepository.Delete(comment);

            _unitOfWork.Commit();
        }

        public async Task<List<CommentReturnDto>> GetAll(int? gameId = null)
        {
            IEnumerable<Comment> comments;

            if (gameId.HasValue)
            {
                comments = await _unitOfWork.commentRepository.GetAll(
                    c => c.GameId == gameId,
                    "Game", "User", "CommentReactions");
            }
            else
            {
                comments = await _unitOfWork.commentRepository.GetAll(
                    null, "Game", "User", "CommentReactions");
            }

            return _mapper.Map<List<CommentReturnDto>>(comments);
        }

        public async Task<CommentReturnDto> GetOne(int id)
        {
            var comment = await _unitOfWork.commentRepository.GetEntity(
                c => c.Id == id,
                "Game", "User", "CommentReactions");

            if (comment == null)
            {
                throw new CustomExceptions(404, "Comment", "Comment not found.");
            }

            return _mapper.Map<CommentReturnDto>(comment);
        }


        public async Task Update(int id, CommentUpdateDto updateDto)
        {
            var comment = await _unitOfWork.commentRepository.GetEntity(c => c.Id == id);

            if (comment == null)
                throw new CustomExceptions(404, "Comment", "Comment not found.");

            var history = new CommentHistory
            {
                CommentId = comment.Id,
                PreviousContent = comment.Content,
                UpdatedDate = DateTime.UtcNow
            };
            await _unitOfWork.commentHistoryRepository.Create(history);

            _mapper.Map(updateDto, comment);
            comment.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.commentRepository.Update(comment);
            _unitOfWork.Commit();
        }


        public async Task<List<CommentHistoryDto>> GetCommentHistory(int commentId)
        {
            var history = await _unitOfWork.commentHistoryRepository.GetAll(h => h.CommentId == commentId);
            return _mapper.Map<List<CommentHistoryDto>>(history);
        }

        public async Task LikeOrDislikeComment(CommentLikeDto reactionDto)
        {
            var comment = await _unitOfWork.commentRepository.GetEntity(c => c.Id == reactionDto.CommentId);
            if (comment == null)
                throw new CustomExceptions(404, "Comment", "Comment not found.");

            var existingReaction = await _unitOfWork.commentReactionRepository.GetEntity(r =>
                r.CommentId == reactionDto.CommentId && r.UserId == reactionDto.UserId);

            if (existingReaction != null)
            {
                if (reactionDto.IsLike == existingReaction.IsLike)
                {
                    await _unitOfWork.commentReactionRepository.Delete(existingReaction);
                }
                else
                {
                    existingReaction.IsLike = reactionDto.IsLike;
                    await _unitOfWork.commentReactionRepository.Update(existingReaction);
                }
            }
            else
            {
                var newReaction = new CommentReaction
                {
                    CommentId = reactionDto.CommentId,
                    UserId = reactionDto.UserId,
                    IsLike = reactionDto.IsLike,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                await _unitOfWork.commentReactionRepository.Create(newReaction);
            }

            _unitOfWork.Commit();
        }

        public async Task<List<CommentReactionDto>> GetCommentReactions(int commentId)
        {
            var reactions = await _unitOfWork.commentReactionRepository.GetAll(
                r => r.CommentId == commentId, "User");

            if (reactions == null || !reactions.Any())
            {
                throw new CustomExceptions(404, "Reactions", "No reactions found for comment.");
            }

            return _mapper.Map<List<CommentReactionDto>>(reactions);
        }



    }
}
