using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.DTOs;

public class CreateAgendamentoDto
{
    [Required]
    public int HorarioId { get; set; }
}