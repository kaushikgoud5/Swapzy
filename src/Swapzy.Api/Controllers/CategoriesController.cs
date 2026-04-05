using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Exceptions;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("categories")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var categories = await categoryService.GetAllAsync(includeInactive);
        return Ok(new { categories });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cat = await categoryService.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category with ID {id} not found.");
        return Ok(new { category = cat });
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var cat = await categoryService.GetBySlugAsync(slug)
            ?? throw new NotFoundException($"Category with slug '{slug}' not found.");
        return Ok(new { category = cat });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await categoryService.CreateAsync(dto, userId);
        return StatusCode(201, new { categoryId = result.Id, category = result });
    }
}
