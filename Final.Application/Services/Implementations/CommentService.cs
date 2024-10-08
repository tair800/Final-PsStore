﻿using AutoMapper;
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
                // Log the detailed error, including inner exception
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

        // Get all comments, optionally filtered by gameId
        public async Task<List<CommentReturnDto>> GetAll(int? gameId = null)
        {
            IEnumerable<Comment> comments;

            if (gameId.HasValue)
            {
                comments = await _unitOfWork.commentRepository.GetAll(
                    c => c.GameId == gameId,
                    "Game");
            }
            else
            {
                comments = await _unitOfWork.commentRepository.GetAll(
                    null, "Game");
            }

            return _mapper.Map<List<CommentReturnDto>>(comments);
        }


        public async Task<CommentReturnDto> GetOne(int id)
        {
            var comment = await _unitOfWork.commentRepository.GetEntity(
                c => c.Id == id,
                "Game");

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