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

            //if (await _unitOfWork.dlcRepository.isExists(d => d.Name.ToLower() == dlcCreateDto.Name.ToLower()))
            //    throw new CustomExceptions(400, "Name", "Dublicate not permitted");

            var dlc = _mapper.Map<Dlc>(dlcCreateDto);
            dlc.Game = game;

            await _unitOfWork.dlcRepository.Create(dlc);
            _unitOfWork.Commit();

            return dlc.Id;
        }

        public async Task Delete(string name)
        {
            var dlc = await _unitOfWork.dlcRepository.GetEntity(d => d.Name.ToLower() == name.ToLower());
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


        public async Task<DlcReturnDto> GetOne(string name)
        {
            var dlc = await _unitOfWork.dlcRepository.GetEntity(d => d.Name == name, "Game");
            if (dlc == null)
            {
                throw new CustomExceptions(404, "DLC", "DLC not found.");
            }

            return _mapper.Map<DlcReturnDto>(dlc);
        }


    }
}
