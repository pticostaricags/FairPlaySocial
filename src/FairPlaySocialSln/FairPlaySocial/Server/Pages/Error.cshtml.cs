using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace FairPlaySocial.Server.Pages
{
    /// <summary>
    /// Represents request error.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// Request id.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Determines whether the request id is available.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        /// <summary>
        /// <see cref="ErrorModel"/> constructor.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> instance.</param>
        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes <see cref="ErrorModel"/> with the current request.
        /// </summary>
        public void OnGet()
        {
            this._logger.LogDebug("Getting Error Information");
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}