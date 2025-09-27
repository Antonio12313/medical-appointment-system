using MedicalAppointment.Api.DTOs;
using MedicalAppointment.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUsuarioService _usuarioService;

    public AuthController(IAuthService authService, IUsuarioService usuarioService)
    {
        _authService = authService;
        _usuarioService = usuarioService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            var usuario = await _usuarioService.GetByEmailAsync(loginDto.Email);

            if (usuario == null || !_authService.VerifyPassword(loginDto.Senha, usuario.Senha))
            {
                return BadRequest(new { message = "Email ou senha inválidos" });
            }

            var token = _authService.GenerateJwtToken(usuario);

            var usuarioDto = new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                DataCriacao = usuario.DataCriacao,
                Medico = usuario.Medico != null
                    ? new MedicoDto
                    {
                        Id = usuario.Medico.Id,
                        Crm = usuario.Medico.Crm,
                        Especialidade = usuario.Medico.Especialidade
                    }
                    : null,
                Paciente = usuario.Paciente != null
                    ? new PacienteDto
                    {
                        Id = usuario.Paciente.Id,
                        Cpf = usuario.Paciente.Cpf,
                        Telefone = usuario.Paciente.Telefone
                    }
                    : null
            };

            return Ok(new AuthResponseDto
            {
                Token = token,
                Usuario = usuarioDto
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(CreateUsuarioDto createUsuarioDto)
    {
        try
        {
            var existingUser = await _usuarioService.GetByEmailAsync(createUsuarioDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email já cadastrado" });
            }

            if (createUsuarioDto.TipoUsuario != "medico" && createUsuarioDto.TipoUsuario != "paciente")
            {
                return BadRequest(new { message = "Tipo de usuário deve ser 'medico' ou 'paciente'" });
            }

            if (createUsuarioDto.TipoUsuario == "medico" && createUsuarioDto.DadosMedico == null)
            {
                return BadRequest(new { message = "Dados do médico são obrigatórios" });
            }

            if (createUsuarioDto.TipoUsuario == "paciente" && createUsuarioDto.DadosPaciente == null)
            {
                return BadRequest(new { message = "Dados do paciente são obrigatórios" });
            }

            var hashedPassword = _authService.HashPassword(createUsuarioDto.Senha);
            var usuario = await _usuarioService.CreateAsync(createUsuarioDto, hashedPassword);

            usuario = await _usuarioService.GetByIdAsync(usuario.Id);

            var token = _authService.GenerateJwtToken(usuario!);

            var usuarioDto = new UsuarioDto
            {
                Id = usuario!.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                DataCriacao = usuario.DataCriacao,
                Medico = usuario.Medico != null
                    ? new MedicoDto
                    {
                        Id = usuario.Medico.Id,
                        Crm = usuario.Medico.Crm,
                        Especialidade = usuario.Medico.Especialidade
                    }
                    : null,
                Paciente = usuario.Paciente != null
                    ? new PacienteDto
                    {
                        Id = usuario.Paciente.Id,
                        Cpf = usuario.Paciente.Cpf,
                        Telefone = usuario.Paciente.Telefone
                    }
                    : null
            };

            return CreatedAtAction(nameof(GetMe), new AuthResponseDto
            {
                Token = token,
                Usuario = usuarioDto
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetMe()
    {
        try
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var usuario = await _usuarioService.GetByIdAsync(userId);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var usuarioDto = new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                DataCriacao = usuario.DataCriacao,
                Medico = usuario.Medico != null
                    ? new MedicoDto
                    {
                        Id = usuario.Medico.Id,
                        Crm = usuario.Medico.Crm,
                        Especialidade = usuario.Medico.Especialidade
                    }
                    : null,
                Paciente = usuario.Paciente != null
                    ? new PacienteDto
                    {
                        Id = usuario.Paciente.Id,
                        Cpf = usuario.Paciente.Cpf,
                        Telefone = usuario.Paciente.Telefone
                    }
                    : null
            };

            return Ok(usuarioDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro interno do servidor", details = ex.Message });
        }
    }
}