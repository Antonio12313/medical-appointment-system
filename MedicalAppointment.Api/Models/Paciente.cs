using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.Models;

public class Paciente
{
    public int Id { get; set; }
    
    public int UsuarioId { get; set; }
    
    [Required]
    [MaxLength(14)]
    public string Cpf { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(15)]
    public string Telefone { get; set; } = string.Empty;

    public Usuario Usuario { get; set; } = null!;
    public List<Agendamento> Agendamentos { get; set; } = new();
}