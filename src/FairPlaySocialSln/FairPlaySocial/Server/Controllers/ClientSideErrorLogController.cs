﻿using FairPlaySocial.Models.ClientSideErrorLog;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles all the data regarding Client Side Errors
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientSideErrorLogController : ControllerBase
    {
        private readonly ClientSideErrorLogService ClientSideErrorLogService;

        /// <summary>
        /// Initializes <see cref="ClientSideErrorLogController"/>
        /// </summary>
        /// <param name="clientSideErrorLogService"></param>
        public ClientSideErrorLogController(ClientSideErrorLogService clientSideErrorLogService)
        {
            this.ClientSideErrorLogService = clientSideErrorLogService;
        }

        /// <summary>
        /// Adds a new client side error entry
        /// </summary>
        /// <param name="createClientSideErrorLogModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> AddClientSideErrorAsync(
            CreateClientSideErrorLogModel createClientSideErrorLogModel,
            CancellationToken cancellationToken)
        {
            await this.ClientSideErrorLogService
                .AddClientSideErrorAsync(createClientSideErrorLogModel, cancellationToken);
            return Ok();
        }
    }
}
