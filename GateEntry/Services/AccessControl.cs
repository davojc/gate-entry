using System.Threading.Channels;
using GateEntry.Repository;
using Microsoft.Extensions.Options;

namespace GateEntry.Services;

public class AccessControl(
    ILogger<AccessControl> logger,
    IGateService gateService,
    IPlateAccessRepository plateAccessRepository,
    Channel<DetectedPlate> detectedPlateChannel,
    IOptions<Settings> settings)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await detectedPlateChannel.Reader.WaitToReadAsync(stoppingToken))
        {
            var detected = await detectedPlateChannel.Reader.ReadAsync(stoppingToken);

            //var hashed = HashHelper.ComputeSha256Hash(detected.Plate);

            var plate = await plateAccessRepository.TryGet(detected.Plate);

            if(plate != null)
            {
                if (plate.Seen.HasValue)
                {
                    var sinceUpdate = DateTime.UtcNow - plate.Seen.Value;

                    if (sinceUpdate.TotalSeconds < settings.Value.Gate.Frequency)
                        continue;
                }

                plate.Seen = DateTime.UtcNow;

                await plateAccessRepository.Updated(plate);

                if (settings.Value.SecureLog)
                    Console.WriteLine("Identified.");
                else
                    Console.WriteLine("Identified: {0} ({1})", plate.Number, plate.Enabled);

                if(plate.Enabled)
                    await gateService.Open(Gate.In);
            }
            else
            {
                if (!settings.Value.SecureLog)
                {
                    Console.WriteLine("Detected: {0}", detected.Plate);
                }
            }
        }
    }
}