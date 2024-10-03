using AutoMapper;
using Final.Application.Dtos.CommentDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

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

            await _unitOfWork.commentRepository.Create(comment);

            _unitOfWork.Commit();

            return comment.Id;
        }

        public async Task Delete(int id)
        {
            var comment = await _unitOfWork.commentRepository.GetEntity(c => c.Id == id);

            if (comment == null)
                throw new CustomExceptions(404, "Comment", "Comment not found.");

            await _unitOfWork.commentRepository.Delete(comment);

            _unitOfWork.Commit();
        }

        public async Task<List<CommentReturnDto>> GetAll(int gameId)
        {
            var comments = await _unitOfWork.commentRepository.GetAll(c => c.GameId == gameId, "User");

            return _mapper.Map<List<CommentReturnDto>>(comments);
        }

        public async Task<CommentReturnDto> GetOne(int id)
        {
            var comment = await _unitOfWork.commentRepository.GetEntity(c => c.Id == id, "User", "Game");

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

            _mapper.Map(updateDto, comment);

            comment.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.commentRepository.Update(comment);
            _unitOfWork.Commit();
        }
    }
}
