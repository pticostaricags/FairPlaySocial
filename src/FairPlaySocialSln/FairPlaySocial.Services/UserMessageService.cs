﻿using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Services
{
    [ServiceOfEntity(entityName: nameof(UserMessage), primaryKeyType: typeof(long))]
    public partial class UserMessageService
    {
    }
}
