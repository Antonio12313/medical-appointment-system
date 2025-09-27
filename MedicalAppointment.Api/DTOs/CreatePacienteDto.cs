using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.DTOs;

public class CreatePacienteDto
{
    [Required]
    public string Cpf { get; set; } = string.Empty;

    [Required]
    public string Telefone { get; set; } = string.Empty;
}