using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.DTOs;

public class CreateUsuarioDto
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    public string TipoUsuario { get; set; } = string.Empty;

    public CreateMedicoDto? DadosMedico { get; set; }
    public CreatePacienteDto? DadosPaciente { get; set; }
}