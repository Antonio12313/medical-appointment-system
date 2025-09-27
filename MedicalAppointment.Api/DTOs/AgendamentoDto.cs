namespace MedicalAppointment.Api.DTOs;

public class AgendamentoDto
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int HorarioId { get; set; }
    public DateTime DataAgendamento { get; set; }
    public string Status { get; set; } = string.Empty;
    public PacienteDto? Paciente { get; set; }
    public HorarioDisponivelDto? Horario { get; set; }
}