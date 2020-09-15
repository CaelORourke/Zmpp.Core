namespace Zmpp.Core.UI
{
    using Zmpp.Core.Blorb;
    using Zmpp.Core.IO;
    using Zmpp.Core.Vm;

    public interface IViewModel
    {
        IInputStream InputStream { get; }
        IStatusLine StatusLine { get; }
        IScreenModel ScreenModel { get; }
        IIOSystem IoSystem { get; }
        ISaveGameDataStore SaveGameDataStore { get; }
        INativeImageFactory NativeImageFactory { get; }
        ISoundEffectFactory SoundEffectFactory { get; }
    }
}
