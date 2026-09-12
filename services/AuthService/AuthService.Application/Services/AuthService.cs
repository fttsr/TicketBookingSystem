using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEventPublisher _eventPublisher;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IEventPublisher eventPublisher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _eventPublisher = eventPublisher;
        }

        public async Task<AuthResponseDto> LoginAsync(RegisterAndLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user is null)
                throw new InvalidOperationException("Неверный email или пароль.");

            var passwordIsValid =
                _passwordHasher.Verify(user, dto.Password, user.PasswordHash);

            if (!passwordIsValid)
                throw new InvalidOperationException("Неверный email или пароль.");

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token
            };

        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterAndLoginDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);

            if (existingUser is not null)
                throw new InvalidOperationException("Пользователь с таким email уже существует.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
            };

            user.PasswordHash = _passwordHasher.Hash(user, dto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            await _eventPublisher.PublishUserRegisteredAsync(user.Id, user.Email);

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token
            };
        }
    }
}
