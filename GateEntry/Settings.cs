namespace GateEntry;


public class CameraSettings
{
    public string Url { get; set; }

    public string User { get; set; }

    public string Pwd { get; set; }
}

public class PlateDetection
{
    public bool EnableGpuProcessing { get; set; } = false;

    public bool EnableGpuClassification { get; set; } = false;

    public int Concurrency { get; set; } = 0;

    public int CudaDevice { get; set; } = -1;

    public string Regex { get; set; } = "^(?!.*[IQ])[A-HJ-PR-Y]{2}[0-9]{2}(?!.*[IQ])[A-HJ-PR-Z]{3}$";
}

public class Settings
{
    public CameraSettings Camera { get; set; }

    public PlateDetection Plate { get; set; }

    public GateAutomation Gate { get; set; }

    public bool SecureLog { get; set; }
    public string Pin { get; set; }

    public Storage Storage { get; set; }

    public Host Host { get; set; }

    public Testing Test { get; set; }
}

public class GateAutomation
{
    public int Frequency { get; set; }
    public string Endpoint { get; set; }

    public string Token { get; set; }
}

public class Storage
{
    public string File { get; set; }
}

public class Host
{
    public int Port { get; set; }
}

public class Testing
{
    public bool UseMockGateService { get; set; } = false;
}