using MedicalAppointment.Api.Data;
using MedicalAppointment.Api.DTOs;
using MedicalAppointment.Api.Enums;
using MedicalAppointment.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AgendamentosController : ControllerBase
{
    private readonly AppDbContext _context;

    public AgendamentosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AgendamentoDto>>> GetAgendamentos()
    {
        try
        {
            var userType = User.FindFirst("tipo")?.Value;
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            IQueryable<Agendamento> query = _context.Agendamentos
                .Include(a => a.Paciente)
                .ThenInclude(p => p.Usuario)
                .Include(a => a.Horario)
                .ThenInclude(h => h.Medico)
                .ThenInclude(m => m.Usuario);

            if (userType == "paciente")
            {
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);
                if (paciente == null)
                {
                    return NotFound(new { message = "Paciente não encontrado" });
                }

                query = query.Where(a => a.PacienteId == paciente.Id);
            }
            else if (userType == "medico")
            {
                var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.UsuarioId == userId);
                if (medico == null)
                {
                    return NotFound(new { message = "Médico não encontrado" });
                }

                query = query.Where(a => a.Horario.MedicoId == medico.Id);
            }

            var agendamentos = await query
                .OrderByDescending(a => a.DataAgendamento)
                .ToListAsync();

            var agendamentosDto = agendamentos.Select(a => new AgendamentoDto
            {
                Id = a.Id,
                PacienteId = a.PacienteId,
                HorarioId = a.HorarioId,
                DataAgendamento = a.DataAgendamento,
                Status = a.Status.ToString(),
                Paciente = new PacienteDto
                {
                    Id = a.Paciente.Id,
                    Cpf = a.Paciente.Cpf,
                    Telefone = a.Paciente.Telefone
                },
                Horario = new HorarioDisponivelDto
                {
                    Id = a.Horario.Id,
                    MedicoId = a.Horario.MedicoId,
                    DataHoraInicio = a.Horario.DataHoraInicio,
                    DataHoraFim = a.Horario.DataHoraFim,
                    Disponivel = a.Horario.Disponivel,
                    Medico = new MedicoDto
                    {
                        Id = a.Horario.Medico.Id,
                        Crm = a.Horario.Medico.Crm,
                        Especialidade = a.Horario.Medico.Especialidade
                    }
                }
            }).ToList();

            return Ok(agendamentosDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao buscar agendamentos", details = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AgendamentoDto>> CreateAgendamento(CreateAgendamentoDto createDto)
    {
        try
        {
            var userType = User.FindFirst("tipo")?.Value;
            if (userType != "paciente")
            {
                return Forbid("Apenas pacientes podem criar agendamentos");
            }

            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null)
            {
                return NotFound(new { message = "Paciente não encontrado" });
            }

            // Verificar se o horário existe e está disponível
            var horario = await _context.HorariosDisponiveis
                .Include(h => h.Medico)
                .ThenInclude(m => m.Usuario)
                .FirstOrDefaultAsync(h => h.Id == createDto.HorarioId);

            if (horario == null)
            {
                return NotFound(new { message = "Horário não encontrado" });
            }

            if (!horario.Disponivel)
            {
                return BadRequest(new { message = "Horário não está disponível" });
            }

            // Verificar se já existe agendamento para este horário
            var agendamentoExistente = await _context.Agendamentos
                .FirstOrDefaultAsync(a =>
                    a.HorarioId == createDto.HorarioId && a.Status != StatusAgendamento.Cancelado);

            if (agendamentoExistente != null)
            {
                return BadRequest(new { message = "Este horário já foi agendado" });
            }

            // Criar o agendamento
            var agendamento = new Agendamento
            {
                PacienteId = paciente.Id,
                HorarioId = createDto.HorarioId,
                DataAgendamento = DateTime.UtcNow,
                Status = StatusAgendamento.Agendado
            };

            _context.Agendamentos.Add(agendamento);

            // Marcar horário como não disponível
            horario.Disponivel = false;

            await _context.SaveChangesAsync();

            // Recarregar o agendamento com as relações
            agendamento = await _context.Agendamentos
                .Include(a => a.Paciente)
                .ThenInclude(p => p.Usuario)
                .Include(a => a.Horario)
                .ThenInclude(h => h.Medico)
                .ThenInclude(m => m.Usuario)
                .FirstOrDefaultAsync(a => a.Id == agendamento.Id);

            var agendamentoDto = new AgendamentoDto
            {
                Id = agendamento!.Id,
                PacienteId = agendamento.PacienteId,
                HorarioId = agendamento.HorarioId,
                DataAgendamento = agendamento.DataAgendamento,
                Status = agendamento.Status.ToString(),
                Paciente = new PacienteDto
                {
                    Id = agendamento.Paciente.Id,
                    Cpf = agendamento.Paciente.Cpf,
                    Telefone = agendamento.Paciente.Telefone
                },
                Horario = new HorarioDisponivelDto
                {
                    Id = agendamento.Horario.Id,
                    MedicoId = agendamento.Horario.MedicoId,
                    DataHoraInicio = agendamento.Horario.DataHoraInicio,
                    DataHoraFim = agendamento.Horario.DataHoraFim,
                    Disponivel = agendamento.Horario.Disponivel,
                    Medico = new MedicoDto
                    {
                        Id = agendamento.Horario.Medico.Id,
                        Crm = agendamento.Horario.Medico.Crm,
                        Especialidade = agendamento.Horario.Medico.Especialidade
                    }
                }
            };

            return CreatedAtAction(nameof(GetAgendamento), new { id = agendamento.Id }, agendamentoDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao criar agendamento", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AgendamentoDto>> GetAgendamento(int id)
    {
        try
        {
            var agendamento = await _context.Agendamentos
                .Include(a => a.Paciente)
                .ThenInclude(p => p.Usuario)
                .Include(a => a.Horario)
                .ThenInclude(h => h.Medico)
                .ThenInclude(m => m.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
            {
                return NotFound(new { message = "Agendamento não encontrado" });
            }

            // Verificar se o usuário tem permissão para ver este agendamento
            var userType = User.FindFirst("tipo")?.Value;
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            bool hasPermission = false;
            if (userType == "paciente")
            {
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);
                hasPermission = paciente?.Id == agendamento.PacienteId;
            }
            else if (userType == "medico")
            {
                var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.UsuarioId == userId);
                hasPermission = medico?.Id == agendamento.Horario.MedicoId;
            }

            if (!hasPermission)
            {
                return Forbid("Você não tem permissão para acessar este agendamento");
            }

            var agendamentoDto = new AgendamentoDto
            {
                Id = agendamento.Id,
                PacienteId = agendamento.PacienteId,
                HorarioId = agendamento.HorarioId,
                DataAgendamento = agendamento.DataAgendamento,
                Status = agendamento.Status.ToString(),
                Paciente = new PacienteDto
                {
                    Id = agendamento.Paciente.Id,
                    Cpf = agendamento.Paciente.Cpf,
                    Telefone = agendamento.Paciente.Telefone
                },
                Horario = new HorarioDisponivelDto
                {
                    Id = agendamento.Horario.Id,
                    MedicoId = agendamento.Horario.MedicoId,
                    DataHoraInicio = agendamento.Horario.DataHoraInicio,
                    DataHoraFim = agendamento.Horario.DataHoraFim,
                    Disponivel = agendamento.Horario.Disponivel,
                    Medico = new MedicoDto
                    {
                        Id = agendamento.Horario.Medico.Id,
                        Crm = agendamento.Horario.Medico.Crm,
                        Especialidade = agendamento.Horario.Medico.Especialidade
                    }
                }
            };

            return Ok(agendamentoDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao buscar agendamento", details = ex.Message });
        }
    }

    [HttpPut("{id}/cancelar")]
    [Authorize]
    public async Task<IActionResult> CancelarAgendamento(int id)
    {
        try
        {
            var agendamento = await _context.Agendamentos
                .Include(a => a.Horario)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
            {
                return NotFound(new { message = "Agendamento não encontrado" });
            }

            // Verificar se o usuário tem permissão para cancelar este agendamento
            var userType = User.FindFirst("tipo")?.Value;
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            bool hasPermission = false;
            if (userType == "paciente")
            {
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);
                hasPermission = paciente?.Id == agendamento.PacienteId;
            }
            else if (userType == "medico")
            {
                var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.UsuarioId == userId);
                hasPermission = medico?.Id == agendamento.Horario.MedicoId;
            }

            if (!hasPermission)
            {
                return Forbid("Você não tem permissão para cancelar este agendamento");
            }

            if (agendamento.Status == StatusAgendamento.Cancelado)
            {
                return BadRequest(new { message = "Agendamento já está cancelado" });
            }

            agendamento.Status = StatusAgendamento.Cancelado;

            agendamento.Horario.Disponivel = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Agendamento cancelado com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao cancelar agendamento", details = ex.Message });
        }
    }
}