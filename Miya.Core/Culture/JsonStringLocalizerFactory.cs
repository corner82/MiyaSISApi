﻿using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miya.Core.Culture
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer();
        }
    }
}
