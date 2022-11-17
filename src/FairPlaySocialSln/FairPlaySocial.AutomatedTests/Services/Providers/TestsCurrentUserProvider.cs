using FairPlaySocial.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.AutomatedTests.Services.Providers
{
    internal class TestsCurrentUserProvider : ICurrentUserProvider
    {
        public string GetObjectId()
        {
            return "Tests_ObjectId";
        }

        public string GetUsername()
        {
            return "Tests_Username";
        }

        public bool IsLoggedIn()
        {
            return true;
        }
    }
}
