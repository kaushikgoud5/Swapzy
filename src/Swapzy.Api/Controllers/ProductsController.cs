using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Enums;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("products")]
[Authorize]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await productService.CreateAsync(dto, userId);
        return StatusCode(201, new { productId = result.Id, product = result });
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await productService.GetByIdAsync(id);
        return Ok(new { product = result });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? ownerId = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] ProductStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var results = await productService.GetAllAsync(ownerId, categoryId, status, page, pageSize);
        return Ok(new { products = results });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await productService.UpdateAsync(id, dto, userId);
        return Ok(new { product = result });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await productService.DeleteAsync(id, userId);
        return Ok(new { message = "Product deleted successfully." });
    }

    [HttpPatch("{id:int}/toggle-availability")]
    public async Task<IActionResult> ToggleAvailability(int id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await productService.ToggleAvailabilityAsync(id, userId);
        return Ok(new { product = result });
    }
}
