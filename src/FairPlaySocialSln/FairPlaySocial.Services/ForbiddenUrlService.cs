﻿using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Services
{
    [ServiceOfEntity(entityName:nameof(ForbiddenUrl), primaryKeyType:typeof(long))]
    public partial class ForbiddenUrlService
    {
    }
}
