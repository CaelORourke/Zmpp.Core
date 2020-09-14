namespace ZMachineConsole
{
    using Zmpp.Core;
    using Zmpp.Core.Iff;
    using Zmpp.Core.Vm;
    using System.IO;

    public class FileSaveGameDataStore : ISaveGameDataStore
    {
        private string fileName = "";

        public FileSaveGameDataStore(string fileName)
        {
            this.fileName = fileName;
        }

        public IFormChunk ReadFormChunk()
        {
            byte[] data = File.ReadAllBytes(fileName);
            IMemory memory = new Memory(data);
            return new FormChunk(memory);
        }

        public bool WriteFormChunk(WritableFormChunk formchunk)
        {
            File.WriteAllBytes(fileName, formchunk.Bytes);
            return true;
        }
    }
}
