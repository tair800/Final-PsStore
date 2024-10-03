using Final.Application.Dtos.CommentDtos;

namespace Final.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<int> Create(CommentCreateDto commentCreateDto);
        Task Delete(int id);
        Task<List<CommentReturnDto>> GetAll(int gameId);
        Task<CommentReturnDto> GetOne(int id);
        Task Update(int id, CommentUpdateDto updateDto);
    }
}
