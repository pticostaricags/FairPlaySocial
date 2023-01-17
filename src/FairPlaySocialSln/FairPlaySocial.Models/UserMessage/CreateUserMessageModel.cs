using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.UserMessage
{
    /// <summary>
    /// Represents the User Message entry
    /// </summary>
    public class CreateUserMessageModel
    {
        /// <summary>
        /// ApplicationUserId of the user to whom the message is sent
        /// </summary>
        public long? ToApplicationUserId { get; set; }
        /// <summary>
        /// Message to be sent
        /// </summary>
        [Required]
        public string? Message { get; set; }
    }
}
