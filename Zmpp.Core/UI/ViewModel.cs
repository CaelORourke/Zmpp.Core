namespace Zmpp.Core.UI
{
    using Zmpp.Core.Blorb;
    using Zmpp.Core.IO;
    using Zmpp.Core.Vm;

    public abstract class ViewModel : IViewModel
    {
        private readonly IInputStream inputStream;
        private readonly IStatusLine statusLine;
        private readonly IScreenModel screenModel;
        private readonly IIOSystem ioSystem;
        private readonly ISaveGameDataStore saveGameDataStore;
        private readonly INativeImageFactory nativeImageFactory;
        private readonly ISoundEffectFactory soundEffectFactory;

        public IInputStream InputStream => inputStream;
        public IStatusLine StatusLine => statusLine;
        public IScreenModel ScreenModel => screenModel;
        public IIOSystem IoSystem => ioSystem;
        public ISaveGameDataStore SaveGameDataStore => saveGameDataStore;
        public INativeImageFactory NativeImageFactory => nativeImageFactory;
        public ISoundEffectFactory SoundEffectFactory => soundEffectFactory;

        public ViewModel(
            IInputStream inputStream,
            IStatusLine statusLine,
            IScreenModel screenModel,
            IIOSystem ioSystem,
            ISaveGameDataStore saveGameDataStore,
            INativeImageFactory nativeImageFactory,
            ISoundEffectFactory soundEffectFactory
            )
        {
            this.inputStream = inputStream;
            this.statusLine = statusLine;
            this.screenModel = screenModel;
            this.ioSystem = ioSystem;
            this.saveGameDataStore = saveGameDataStore;
            this.nativeImageFactory = nativeImageFactory;
            this.soundEffectFactory = soundEffectFactory;
        }
    }
}
