using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.DTOs;

public class CreateMedicoDto
{
    [Required]
    public string Crm { get; set; } = string.Empty;

    [Required]
    public string Especialidade { get; set; } = string.Empty;
}