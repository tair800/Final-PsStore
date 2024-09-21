using AutoMapper;
using Final.Application.Dtos.UserDtos;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserReturnDto> GetUserByEmail(string email)
        {
            var user = await _unitOfWork.userRepository.GetEntity(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return _mapper.Map<UserReturnDto>(user);
        }

        public async Task<UserReturnDto> CreateUser(RegisterDto registerDto)
        {
            var userEntity = _mapper.Map<User>(registerDto);

            await _unitOfWork.userRepository.Create(userEntity);
            _unitOfWork.Commit();

            return _mapper.Map<UserReturnDto>(userEntity);
        }

        public async Task<bool> DeleteUser(string email)
        {
            var user = await _unitOfWork.userRepository.GetEntity(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            await _unitOfWork.userRepository.Delete(user);
            _unitOfWork.Commit();
            return true;
        }

        public async Task<UserReturnDto> UpdateUser(string email, UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.userRepository.GetEntity(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            _mapper.Map(updateUserDto, user);
            await _unitOfWork.userRepository.Update(user);
            _unitOfWork.Commit();

            return _mapper.Map<UserReturnDto>(user);
        }

        public async Task<List<UserReturnDto>> GetAllUsers()
        {
            var users = await _unitOfWork.userRepository.GetAll();
            return _mapper.Map<List<UserReturnDto>>(users);
        }
    }
}
