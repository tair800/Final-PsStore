using AutoMapper;
using Final.Application.Dtos.PromoDtos;
using Final.Application.Exceptions;
using Final.Application.Extensions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class PromoService : IPromoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PromoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Create(PromoCreateDto createDto)
        {
            if (await _unitOfWork.promoRepository.isExists(p => p.Name.ToLower() == createDto.Name.ToLower()))
                throw new CustomExceptions(400, "Name", "Duplicate is not permitted");

            var promo = _mapper.Map<Promo>(createDto);
            if (createDto.File != null)
            {
                var newFileName = createDto.File.Save(Directory.GetCurrentDirectory(), "uploads/images/");
                promo.Image = newFileName;
            }

            await _unitOfWork.promoRepository.Create(promo);
            _unitOfWork.Commit();

            return promo.Id;
        }

        public async Task<List<PromoReturnDto>> GetAll()
        {
            var promos = await _unitOfWork.promoRepository.GetAll();
            return _mapper.Map<List<PromoReturnDto>>(promos);
        }

        public async Task<PromoReturnDto> GetOne(int id)
        {
            var promo = await _unitOfWork.promoRepository.GetEntity(p => p.Id == id);

            if (promo == null)
            {
                throw new CustomExceptions(404, "Promo", "Promo not found.");
            }

            return _mapper.Map<PromoReturnDto>(promo);
        }

        public async Task Delete(int id)
        {
            var promo = await _unitOfWork.promoRepository.GetEntity(p => p.Id == id);
            if (promo == null)
                throw new CustomExceptions(404, "Promo", "Promo not found.");

            if (!string.IsNullOrEmpty(promo.Image))
            {
                FileExtension.DeleteImage(promo.Image);
            }

            await _unitOfWork.promoRepository.Delete(promo);
            _unitOfWork.Commit();
        }

        public async Task Update(int id, PromoUpdateDto updateDto)
        {
            var promo = await _unitOfWork.promoRepository.GetEntity(p => p.Id == id);
            if (promo == null)
                throw new CustomExceptions(404, "Promo", "Promo not found.");

            if (updateDto.File != null)
            {
                if (!string.IsNullOrEmpty(promo.Image))
                {
                    FileExtension.DeleteImage(promo.Image);
                }

                var newFileName = updateDto.File.Save(Directory.GetCurrentDirectory(), "uploads/images/");
                promo.Image = newFileName;
            }

            promo.UpdatedDate = DateTime.UtcNow;

            _mapper.Map(updateDto, promo);

            await _unitOfWork.promoRepository.Update(promo);
            _unitOfWork.Commit();
        }
    }
}
