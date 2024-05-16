namespace LoggingWithSerilog.Entity
{
    public class LogEntry
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
    }
}
