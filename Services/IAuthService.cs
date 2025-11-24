using Museo.Models.DTOS;

namespace Museo.Services
{
    public interface IAuthService
    {
        Task<(bool ok, LoginResponseDto? response)> LoginAsync(LoginDto dto);
        Task<string> RegisterAsync(RegisterDto dto);
        Task<(bool ok, LoginResponseDto? response)> RefreshAsync(RefreshRequestDto dto);
    }
}