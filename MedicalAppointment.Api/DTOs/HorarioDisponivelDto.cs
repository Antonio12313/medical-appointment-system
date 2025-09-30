namespace MedicalAppointment.Api.DTOs;

public class HorarioDisponivelDto
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public DateTime DataHoraInicio { get; set; }
    public DateTime DataHoraFim { get; set; }
    public bool Disponivel { get; set; }
    public MedicoDto? Medico { get; set; }
}