﻿using ApplicationDev.Common.Exceptions;
using System.Net;
using ApplicationDev.Modules.User.DTOs;
using ApplicationDev.Modules.User.Entity;
using ApplicationDev.Modules.User.Repos;

namespace ApplicationDev.Modules.User.Services
{
	public class UserService
	{
		private readonly UserRepository _userRepo;

		private readonly ILogger<UserService> _logger;
		public UserService(UserRepository userRepo, ILogger<UserService> logger)
		{
			_userRepo = userRepo;
			_logger = logger;
		}


		public async Task<UserEntity> CreateUser(UserCreateDTO data)
		{
			//Check if that user is already registered or not
			UserEntity? existingUser = await _userRepo.FindOne(x => x.UserName == data.UserName);

			//If found throw conflict error
			if (existingUser != null)
			{
				throw new HttpException(HttpStatusCode.Conflict, "User Already Exists");
			}


			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);
			_logger.LogInformation("Hashed Password: " + hashedPassword);

			UserEntity userDataToSend = new UserEntity { Name = data.Name, Password = hashedPassword, UserName = data.UserName };

			UserEntity createdUser = await _userRepo.CreateAsync(userDataToSend, true);

			return createdUser;
		}
		public async Task<UserEntity?> GetUserByIdAsync(int id)
		{
			return await _userRepo.FindByIdAsync(id);
		}

		async public Task<UserEntity?> FindOne(string userName)
		{
			return await _userRepo.FindOne(x => x.UserName == userName);
		}
	}
}