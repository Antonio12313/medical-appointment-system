using MedicalAppointment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointment.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Medico> Medicos { get; set; }
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<HorarioDisponivel> HorariosDisponiveis { get; set; }
    public DbSet<Agendamento> Agendamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Medico>()
            .HasOne(m => m.Usuario)
            .WithOne(u => u.Medico)
            .HasForeignKey<Medico>(m => m.UsuarioId);

        modelBuilder.Entity<Paciente>()
            .HasOne(p => p.Usuario)
            .WithOne(u => u.Paciente)
            .HasForeignKey<Paciente>(p => p.UsuarioId);

        modelBuilder.Entity<HorarioDisponivel>()
            .HasOne(h => h.Medico)
            .WithMany(m => m.HorariosDisponiveis)
            .HasForeignKey(h => h.MedicoId);

        modelBuilder.Entity<Agendamento>()
            .HasOne(a => a.Paciente)
            .WithMany(p => p.Agendamentos)
            .HasForeignKey(a => a.PacienteId);

        modelBuilder.Entity<Agendamento>()
            .HasOne(a => a.Horario)
            .WithOne(h => h.Agendamento)
            .HasForeignKey<Agendamento>(a => a.HorarioId);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Medico>()
            .HasIndex(m => m.Crm)
            .IsUnique();

        modelBuilder.Entity<Paciente>()
            .HasIndex(p => p.Cpf)
            .IsUnique();
    }
}