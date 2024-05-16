using LoggingWithSerilog.Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoggingWithSerilog.Controllers
{
    [ApiController]
    [Route("api/filter-logs")]
    public class FilterLogController : ControllerBase
    {
        /// <summary>
        /// Retrieves and filters log entries based on the specified log level.
        /// </summary>
        /// <param name="level">The log level to filter by (e.g., "Information", "Warning", "Error").</param>
        /// <returns>An array of log entries filtered by the specified log level.</returns>
        [HttpGet("level-logs")]
        public async Task<IActionResult> FilterLogs(string? level)
        {
            var logs = ReadLogFiles();
            if (string.IsNullOrEmpty(level))
            {
                return Ok(FormatLogData(logs.Select(log => JObject.Parse(log.Value)).ToList()));
            }
            var filteredLogs = logs.Where(log => log.Key.Level == level)
                                   .Select(log => JObject.Parse(log.Value))
                                   .ToList();
            return Ok(FormatLogData(filteredLogs));
        }

        /// <summary>
        /// Retrieves and filters log entries within a specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the date range.</param>
        /// <param name="endDate">The end date of the date range.</param>
        /// <returns>An array of log entries filtered by the specified date range.</returns>
        [HttpGet("date-range-logs")]
        public IActionResult FilterLogs(DateTime startDate, DateTime endDate)
        {
            var logs = ReadLogFiles();
            var filteredLogs = logs.Where(log => log.Key.Timestamp >= startDate && log.Key.Timestamp <= endDate)
                                   .Select(log => JObject.Parse(log.Value))
                                   .ToList();
            return Ok(FormatLogData(filteredLogs));
        }

        /// <summary>
        /// Retrieves and filters log entries based on a message template, supporting fuzzy search.
        /// </summary>
        /// <param name="message">The search string to match against message templates.</param>
        /// <returns>An array of log entries where the message template contains the specified search string.</returns>
        [HttpGet("message-template-logs")]
        public IActionResult FilterMessageLogs(string message)
        {
            var logs = ReadLogFiles();
            var filteredLogs = logs.Where(log => log.Key.MessageTemplate.Contains(message))
                                   .Select(log => JObject.Parse(log.Value))
                                   .ToList();
            return Ok(FormatLogData(filteredLogs));
        }

        private string FormatLogData(List<JObject> data)
        {
            // Create a dictionary to hold the formatted log data
            var formattedData = new Dictionary<string, List<JObject>>
            {
                ["logs"] = data
            };

            // Serialize the formatted data to a JSON string
            return JsonConvert.SerializeObject(formattedData, Formatting.Indented);
        }

        private Dictionary<LogEntry, string> ReadLogFiles()
        {
            var logDictionary = new Dictionary<LogEntry, string>();

            // Get all log files in the Logs folder
            var logFiles = Directory.GetFiles(".\\Logs\\");

            foreach (var logFile in logFiles)
            {
                // Read the content of the log file
                var logFileContent = System.IO.File.ReadAllLines(logFile);

                foreach (var logLine in logFileContent)
                {
                    // Deserialize each log entry and add it to the dictionary
                    var logEntry = JsonConvert.DeserializeObject<LogEntry>(logLine);
                    logDictionary.Add(logEntry, logLine);
                }
            }

            return logDictionary;
        }
    }
}
