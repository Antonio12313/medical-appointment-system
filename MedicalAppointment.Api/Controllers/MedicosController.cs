using MedicalAppointment.Api.Data;
using MedicalAppointment.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicoDto>>> GetMedicos()
    {
        try
        {
            var medicos = await _context.Medicos
                .Include(m => m.Usuario)
                .Select(m => new MedicoDto
                {
                    Id = m.Id,
                    Crm = m.Crm,
                    Especialidade = m.Especialidade,
                    Nome = m.Usuario.Nome,
                })
                .ToListAsync();

            return Ok(medicos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao buscar médicos", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MedicoDto>> GetMedico(int id)
    {
        try
        {
            var medico = await _context.Medicos
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null)
            {
                return NotFound(new { message = "Médico não encontrado" });
            }

            var medicoDto = new MedicoDto
            {
                Id = medico.Id,
                Crm = medico.Crm,
                Especialidade = medico.Especialidade,
                Nome = medico.Usuario.Nome
            };

            return Ok(medicoDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao buscar médico", details = ex.Message });
        }
    }

    [HttpGet("{id}/horarios")]
    public async Task<ActionResult<IEnumerable<HorarioDisponivelDto>>> GetHorariosMedico(int id)
    {
        try
        {
            var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.Id == id);
            if (medico == null)
            {
                return NotFound(new { message = "Médico não encontrado" });
            }

            var horarios = await _context.HorariosDisponiveis
                .Include(h => h.Medico)
                .Where(h => h.MedicoId == id && h.Disponivel)
                .OrderBy(h => h.DataHoraInicio)
                .Select(h => new HorarioDisponivelDto
                {
                    Id = h.Id,
                    MedicoId = h.MedicoId,
                    DataHoraInicio = h.DataHoraInicio,
                    DataHoraFim = h.DataHoraFim,
                    Disponivel = h.Disponivel,
                    Medico =new MedicoDto
                    {
                        Id = h.Medico.Id,
                        Crm = h.Medico.Crm,
                        Especialidade = h.Medico.Especialidade,
                        Nome = h.Medico.Usuario.Nome
                    }
                })
                .ToListAsync();

            return Ok(horarios);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao buscar horários do médico", details = ex.Message });
        }
    }
}