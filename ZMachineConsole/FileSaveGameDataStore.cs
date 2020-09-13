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

        public IFormChunk retrieveFormChunk()
        {
            byte[] data = File.ReadAllBytes(fileName);
            IMemory memory = new Memory(data);
            return new FormChunk(memory);
        }

        public bool saveFormChunk(WritableFormChunk formchunk)
        {
            File.WriteAllBytes(fileName, formchunk.Bytes);
            return true;
        }
    }
}
