using MarketMate.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketMate.Api.Controllers
{
    [ApiController]
    [Route("api/sellers-data")]
    public class SellersDataController : ControllerBase
    {
        private readonly ISellersDataGenerator _dataGenerator;

        public SellersDataController(ISellersDataGenerator dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> FetchDataFromSadovodBase([FromQuery] int pageFrom, int pageTo)
        {
            var data = await _dataGenerator.GetSupplierInfoFromSadovodBaseAsync(pageFrom, pageTo);
            await _dataGenerator.SaveSuppliersToCsvAsync(data, "suppliersData.csv");
            return Ok(data);
        }
    }
}
