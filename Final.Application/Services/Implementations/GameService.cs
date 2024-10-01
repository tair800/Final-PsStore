﻿using AutoMapper;
using Final.Application.Dtos.GameDtos;
using Final.Application.Exceptions;
using Final.Application.Extensions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Create a new game
        public async Task<int> Create(GameCreateDto createDto)
        {
            // Check if a game with the same title already exists
            if (await _unitOfWork.gameRepository.isExists(g => g.Title.ToLower() == createDto.Title.ToLower()))
                throw new CustomExceptions(400, "Name", "Duplicate is not permitted");

            var game = _mapper.Map<Game>(createDto);
            await _unitOfWork.gameRepository.Create(game);
            _unitOfWork.Commit();

            return game.Id;
        }

        // Get all games
        public async Task<List<GameReturnDto>> GetAll()
        {
            var games = await _unitOfWork.gameRepository.GetAll(null, "Dlcs", "Category");
            return _mapper.Map<List<GameReturnDto>>(games);
        }

        // Get a single game by ID
        public async Task<GameReturnDto> GetOne(int id)
        {
            // Fetch game with related DLCs and Category
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == id, "Dlcs", "Category");

            if (game == null)
            {
                throw new CustomExceptions(404, "Game", "Game not found.");
            }

            // Use AutoMapper to map from Game to GameReturnDto
            var gameReturnDto = _mapper.Map<GameReturnDto>(game);

            return gameReturnDto;
        }

        // Delete a game by ID
        public async Task Delete(int id)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == id);
            if (game is null)
                throw new CustomExceptions(404, "Game", "Game not found.");

            await _unitOfWork.gameRepository.Delete(game);
            _unitOfWork.Commit();
        }

        // Update a game by ID
        public async Task Update(int id, GameUpdateDto updateDto)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == id);
            if (game == null)
                throw new CustomExceptions(404, "Game", "Game not found.");

            if (updateDto.File != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(game.ImgUrl))
                {
                    FileExtension.DeleteImage(game.ImgUrl);
                }

                // Save the new image
                var newFileName = updateDto.File.Save(Directory.GetCurrentDirectory(), "uploads/images/");
                game.ImgUrl = newFileName;
            }

            game.UpdatedDate = DateTime.Now;

            // Update other fields using AutoMapper
            _mapper.Map(updateDto, game);

            await _unitOfWork.gameRepository.Update(game);
            _unitOfWork.Commit();
        }

        // Search games by title
        public async Task<List<GameReturnDto>> SearchGames(string title)
        {
            var games = await _unitOfWork.gameRepository.Search(g => g.Title.Contains(title));
            return _mapper.Map<List<GameReturnDto>>(games);
        }
    }
}
