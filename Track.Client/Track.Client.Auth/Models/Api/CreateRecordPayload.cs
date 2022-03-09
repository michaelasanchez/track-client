namespace Track.Broker.Models.Api
{
    internal class CreateRecordPayload
    {
        public DateTime RecordDate { get; set; }

        // TODO:
        //public Location Location { get; set; }
        //public IEnumerable<Note> Notes { get; set; }

        public Dictionary<int, decimal?> Properties { get; set; }

        public CreateRecordPayload() { }
    }
}
