using MarketMate.Api.Requests;
using MarketMate.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketMate.Api.Controllers;

[ApiController]
[Route("api/photo-organizer")]
public class PhotoOrganizerController : ControllerBase
{
    private readonly IPhotoOrganizer _photoOrganizer;

    public PhotoOrganizerController(IPhotoOrganizer photoOrganizer)
    {
        _photoOrganizer = photoOrganizer;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPhotoOrganization([FromBody] ProcessPhotoOrganizationRequest request)
    {
        await _photoOrganizer.ProcessAsync(request.PurchaseId, request.AlbumId);
        return Ok("{\"Xyy\": \"PIZDA\"}");
    }
}
