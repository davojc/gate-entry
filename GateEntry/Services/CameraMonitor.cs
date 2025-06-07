using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Options;

namespace GateEntry.Services;

public class CameraMonitor : BackgroundService
{
    private readonly Channel<CameraImage> _imageChannel;
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly int _seconds = 1;
    private readonly string _url;

    public CameraMonitor(Channel<CameraImage> imageChannel, IOptions<Settings> settings)
    {
        _imageChannel = imageChannel;
        var cam = settings.Value.Camera;

        _url = $"{cam.Url}/cgi-bin/api.cgi?cmd=Snap&channel=0&rs=abc&user={cam.User}&password={cam.Pwd}";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var imageData = await _httpClient.GetByteArrayAsync(_url, stoppingToken);
                await _imageChannel.Writer.WriteAsync(new CameraImage { Data = imageData }, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.Delay(TimeSpan.FromSeconds(_seconds), stoppingToken);
        }
    }
}