using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.Commands;
using Swapzy.Application.Commands.LoginUser;
using Swapzy.Application.Commands.RegisterUser;
using Swapzy.Core.Entities;

namespace Swapzy.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<Guid> Register([FromBody] RegisterUserDto user)
        {
            return await _mediator.Send(new RegisterUserCommand(user));
        }

        [HttpPost("login")]
        public async Task<string> Login([FromBody] LoginUserDto loginUserDto)
        { 
            return  await _mediator.Send(new LoginUserCommand(loginUserDto));
		}
    }
}
