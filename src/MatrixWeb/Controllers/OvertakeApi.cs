using MatrixWeatherDisplay.Data;
using MatrixWeb.Data;
using MatrixWeb.Services;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MatrixWeb.Controllers;
[Route("api/overtake")]
public class OvertakeApi : Controller {
    private const double s_secondsToShow = 60;

    private readonly DisplayService _displayService;

    public OvertakeApi(DisplayService displayService) {
        _displayService = displayService;
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody]UpdateRequest request) {
        var img = Image.Load<Rgb24>(request.Gif);
        await _displayService.Inject(new Screen(img, TimeSpan.FromSeconds(s_secondsToShow)));

        return Ok();
    }

    [HttpPost]
    public IActionResult Stop() {
        _displayService.StopInjection();
        return Ok();
    }
}
