using bd2_first_api_202603.DTOs;

namespace bd2_first_api_202603.Services
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task AsignarRolAsync(AsignarRolDto dto);
    }
}
