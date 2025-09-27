using MedicalAppointment.Api.Data;
using MedicalAppointment.Api.DTOs;
using MedicalAppointment.Api.Models;
using MedicalAppointment.Api.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HorariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HorariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HorarioDisponivelDto>>> GetHorarios([FromQuery] int? medicoId = null)
        {
            try
            {
                var query = _context.HorariosDisponiveis
                    .Include(h => h.Medico)
                    .ThenInclude(m => m.Usuario)
                    .Where(h => h.Disponivel)
                    .AsQueryable();

                if (medicoId.HasValue)
                {
                    query = query.Where(h => h.MedicoId == medicoId.Value);
                }

                var horarios = await query
                    .OrderBy(h => h.DataHoraInicio)
                    .ToListAsync();

                var horariosDto = horarios.Select(h => new HorarioDisponivelDto
                {
                    Id = h.Id,
                    MedicoId = h.MedicoId,
                    DataHoraInicio = h.DataHoraInicio,
                    DataHoraFim = h.DataHoraFim,
                    Disponivel = h.Disponivel,
                    Medico = new MedicoDto
                    {
                        Id = h.Medico.Id,
                        Crm = h.Medico.Crm,
                        Especialidade = h.Medico.Especialidade
                    }
                }).ToList();

                return Ok(horariosDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar horários", details = ex.Message });
            }
        }

        [HttpGet("meus-horarios")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<HorarioDisponivelDto>>> GetMeusHorarios()
        {
            try
            {
                var userType = User.FindFirst("tipo")?.Value;
                if (userType != "medico")
                {
                    return Forbid("Apenas médicos podem acessar esta funcionalidade");
                }

                var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.UsuarioId == userId);

                if (medico == null)
                {
                    return NotFound(new { message = "Médico não encontrado" });
                }

                var horarios = await _context.HorariosDisponiveis
                    .Where(h => h.MedicoId == medico.Id)
                    .OrderBy(h => h.DataHoraInicio)
                    .ToListAsync();

                var horariosDto = horarios.Select(h => new HorarioDisponivelDto
                {
                    Id = h.Id,
                    MedicoId = h.MedicoId,
                    DataHoraInicio = h.DataHoraInicio,
                    DataHoraFim = h.DataHoraFim,
                    Disponivel = h.Disponivel
                }).ToList();

                return Ok(horariosDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar seus horários", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<HorarioDisponivelDto>> CreateHorario(CreateHorarioDisponivelDto createDto)
        {
            try
            {
                var userType = User.FindFirst("tipo")?.Value;
                if (userType != "medico")
                {
                    return Forbid("Apenas médicos podem criar horários");
                }

                var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.UsuarioId == userId);

                if (medico == null)
                {
                    return NotFound(new { message = "Médico não encontrado" });
                }

                var conflito = await _context.HorariosDisponiveis
                    .Where(h => h.MedicoId == medico.Id)
                    .Where(h => h.DataHoraInicio < createDto.DataHoraFim && h.DataHoraFim > createDto.DataHoraInicio)
                    .AnyAsync();

                if (conflito)
                {
                    return BadRequest(new { message = "Já existe um horário neste período" });
                }

                var horario = new HorarioDisponivel
                {
                    MedicoId = medico.Id,
                    DataHoraInicio = createDto.DataHoraInicio,
                    DataHoraFim = createDto.DataHoraFim,
                    Disponivel = true
                };

                _context.HorariosDisponiveis.Add(horario);
                await _context.SaveChangesAsync();

                var horarioDto = new HorarioDisponivelDto
                {
                    Id = horario.Id,
                    MedicoId = horario.MedicoId,
                    DataHoraInicio = horario.DataHoraInicio,
                    DataHoraFim = horario.DataHoraFim,
                    Disponivel = horario.Disponivel
                };

                return CreatedAtAction(nameof(GetHorario), new { id = horario.Id }, horarioDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao criar horário", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HorarioDisponivelDto>> GetHorario(int id)
        {
            try
            {
                var horario = await _context.HorariosDisponiveis
                    .Include(h => h.Medico)
                    .ThenInclude(m => m.Usuario)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (horario == null)
                {
                    return NotFound(new { message = "Horário não encontrado" });
                }

                var horarioDto = new HorarioDisponivelDto
                {
                    Id = horario.Id,
                    MedicoId = horario.MedicoId,
                    DataHoraInicio = horario.DataHoraInicio,
                    DataHoraFim = horario.DataHoraFim,
                    Disponivel = horario.Disponivel,
                    Medico = new MedicoDto
                    {
                        Id = horario.Medico.Id,
                        Crm = horario.Medico.Crm,
                        Especialidade = horario.Medico.Especialidade
                    }
                };

                return Ok(horarioDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar horário", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteHorario(int id)
        {
            try
            {
                var userType = User.FindFirst("tipo")?.Value;
                if (userType != "medico")
                {
                    return Forbid("Apenas médicos podem deletar horários");
                }

                var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.UsuarioId == userId);

                if (medico == null)
                {
                    return NotFound(new { message = "Médico não encontrado" });
                }

                var horario = await _context.HorariosDisponiveis
                    .FirstOrDefaultAsync(h => h.Id == id && h.MedicoId == medico.Id);

                if (horario == null)
                {
                    return NotFound(new { message = "Horário não encontrado" });
                }

                // CORREÇÃO: Comparar enum com enum, não com string
                var agendamento = await _context.Agendamentos
                    .FirstOrDefaultAsync(a => a.HorarioId == id && a.Status != StatusAgendamento.Cancelado);

                if (agendamento != null)
                {
                    return BadRequest(new { message = "Não é possível deletar um horário com agendamento ativo" });
                }

                _context.HorariosDisponiveis.Remove(horario);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao deletar horário", details = ex.Message });
            }
        }
    }
}