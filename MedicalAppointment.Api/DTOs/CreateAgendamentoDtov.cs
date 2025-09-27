using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.DTOs;

public class CreateAgendamentoDtov
{
    [Required]
    public int HorarioId { get; set; }
}