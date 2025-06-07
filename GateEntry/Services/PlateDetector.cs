using System.Text.RegularExpressions;
using System.Threading.Channels;
using GateEntry.Processors;
using Microsoft.Extensions.Options;

namespace GateEntry.Services;

public class PlateDetector(
    Channel<CameraImage> imageChannel,
    Channel<DetectedPlate> plateChannel,
    IPlateDetector plateDetector,
    IOptions<Settings> settings)
    : BackgroundService
{
    private readonly Regex _numberPlateRegex = new(settings.Value.Plate.Regex,
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await imageChannel.Reader.WaitToReadAsync(stoppingToken))
        {
            var image = await imageChannel.Reader.ReadAsync(stoppingToken);

            foreach (var found in plateDetector.DetectPlates(image.Data))
            {
                if (!_numberPlateRegex.IsMatch(found))
                    continue;

                await plateChannel.Writer.WriteAsync(new DetectedPlate { Plate = found }, stoppingToken);
            }
        }
    }
}