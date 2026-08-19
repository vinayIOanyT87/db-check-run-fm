namespace FMPointTagArchive.Core.InternalClasses
{
    internal class TagData : ArchiveTagData
    {
        public TagData()
        {
            this.ID = string.Empty;
        }

        public string ID { get; set; }
    }
}
