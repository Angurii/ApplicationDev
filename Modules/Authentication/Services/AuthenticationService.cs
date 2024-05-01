using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using ApplicationDev.Common.Database.BaseEntity;
using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Authentication.DTOs;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationDev.Modules.Authentication.Services
{
	public class AuthenticationService
	{
		private const string SecretKey = "your_secret_key_here_ahdkahdkjashksaaskh_Nishan_Raut_hdkjashdkjsakdhsa_Raut_Nishan";
		private readonly ILogger<AuthenticationService> _logger;
		public AuthenticationService(ILogger<AuthenticationService> logger)
		{
			_logger = logger;

		}

		public string Login(BaseUserEntity entity, UserLoginDTO incomingData, string role)
		{

			try
			{
				bool isPasswordCorrect = checkPassword(entity, incomingData.Password);
				if (!isPasswordCorrect)
				{
					throw new HttpException(HttpStatusCode.Unauthorized, "Invalid Credentials");
				}

				// Define const Key this should be private secret key  stored in some safe place
				return GenerateToken(entity.id.ToString(), role);
			}
			catch (Exception)
			{
				_logger.LogError("Error in AuthService.Login");
				throw;
			}
		}

		public bool checkPassword(BaseUserEntity entity, string incomingPassword)
		{
			try
			{
				return BCrypt.Net.BCrypt.Verify(incomingPassword, entity.Password);
			}
			catch (Exception ex)
			{

				_logger.LogError("Error in AuthService.checkPassword" + ex);
				throw;
			}
		}
		private string GenerateToken(string userId, string role)
		{
			_logger.LogInformation("This is UserId: " + userId);

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
				new Claim(ClaimTypes.NameIdentifier, userId),
				new Claim(ClaimTypes.Role, role)
			}),
				Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}


}