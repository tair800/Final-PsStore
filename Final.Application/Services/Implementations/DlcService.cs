using AutoMapper;
using Final.Application.Dtos.DlcDtos;
using Final.Application.Exceptions;
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

            var dlc = _mapper.Map<Dlc>(dlcCreateDto);
            dlc.Game = game;

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
            var dlc = await _unitOfWork.dlcRepository.GetEntity(d => d.Id == id);

            if (dlc is null)
                throw new CustomExceptions(404, "Dlc", "Dlc not found.");

            var gameExists = await _unitOfWork.gameRepository.isExists(g => g.Id == dlcUpdateDto.GameId);
            if (!gameExists)
                throw new CustomExceptions(404, "Game", "The provided GameId does not exist.");

            dlc.UpdatedDate = DateTime.Now;

            _mapper.Map(dlcUpdateDto, dlc);

            if (!string.IsNullOrEmpty(dlcUpdateDto.Image))
            {
                dlc.Image = dlcUpdateDto.Image;
            }

            await _unitOfWork.dlcRepository.Update(dlc);
            _unitOfWork.Commit();
        }
    }
}
