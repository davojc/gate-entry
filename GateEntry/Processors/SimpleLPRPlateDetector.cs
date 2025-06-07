using Microsoft.Extensions.Options;
using SimpleLPR3;

namespace GateEntry.Processors;

public class SimpleLPRPlateDetector : IPlateDetector
{
    private readonly ISimpleLPR _lpr;

    public SimpleLPRPlateDetector(IOptions<Settings> settings)
    {
        EngineSetupParms setupP;
        setupP.cudaDeviceId = settings.Value.Plate.CudaDevice; // Select CPU
        setupP.enableImageProcessingWithGPU = settings.Value.Plate.EnableGpuProcessing;
        setupP.enableClassificationWithGPU = settings.Value.Plate.EnableGpuClassification;
        setupP.maxConcurrentImageProcessingOps = settings.Value.Plate.Concurrency;  // Use the default value. 

        _lpr = SimpleLPR.Setup(setupP);

        // Configure country weights based on the selected country
        uint countryId = 84;

        if (countryId >= _lpr.numSupportedCountries)
            throw new Exception("Invalid country id");

        for (uint ui = 0; ui < _lpr.numSupportedCountries; ++ui)
            _lpr.set_countryWeight(ui, 0.0f);

        _lpr.set_countryWeight(countryId, 1.0f);

        _lpr.realizeCountryWeights();
    }

    public IEnumerable<string> DetectPlates(byte[] imageData)
    {
        var proc = GetProcessor();
        var cds = proc.analyze(imageData);

        return Process(cds);
    }

    public IEnumerable<string> DetectPlates(Stream imageData)
    {
        var proc = GetProcessor();
        var cds = proc.analyze(imageData);

        return Process(cds);
    }

    private IProcessor GetProcessor()
    {
        // Create a processor object
        IProcessor proc = _lpr.createProcessor();
        proc.plateRegionDetectionEnabled = true;
        proc.cropToPlateRegionEnabled = true;

        return proc;
    }

    private IEnumerable<string> Process(List<Candidate> cds)
    {
        if (cds.Count == 0)
        {
            //Console.WriteLine("No license plate found");
            yield break;
        }

        //Console.WriteLine("{0} license plate candidates found:", cds.Count);

        // Iterate over all candidates
        foreach (Candidate cd in cds)
        {
            /*
            Console.WriteLine("***********");
            Console.WriteLine("Light background: {0}, left: {1}, top: {2}, width: {3}, height: {4}", cd.brightBackground, cd.bbox.Left, cd.bbox.Top, cd.bbox.Width, cd.bbox.Height);
            Console.WriteLine("Plate confidence: {0}. Plate vertices: {1}", cd.plateDetectionConfidence, string.Join(", ", cd.plateRegionVertices));
            Console.WriteLine("Matches:");
            */
            foreach (CountryMatch match in cd.matches)
            {
                var result = match.text.Replace(" ", "");

                result = result.Replace("I", "1");

                yield return result;

                /*
                Console.WriteLine("-----------");
                Console.WriteLine("Text: {0}, country: {1}, ISO: {2}, confidence: {3}", match.text, match.country, match.countryISO, match.confidence);
                Console.WriteLine("Elements:");

                foreach (Element e in match.elements)
                {
                    Console.WriteLine("   Glyph: {0}, confidence: {1}, left: {2}, top: {3}, width: {4}, height: {5}",
                        e.glyph, e.confidence, e.bbox.Left, e.bbox.Top, e.bbox.Width, e.bbox.Height);
                }
                */
            }
        }
    }
}