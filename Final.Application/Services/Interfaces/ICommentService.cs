using Final.Application.Dtos.CommentDtos;

namespace Final.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<int> Create(CommentCreateDto commentCreateDto);
        Task Delete(int id);
        Task<List<CommentReturnDto>> GetAll(int? gameId = null);
        Task<CommentReturnDto> GetOne(int id);
        Task<List<CommentHistoryDto>> GetCommentHistory(int commentId);
        Task Update(int id, CommentUpdateDto updateDto);
    }
}
