using MediatR;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application.Commands
{
    public record PostUserCommand(UserEntity userModel) : IRequest<UserEntity>;

    public class PostUserCommandHandler : IRequestHandler<PostUserCommand, UserEntity>
    {
        private readonly IUserRepository _userRepository;
        public PostUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;  
        }
        public async Task<UserEntity> Handle(PostUserCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.AddAsync(request.userModel);
        }
    }
}
