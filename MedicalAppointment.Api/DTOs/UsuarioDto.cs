namespace MedicalAppointment.Api.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TipoUsuario { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public MedicoDto? Medico { get; set; }
    public PacienteDto? Paciente { get; set; }
}