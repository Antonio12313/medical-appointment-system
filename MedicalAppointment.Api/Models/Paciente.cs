using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.Models;

public class Paciente
{
    public int Id { get; set; }
    
    [Required]
    public int UsuarioId { get; set; }
    
    [Required]
    [MaxLength(14)]
    public string Cpf { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(15)]
    public string Telefone { get; set; } = string.Empty;

    public Usuario Usuario { get; set; } = null!;
    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
}