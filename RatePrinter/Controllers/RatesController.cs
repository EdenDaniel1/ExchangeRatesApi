using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Types;

namespace RatePrinter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllRates()
    {
        if (!System.IO.File.Exists(Constants.FilePath))
            return NotFound("Rates file not found.");

        var json = System.IO.File.ReadAllText(Constants.FilePath);
        var rates = JsonSerializer.Deserialize<List<ExchangeRate>>(json);

        return Ok(rates);
    }

    [HttpGet("{pair}")]
    public IActionResult GetRateByPair(string pair)
    {
        if (!System.IO.File.Exists(Constants.FilePath))
            return NotFound("Rates file not found.");

        var json = System.IO.File.ReadAllText(Constants.FilePath);
        var rates = JsonSerializer.Deserialize<List<ExchangeRate>>(json);

        var rate = rates?.FirstOrDefault(r => r.PairName.Equals(pair, StringComparison.OrdinalIgnoreCase));
        if (rate == null)
            return NotFound($"Pair {pair} not found.");

        return Ok(rate);
    }
}