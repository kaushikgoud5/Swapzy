using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Interests;
using Swapzy.Core.Entities.Notifications;
using Swapzy.Core.Enums;
using Swapzy.Core.Events;
using Swapzy.Core.Exceptions;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Services;

public class InterestService : IInterestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<InterestService> _logger;
    private readonly SwapzyDbContext _context;
    private readonly IStorageService _storageService;

    public InterestService(IUnitOfWork unitOfWork, IEventPublisher eventPublisher, IMapper mapper, ILogger<InterestService> logger, SwapzyDbContext context, IStorageService storageService)
    {
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _logger = logger;
        _context = context;
        _storageService = storageService;
    }

    public async Task<InterestResponseDto> ExpressInterestAsync(Guid buyerId, int productId)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId)
            ?? throw new NotFoundException($"Product {productId} not found.");

        if (product.OwnerId == buyerId)
            throw new BadRequestException("You cannot express interest in your own product.");

        var existing = await _unitOfWork.Interests.GetByBuyerAndProductAsync(buyerId, productId);
        if (existing != null)
            return _mapper.Map<InterestResponseDto>(existing);

        var interest = new Interest
        {
            Id = Guid.NewGuid(),
            BuyerId = buyerId,
            SellerId = product.OwnerId,
            ProductId = productId,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = buyerId.ToString()
        };

        await _unitOfWork.Interests.AddAsync(interest);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            await _eventPublisher.PublishAsync(new InterestCreatedEvent
            {
                InterestId = interest.Id,
                BuyerId = buyerId,
                SellerId = interest.SellerId,
                ProductId = productId
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish InterestCreatedEvent for interest {InterestId}", interest.Id);
        }

        interest.Product = product;
        return _mapper.Map<InterestResponseDto>(interest);
    }

    public async Task<List<InterestResponseDto>> GetInterestsForSellerAsync(Guid sellerId, int page = 1, int pageSize = 20)
    {
        var interests = await _unitOfWork.Interests.GetBySellerAsync(sellerId, page, pageSize);
        return _mapper.Map<List<InterestResponseDto>>(interests);
    }

    public async Task<List<InterestResponseDto>> GetInterestsForBuyerAsync(Guid buyerId, int page = 1, int pageSize = 20)
    {
        var interests = await _unitOfWork.Interests.GetByBuyerAsync(buyerId, page, pageSize);
        return _mapper.Map<List<InterestResponseDto>>(interests);
    }

    public async Task<InterestResponseDto> UpdateStatusAsync(Guid interestId, InterestStatus status, Guid userId)
    {
        var interest = await _unitOfWork.Interests.GetByIdAsync(interestId)
            ?? throw new NotFoundException($"Interest {interestId} not found.");

        if (interest.SellerId != userId)
            throw new ForbiddenException("Only the seller can update interest status.");

        interest.Status = status;
        interest.ModifiedOn = DateTime.UtcNow;
        interest.ModifiedBy = userId.ToString();

        if (status == InterestStatus.Accepted)
        {
            var alreadyMatched = await _context.Matches.AnyAsync(m => m.InterestId == interestId);
            if (!alreadyMatched)
            {
                await _context.Matches.AddAsync(new Match
                {
                    Id = Guid.NewGuid(),
                    InterestId = interestId,
                    BuyerId = interest.BuyerId,
                    SellerId = interest.SellerId,
                    ProductId = interest.ProductId,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = userId.ToString()
                });

                // notify buyer
                await _context.Notifications.AddAsync(new Notification
                {
                    UserId = interest.BuyerId,
                    Title = "It's a match!",
                    Message = $"The seller accepted your interest in {interest.Product.Name}.",
                    EventType = "MatchCreatedEvent",
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "system"
                });
            }
        }

        await _unitOfWork.SaveChangesAsync();
        await _context.SaveChangesAsync();
        return _mapper.Map<InterestResponseDto>(interest);
    }

    public async Task<List<MatchResponseDto>> GetMatchesAsync(Guid userId, int page, int pageSize)
    {
        var matches = await _context.Matches
            .Include(m => m.Product)
                .ThenInclude(p => p.Images.Where(i => i.DateDeleted == null))
            .Where(m => m.DateDeleted == null && (m.BuyerId == userId || m.SellerId == userId))
            .OrderByDescending(m => m.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return matches.Select(m => new MatchResponseDto
        {
            Id = m.Id,
            InterestId = m.InterestId,
            BuyerId = m.BuyerId,
            SellerId = m.SellerId,
            ProductId = m.ProductId,
            ProductName = m.Product.Name,
            ProductImageUrl = m.Product.Images.OrderBy(i => i.DisplayOrder).Select(i => _storageService.GetPublicUrl(i.S3Key)).FirstOrDefault(),
            IsSwapped = m.Product.Status == Swapzy.Core.Enums.ProductStatus.Sold,
            CreatedOn = m.CreatedOn
        }).ToList();
    }
}
