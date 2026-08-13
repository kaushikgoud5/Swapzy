using AutoMapper;
using Microsoft.Extensions.Logging;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Interests;
using Swapzy.Core.Enums;
using Swapzy.Core.Events;
using Swapzy.Core.Exceptions;

namespace Swapzy.Infrastructure.Services;

public class InterestService : IInterestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<InterestService> _logger;

    public InterestService(IUnitOfWork unitOfWork, IEventPublisher eventPublisher, IMapper mapper, ILogger<InterestService> logger)
    {
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _logger = logger;
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

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<InterestResponseDto>(interest);
    }
}
