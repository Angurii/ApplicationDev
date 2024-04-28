using ApplicationDev.Modules.User.DTOs;
using ApplicationDev.Modules.User.Entity;
using ApplicationDev.Modules.User.Repos;

namespace ApplicationDev.Modules.User.Services
{
	public class UserService
	{
		private readonly UserRepository _userRepo;

		//private readonly IDataBaseBaseInterface<UserEntity> _userRepo;
		public UserService(UserRepository userRepo)
		{
			_userRepo = userRepo;
		}

		public async Task<UserEntity> CreateUser(UserCreateDTO data)
		{
			UserEntity userDataToSend = new UserEntity { Name = data.Name, Password = data.Password, UserName = data.UserName };
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
