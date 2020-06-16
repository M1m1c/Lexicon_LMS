using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Services
{
    public interface ILookupService
    {
        List<SelectListItem> GetLevels();
    }
}
