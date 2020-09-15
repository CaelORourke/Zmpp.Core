namespace ZMachineConsole
{
    using System.IO;
    using Zmpp.Core.Blorb;
    using Zmpp.Core.IO;
    using Zmpp.Core.UI;
    using Zmpp.Core.Vm;

    public class ConsoleViewModel : ViewModel
    {
        public ConsoleViewModel(string storyFilePath) : this(new ConsoleInput(), new ConsoleStatusLine(), new ConsoleScreenModel(), null, new FileSaveGameDataStore(Path.Combine(Path.GetDirectoryName(storyFilePath), string.Concat(Path.GetFileNameWithoutExtension(storyFilePath), ".dat"))), null, null)
        {
        }

        private ConsoleViewModel(IInputStream inputStream,
            IStatusLine statusLine,
            IScreenModel screenModel,
            IIOSystem ioSystem,
            ISaveGameDataStore saveGameDataStore,
            INativeImageFactory nativeImageFactory,
            ISoundEffectFactory soundEffectFactory)
            : base(inputStream, statusLine, screenModel, ioSystem, saveGameDataStore, nativeImageFactory, soundEffectFactory)
        {
        }
    }
}
