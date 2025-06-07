namespace GateEntry.Processors;

public interface IPlateDetector
{
    IEnumerable<string> DetectPlates(byte[] imageData);

    IEnumerable<string> DetectPlates(Stream imageData);
}