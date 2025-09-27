using MedicalAppointment.Api.Data;
using MedicalAppointment.Api.DTOs;
using MedicalAppointment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointment.Api.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .Include(u => u.Medico)
            .Include(u => u.Paciente)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario> CreateAsync(CreateUsuarioDto dto, string hashedPassword)
    {
        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = hashedPassword,
            TipoUsuario = dto.TipoUsuario,
            DataCriacao = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Criar perfil específico baseado no tipo
        if (dto.TipoUsuario == "medico" && dto.DadosMedico != null)
        {
            var medico = new Medico
            {
                UsuarioId = usuario.Id,
                Crm = dto.DadosMedico.Crm,
                Especialidade = dto.DadosMedico.Especialidade
            };
            _context.Medicos.Add(medico);
        }
        else if (dto.TipoUsuario == "paciente" && dto.DadosPaciente != null)
        {
            var paciente = new Paciente
            {
                UsuarioId = usuario.Id,
                Cpf = dto.DadosPaciente.Cpf,
                Telefone = dto.DadosPaciente.Telefone
            };
            _context.Pacientes.Add(paciente);
        }

        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Medico)
            .Include(u => u.Paciente)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}