
    using StarBord.Data;
    using StarBord.DTOS;
    using StarBord.Models;
    using StarBord.Services.IService;
    using Microsoft.EntityFrameworkCore;

    namespace StarBord.Services
    {
        public class PlatformTokenService : IPlatformTokenService
        {
            private readonly StarBordDbContext _context;
            private readonly ILogger<PlatformTokenService> _logger;

            public PlatformTokenService(StarBordDbContext context, ILogger<PlatformTokenService> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<GetPlatformTokenDto> CreatePlatformTokenAsync(CreatePlatformTokenDto dto, CancellationToken cancellationToken)
            {
                try
                {
                    var token = new PlatformToken
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = dto.BusinessId,
                        Platform = dto.Platform,
                        AccessToken = dto.AccessToken,
                        ExpiresAt = dto.ExpiresAt
                    };

                    await _context.PlatformTokens.AddAsync(token, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("PlatformToken {TokenId} created for business {BusinessId} on {Platform}", token.Id, token.BusinessId, token.Platform);

                    return new GetPlatformTokenDto
                    {
                        Id = token.Id,
                        BusinessId = token.BusinessId,
                        Platform = token.Platform,
                        ExpiresAt = token.ExpiresAt
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating platform token for business {BusinessId}", dto.BusinessId);
                    throw;
                }
            }

            public async Task<GetPlatformTokenDto?> GetPlatformTokenByIdAsync(Guid id, CancellationToken cancellationToken)
            {
                try
                {
                    var token = await _context.PlatformTokens
                        .Where(t => t.Id == id)
                        .Select(t => new GetPlatformTokenDto
                        {
                            Id = t.Id,
                            BusinessId = t.BusinessId,
                            Platform = t.Platform,
                            ExpiresAt = t.ExpiresAt
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);

                    return token;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving platform token {TokenId}", id);
                    throw;
                }
            }

            public async Task<IEnumerable<GetPlatformTokenDto>> GetPlatformTokensByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken)
            {
                try
                {
                    var tokens = await _context.PlatformTokens
                        .Where(t => t.BusinessId == businessId)
                        .Select(t => new GetPlatformTokenDto
                        {
                            Id = t.Id,
                            BusinessId = t.BusinessId,
                            Platform = t.Platform,
                            ExpiresAt = t.ExpiresAt
                        })
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                    return tokens;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving platform tokens for business {BusinessId}", businessId);
                    throw;
                }
            }

            public async Task<GetPlatformTokenDto?> UpdatePlatformTokenAsync(Guid id, CreatePlatformTokenDto dto, CancellationToken cancellationToken)
            {
                try
                {
                    var token = await _context.PlatformTokens.FindAsync(new object[] { id }, cancellationToken);
                    if (token == null) return null;

                    token.Platform = dto.Platform;
                    token.AccessToken = dto.AccessToken;
                    token.ExpiresAt = dto.ExpiresAt;

                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("PlatformToken {TokenId} updated", id);

                    return new GetPlatformTokenDto
                    {
                        Id = token.Id,
                        BusinessId = token.BusinessId,
                        Platform = token.Platform,
                        ExpiresAt = token.ExpiresAt
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating platform token {TokenId}", id);
                    throw;
                }
            }

            public async Task<bool> DeletePlatformTokenAsync(Guid id, CancellationToken cancellationToken)
            {
                try
                {
                    var token = await _context.PlatformTokens.FindAsync(new object[] { id }, cancellationToken);
                    if (token == null)
                    {
                        _logger.LogError("PlatformToken with id {TokenId} not found for deletion", id);
                        return false;
                    }

                    _context.PlatformTokens.Remove(token);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("PlatformToken {TokenId} deleted", id);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting platform token {TokenId}", id);
                    throw;
                }
            }
        }
    }
