namespace Adriva.Camera.Maui.ZXingHelper;

public record BarcodeEventArgs
{
    public BarcodeResult[] Result { get; init; }
}
