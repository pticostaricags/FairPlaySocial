using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Common.CustomAttributes.Localization
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceKeyAttribute : Attribute
    {
        public string DefaultValue { get; }
        public ResourceKeyAttribute(string defaultValue)
        {
            this.DefaultValue = defaultValue;
        }
    }
}
