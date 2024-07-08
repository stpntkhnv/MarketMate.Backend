using MarketMate.Api.Requests;
using MarketMate.Api.Responses;
using MarketMate.Application.Abstractions.Services;
using MarketMate.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace MarketMate.Api.Controllers;

[ApiController]
[Route("api/purchases")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest request)
    {
        var response = await _purchaseService.CreateAsync(new PurchaseDto
        {
            AlbumId = request.AlbumId,
            CommunityId = request.CommunityId,
            AgentId = request.AgentId,
            Description = request.Description,
            EndDate = request.EndDate,
            Name = request.Name,
            StartDate = request.StartDate
        });

        return Ok(new CreatePurchaseResponse
        {
            PurchaseId = response.Id
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPurchases()
    {
        var purchases = await _purchaseService.GetAllAsync();
        return Ok(purchases);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPurchaseById([FromRoute] string id)
    {
        var purchase = await _purchaseService.GetByIdAsync(id);

        if (purchase == null)
        {
            return NotFound();
        }

        return Ok(purchase);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePurchase([FromRoute] string id)
    {
        var purchase = await _purchaseService.GetByIdAsync(id);

        if (purchase == null)
        {
            return NotFound();
        }

        await _purchaseService.DeleteAsync(id);
        return NoContent();
    }
}
