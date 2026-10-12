using MediatR;
using Microsoft.EntityFrameworkCore;
using StarBord.Application.Exceptions;
using StarBord.Data;
using StarBord.DTOS;
using StarBord.Models;

namespace StarBord.Application.Users
{
    public record CreateUserCommand(CreateUserDto UserDto) : IRequest<GetUserDto>;
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, GetUserDto>
    {
        private readonly StarBordDbContext _context;

        public CreateUserCommandHandler(StarBordDbContext context) => _context = context;

        public async Task<GetUserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var email = request.UserDto.Email.Trim().ToLowerInvariant();

            var emailTaken = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email, cancellationToken);

            if (emailTaken)
            throw new ConflictException("An account with this email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.UserDto.Username,
                Email = request.UserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.UserDto.Password),
                CreatedAt = DateTime.UtcNow
            };
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new GetUserDto
            {
                Username = user.Username,
                Email = user.Email
            };

        }

    }
}
