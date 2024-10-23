using AutoMapper;
using Final.Application.Dtos.DlcDtos;
using Final.Application.Exceptions;
using Final.Application.Extensions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class DlcService : IDlcService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DlcService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Create(DlcCreateDto dlcCreateDto)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == dlcCreateDto.GameId);

            if (game == null)
            {
                throw new CustomExceptions(404, "Game", "Game not found.");
            }

            // Handle image upload
            string imagePath = null;
            if (dlcCreateDto.Image != null && dlcCreateDto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{dlcCreateDto.Image.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dlcCreateDto.Image.CopyToAsync(stream);
                }

                imagePath = uniqueFileName;
            }

            var dlc = _mapper.Map<Dlc>(dlcCreateDto);
            dlc.Game = game;
            dlc.Image = imagePath;

            await _unitOfWork.dlcRepository.Create(dlc);
            _unitOfWork.Commit();

            return dlc.Id;
        }


        public async Task Delete(int id)
        {
            var dlc = await _unitOfWork.dlcRepository.GetEntity(d => d.Id == id);
            if (dlc == null)
                throw new CustomExceptions(400, "Name", "Dlc not found.");

            await _unitOfWork.dlcRepository.Delete(dlc);
            _unitOfWork.Commit();
        }

        public async Task<List<DlcReturnDto>> GetAll()
        {
            var dlcs = await _unitOfWork.dlcRepository.GetAll(null, "Game");
            return _mapper.Map<List<DlcReturnDto>>(dlcs);
        }

        public async Task<DlcReturnDto> GetOne(int id)
        {
            var dlc = await _unitOfWork.dlcRepository.GetEntity(d => d.Id == id, "Game");
            if (dlc == null)
            {
                throw new CustomExceptions(404, "DLC", "DLC not found.");
            }

            return _mapper.Map<DlcReturnDto>(dlc);
        }

        public async Task Update(int id, DlcUpdateDto dlcUpdateDto)
        {
            // Retrieve the existing DLC entity
            var dlc = await _unitOfWork.dlcRepository.GetEntity(g => g.Id == id);

            if (dlc == null)
                throw new CustomExceptions(404, "Dlc", "Dlc not found.");

            // Handle new file upload (if provided)
            if (dlcUpdateDto.File != null && dlcUpdateDto.File.Length > 0)
            {
                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(dlc.Image))
                {
                    FileExtension.DeleteImage(dlc.Image); // Utility function to delete the old image
                }

                // Save the new image
                var newFileName = dlcUpdateDto.File.Save(Directory.GetCurrentDirectory(), "uploads/images/");
                dlc.Image = newFileName;
            }
            else
            {
                // If no new file is uploaded, keep the existing image (dlc.Image)
                dlc.Image = dlcUpdateDto.Image ?? dlc.Image;
            }

            // Update the other fields if provided
            if (!string.IsNullOrEmpty(dlcUpdateDto.Name))
            {
                dlc.Name = dlcUpdateDto.Name;
            }

            if (dlcUpdateDto.Price.HasValue)
            {
                dlc.Price = dlcUpdateDto.Price.Value;
            }

            if (dlcUpdateDto.GameId.HasValue)
            {
                var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == dlcUpdateDto.GameId.Value);
                if (game == null)
                {
                    throw new CustomExceptions(404, "Game", "Game not found.");
                }
                dlc.Game = game;
            }

            // Save the changes to the database
            await _unitOfWork.dlcRepository.Update(dlc);
            _unitOfWork.Commit();
        }

    }
}
