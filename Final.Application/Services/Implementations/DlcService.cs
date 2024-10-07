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
            var dlc = await _unitOfWork.dlcRepository.GetEntity(g => g.Id == id);

            if (dlc == null) throw new CustomExceptions(404, "Dlc", "Dlc not found.");

            if (dlcUpdateDto.File != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(dlc.Image))
                {
                    FileExtension.DeleteImage(dlc.Image);
                }

                // Save the new image
                var newFileName = dlcUpdateDto.File.Save(Directory.GetCurrentDirectory(), "uploads/images/");
                dlc.Image = newFileName;
            }

            dlc.UpdatedDate = DateTime.Now;

            // Update other fields using AutoMapper
            _mapper.Map(dlcUpdateDto, dlc);

            if (dlcUpdateDto.Price > 0)
            {
                dlc.Price = (int)dlcUpdateDto.Price;
            }

            await _unitOfWork.dlcRepository.Update(dlc);
            _unitOfWork.Commit();
        }
    }
}
