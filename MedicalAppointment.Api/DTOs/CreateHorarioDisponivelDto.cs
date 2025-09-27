using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.DTOs;

public class CreateHorarioDisponivelDto
{
    [Required]
    public DateTime DataHoraInicio { get; set; }

    [Required]
    public DateTime DataHoraFim { get; set; }
}