using Microsoft.AspNetCore.Http;

namespace Final.Application.Extensions
{
    public static class FileExtension
    {
        public static string Save(this IFormFile file, string root, string folder)
        {
            string newFileName = Guid.NewGuid().ToString() + file.FileName;
            string path = Path.Combine(root, "wwwroot", folder, newFileName);
            using FileStream fileStream = new(path, FileMode.Create);
            file.CopyTo(fileStream);
            return newFileName;
        }

        public static void DeleteImage(string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads/images", fileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}