using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.Commands;
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

        [HttpPost]
        public async Task<UserEntity> Post([FromBody] UserEntity user)
        {
            return await _mediator.Send(new PostUserCommand(user));
        }
    }
}
