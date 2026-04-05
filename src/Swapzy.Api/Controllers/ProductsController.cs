using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
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
}
