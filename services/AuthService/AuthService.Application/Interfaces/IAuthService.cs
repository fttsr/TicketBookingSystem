using AuthService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterAndLoginDto dto);
        Task<AuthResponseDto> LoginAsync(RegisterAndLoginDto dto);
    }
}
