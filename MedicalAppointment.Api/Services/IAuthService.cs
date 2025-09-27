using MedicalAppointment.Api.Models;

namespace MedicalAppointment.Api.Services;

public interface IAuthService
{
    string GenerateJwtToken(Usuario usuario);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}