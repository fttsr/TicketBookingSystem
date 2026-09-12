using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthService.Application.DTOs
{
    public class RegisterAndLoginDto
    {
        [Required(ErrorMessage = "Email обязателен для заполнения.")]
        [EmailAddress(ErrorMessage = "Неверный формат email.")]
        [MaxLength(256, ErrorMessage = "Email слишком длинный (не более 256 символов).")]
        public string Email { get; set; } = String.Empty;

        [Required(ErrorMessage = "Пароль обязателен для заполнения.")]
        [MinLength(8, ErrorMessage = "Пароль должен содержать минимум 8 символов.")]
        [MaxLength(128, ErrorMessage = "Пароль слишком длинный (не более 128 символов).")]
        public string Password { get; set; } = String.Empty;
    }
}
