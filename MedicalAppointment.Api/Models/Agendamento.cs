using System.ComponentModel.DataAnnotations;
using MedicalAppointment.Api.Enums;

namespace MedicalAppointment.Api.Models;

public class Agendamento
{
    public int Id { get; set; }
    
    public int PacienteId { get; set; }
    
    public int HorarioId { get; set; }
    
    public DateTime DataAgendamento { get; set; } = DateTime.UtcNow;
    
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;
    public Paciente Paciente { get; set; } = null!;
    public HorarioDisponivel Horario { get; set; } = null!;

}