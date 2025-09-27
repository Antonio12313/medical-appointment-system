using MedicalAppointment.Api.DTOs;
using MedicalAppointment.Api.Models;

namespace MedicalAppointment.Api.Services;

public interface IUsuarioService
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario> CreateAsync(CreateUsuarioDto dto, string hashedPassword);
    Task<Usuario?> GetByIdAsync(int id);
}