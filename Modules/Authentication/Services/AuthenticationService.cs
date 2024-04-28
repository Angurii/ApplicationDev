using System.Net;
using System.Runtime.Serialization;
using ApplicationDev.Common.Database.BaseEntity;
using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Authentication.DTOs;
using Microsoft.AspNetCore.Server.HttpSys;
namespace ApplicationDev.Modules.Authentication.Services
{
	public class AuthenticationService
	{
		private readonly ILogger<AuthenticationService> _logger;
		public AuthenticationService(ILogger<AuthenticationService> logger)
		{
			_logger = logger;
		}
		public string Login(BaseUserEntity entity, UserLoginDTO incomingData)
		{
			_logger.LogInformation("Incoming Data in AuthService: {Data}", incomingData);
			bool isPasswordCorrect = checkPassword(entity, incomingData.Password);
			if (!isPasswordCorrect)
			{
				throw new HttpException(HttpStatusCode.Unauthorized, "Invalid Credentials");

			}
			return "akjdhaskdhsakdh";
		}

		public bool checkPassword(BaseUserEntity entity, string incomingPassword)
		{
			if (entity.Password == incomingPassword)
			{
				return true;
			}
			return false;

		}
	}

}
