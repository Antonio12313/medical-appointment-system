using System.ComponentModel.DataAnnotations;

namespace MedicalAppointment.Api.Models;

public class Medico
{
    public int Id { get; set; }
        
    [Required]
    public int UsuarioId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Crm { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Especialidade { get; set; } = string.Empty;

    public Usuario Usuario { get; set; } = null!;
    public ICollection<HorarioDisponivel> HorariosDisponiveis { get; set; } = new List<HorarioDisponivel>();
}