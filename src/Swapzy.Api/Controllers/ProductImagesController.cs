using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using System.Security.Claims;

namespace Swapzy.Api.Controllers;

[ApiController]
[Route("products/{productId:int}/images")]
[Authorize]
public class ProductImagesController(IProductImageService imageService) : ControllerBase
{
    [HttpPost("upload-url")]
    public async Task<IActionResult> GetUploadUrl(int productId, [FromQuery] string contentType = "image/jpeg")
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await imageService.GenerateUploadUrlAsync(productId, contentType, userId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmUpload(int productId, [FromBody] ConfirmImageDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await imageService.ConfirmUploadAsync(productId, dto.S3Key, dto.ContentType, userId);
        return StatusCode(201, result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(int productId)
    {
        var result = await imageService.GetAllAsync(productId);
        return Ok(new { images = result });
    }

    [HttpDelete("{imageId:int}")]
    public async Task<IActionResult> Delete(int productId, int imageId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await imageService.DeleteAsync(productId, imageId, userId);
        return Ok(new { message = "Image deleted." });
    }
}
