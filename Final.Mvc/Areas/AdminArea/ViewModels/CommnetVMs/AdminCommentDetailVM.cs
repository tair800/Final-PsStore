using Final.Application.Dtos.CommentDtos;

namespace Final.Mvc.Areas.AdminArea.ViewModels.CommnetVMs
{
    public class AdminCommentDetailVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<CommentHistoryDto> History { get; set; } // Include comment history
        public List<CommentReactionDto>? Reactions { get; set; }

    }
}
