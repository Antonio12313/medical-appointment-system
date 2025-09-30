using System.ComponentModel.DataAnnotations;
using MedicalAppointment.Api.Enums;

namespace MedicalAppointment.Api.Models;

public class Agendamento
{
    public int Id { get; set; }

    [Required] public int PacienteId { get; set; }

    [Required] public int HorarioId { get; set; }

    public DateTime DataAgendamento { get; set; }

    public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;

    public Paciente Paciente { get; set; } = null!;
    public HorarioDisponivel Horario { get; set; } = null!;
}