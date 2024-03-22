using Microsoft.Maui.Handlers;
#if IOS || MACCATALYST
using PlatformView = Adriva.Camera.Maui.Platforms.Apple.MauiCameraView;
#elif ANDROID
using PlatformView = Adriva.Camera.Maui.Platforms.Android.MauiCameraView;
#elif WINDOWS
using PlatformView = Adriva.Camera.Maui.Platforms.Windows.MauiCameraView;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif

namespace Adriva.Camera.Maui;

public partial class CameraViewHandler : ViewHandler<CameraView, PlatformView>
{
    private static readonly IPropertyMapper<CameraView, CameraViewHandler> PropertyMapper = new PropertyMapper<CameraView, CameraViewHandler>(ViewMapper)
    {
        [nameof(CameraView.TorchEnabled)] = MapTorch,
        [nameof(CameraView.MirroredImage)] = MapMirroredImage,
        [nameof(CameraView.ZoomFactor)] = MapZoomFactor,
    };

    public static CommandMapper<CameraView, CameraViewHandler> CommandMapper = new(ViewCommandMapper)
    {
    };

    public CameraViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }

#if ANDROID
    protected override PlatformView CreatePlatformView() => new(Context, VirtualView);
#elif IOS || MACCATALYST || WINDOWS
    protected override PlatformView CreatePlatformView() => new(VirtualView);
#else
    protected override PlatformView CreatePlatformView() => new();
#endif

    protected override void DisconnectHandler(PlatformView platformView)
    {
#if WINDOWS || IOS || MACCATALYST || ANDROID
        platformView.DisposeControl();
#endif
        base.DisconnectHandler(platformView);
    }

    public static void MapTorch(CameraViewHandler handler, CameraView cameraView)
    {
#if WINDOWS || ANDROID || IOS || MACCATALYST
        handler.PlatformView?.UpdateTorch();
#endif
    }

    public static void MapMirroredImage(CameraViewHandler handler, CameraView cameraView)
    {
#if WINDOWS || ANDROID || IOS || MACCATALYST
        handler.PlatformView?.UpdateMirroredImage();
#endif
    }

    public static void MapZoomFactor(CameraViewHandler handler, CameraView cameraView)
    {
#if WINDOWS || ANDROID || IOS || MACCATALYST
        handler.PlatformView?.SetZoomFactor(cameraView.ZoomFactor);
#endif
    }

    public Task<CameraResult> StartCameraAsync(Size PhotosResolution)
    {
        if (PlatformView != null)
        {
#if WINDOWS || ANDROID || IOS || MACCATALYST
            return PlatformView.StartCameraAsync(PhotosResolution);
#endif
        }
        return Task.Run(() => { return CameraResult.AccessError; });
    }

    public Task<CameraResult> StartRecordingAsync(string file, Size Resolution)
    {
        if (PlatformView != null)
        {
#if WINDOWS || ANDROID || IOS
            return PlatformView.StartRecordingAsync(file, Resolution);
#endif
        }
        return Task.Run(() => { return CameraResult.PlatformNotSupported; });
    }

    public Task<CameraResult> StopCameraAsync()
    {
        if (PlatformView != null)
        {
#if WINDOWS
            return PlatformView.StopCameraAsync();
#elif ANDROID || IOS || MACCATALYST
            var task = new Task<CameraResult>(() => { return PlatformView.StopCamera(); });
            task.Start();
            return task;
#endif
        }
        return Task.Run(() => { return CameraResult.PlatformNotSupported; });
    }

    public Task<CameraResult> StopRecordingAsync()
    {
        if (PlatformView != null)
        {
#if WINDOWS || ANDROID || IOS || MACCATALYST
            return PlatformView.StopRecordingAsync();
#endif
        }
        return Task.Run(() => { return CameraResult.PlatformNotSupported; });
    }

    public ImageSource GetSnapShot(ImageFormat imageFormat)
    {
        if (PlatformView != null)
        {
#if WINDOWS || ANDROID || IOS || MACCATALYST
            return PlatformView.GetSnapShot(imageFormat);
#endif
        }
        return null;
    }

    public Task<Stream> TakePhotoAsync(ImageFormat imageFormat)
    {
        if (PlatformView != null)
        {
#if  IOS || MACCATALYST || WINDOWS
            return PlatformView.TakePhotoAsync(imageFormat);
#elif ANDROID
            return Task.Run(() => { return PlatformView.TakePhotoAsync(imageFormat); });
#endif
        }

        return Task.Run(() => { Stream result = null; return result; });
    }

    public async Task<bool> SaveSnapShotAsync(ImageFormat imageFormat, string SnapFilePath)
    {
        if (PlatformView != null)
        {
#if WINDOWS
            return await PlatformView.SaveSnapShot(imageFormat, SnapFilePath);
#elif ANDROID || IOS || MACCATALYST
            return await Task.Run<bool>(() => PlatformView.SaveSnapShot(imageFormat, SnapFilePath));
#endif
        }
        return await Task.FromResult(false);
    }

    public void ForceAutoFocus()
    {
#if ANDROID || WINDOWS || IOS || MACCATALYST
        PlatformView?.ForceAutoFocus();
#endif
    }

    public void ForceDispose()
    {
#if ANDROID || WINDOWS || IOS || MACCATALYST
        PlatformView?.DisposeControl();
#endif
    }
}
