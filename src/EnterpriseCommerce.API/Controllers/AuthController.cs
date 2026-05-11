using EnterpriseCommerce.Application.Events;
using EnterpriseCommerce.Application.Features.Auth.DTOs;
using EnterpriseCommerce.Application.Interfaces;
using EnterpriseCommerce.Domain.Entities;
using EnterpriseCommerce.Domain.Interfaces;
using EnterpriseCommerce.Infrastructure.Repositories;
using EnterpriseCommerce.Infrastructure.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthController(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository , IMessageBroker messageBroker)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
            _messageBroker = messageBroker;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            var existingUser =
                await _userRepository.GetByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Customer"
            };

            await _userRepository.CreateAsync(user);
            var userRegisteredEvent =
    new UserRegisteredEvent
    {
        UserId = user.Id,
        Email = user.Email,
        FullName = user.FullName
    };

            _messageBroker.Publish(userRegisteredEvent);
            BackgroundJob.Enqueue<EmailJobService>(
    x => x.SendWelcomeEmail(user.Email));

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            var user =
                await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var validPassword =
                BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!validPassword)
            {
                return Unauthorized("Invalid credentials");
            }

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = RefreshTokenGenerator.Generate(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            var accessToken =
    _jwtTokenGenerator.GenerateToken(user);
            await _refreshTokenRepository
                .CreateAsync(refreshToken);

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Email = user.Email,
                Role = user.Role
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
    [FromBody] string refreshToken)
        {
            var existingToken =
                await _refreshTokenRepository
                    .GetByTokenAsync(refreshToken);

            if (existingToken == null)
            {
                return Unauthorized();
            }

            if (existingToken.IsRevoked)
            {
                return Unauthorized();
            }

            if (existingToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var user =
                await _userRepository
                    .GetByIdAsync(existingToken.UserId);

            if (user == null)
            {
                return Unauthorized();
            }

            var newAccessToken =
                _jwtTokenGenerator.GenerateToken(user);

            return Ok(new
            {
                AccessToken = newAccessToken
            });
        }
    }

}
