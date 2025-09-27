using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.Models;

public class HorarioDisponivel
{
    public int Id { get; set; }
    
    public int MedicoId { get; set; }
    
    [Required]
    public DateTime DataHoraInicio { get; set; }
    
    [Required]
    public DateTime DataHoraFim { get; set; }
    
    public bool Disponivel { get; set; } = true;
    
    public Medico Medico { get; set; } = null!;
    public Agendamento? Agendamento { get; set; }
}