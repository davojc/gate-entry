namespace GateEntry;


public record CameraSettings
{
    public string Url { get; set; } = "";

    public string User { get; set; } = "";

    public string Pwd { get; set; } = "";

    public double Frequency { get; set; } = 0.5;
}

public record PlateDetection
{
    public bool EnableGpuProcessing { get; set; } = false;

    public bool EnableGpuClassification { get; set; } = false;

    public int Concurrency { get; set; } = 0;

    public int CudaDevice { get; set; } = -1;

    public string Regex { get; set; } = "^(?!.*[IQ])[A-HJ-PR-Y]{2}[0-9]{2}(?!.*[IQ])[A-HJ-PR-Z]{3}$";
}

public record Settings
{
    public CameraSettings? Camera { get; set; }

    public PlateDetection? Plate { get; set; }

    public GateAutomation? Gate { get; set; }

    public bool SecureLog { get; set; } = true;
    public string Pin { get; set; } = "1234";

    public Storage? Storage { get; set; }

    public Host? Host { get; set; }

    public Testing? Test { get; set; }
}

public record GateAutomation
{
    public int Frequency { get; set; } = 10;
    public string Endpoint { get; set; } = "";

    public string Token { get; set; } = "";
}

public record Storage
{
    public string File { get; set; } = "plates.json";
}

public record Host
{
    public int Port { get; set; } = 80;
}

public record Testing
{
    public bool UseMockGateService { get; set; } = false;
}