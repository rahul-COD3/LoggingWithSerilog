using Microsoft.AspNetCore.Mvc;

namespace LoggingWithSerilog.Controllers
{
    [ApiController]
    [Route("api/log")]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;

        /// <summary>
        /// Constructor to inject the ILogger.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to write various log messages.
        /// </summary>
        /// <returns>OK response indicating the logs were written successfully.</returns>
        [HttpGet("log")]
        public IActionResult Log()
        {
            // Write log messages of different levels
            _logger.LogInformation("This is an information log message");
            _logger.LogWarning("This is a warning log message");
            _logger.LogError("This is an error log message");
            _logger.LogDebug("This is a debug log message");
            _logger.LogTrace("This is a trace log message");

            // Create log objects and log them
            var logObject = new { Property1 = "Value1", Property2 = "Value2" };
            var anotherObject = new { Property3 = "Value3", Property4 = logObject };
            var logObjectWithAnotherObject = new { logObject, anotherObject };
            _logger.LogInformation("This is an information log message with an object: {@logObject}", logObjectWithAnotherObject);

            return Ok("Logs written successfully");
        }

        /// <summary>
        /// Endpoint to simulate logging an error.
        /// </summary>
        /// <returns>500 status code response with an error message.</returns>
        [HttpGet("log-error")]
        public IActionResult LogError()
        {
            try
            {
                // Simulate an exception
                throw new Exception("This is a sample exception");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred");
                return StatusCode(500, "An error occurred");
            }
        }

        /// <summary>
        /// Endpoint to write a custom log message.
        /// </summary>
        /// <returns>OK response indicating the custom log was written successfully.</returns>
        [HttpGet("log-custom")]
        public IActionResult LogCustom()
        {
            var customLog = new { Message = "This is a custom log message", CustomProperty = "Custom value" };
            _logger.LogInformation("Custom log message: {@customLog}", customLog);

            return Ok("Custom log written successfully");
        }

        /// <summary>
        /// Endpoint to write log message within a scope.
        /// </summary>
        /// <returns>OK response indicating the scope log was written successfully.</returns>
        [HttpGet("log-scope")]
        public IActionResult LogScope()
        {
            // Log a message within a scope
            using (_logger.BeginScope("Scope: {ScopeValue}", "Scope1"))
            {
                _logger.LogInformation("This log message is within a scope");
            }

            return Ok("Scope log written successfully");
        }
    }
}
