using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Search;

namespace CommonLibrary.Entities.Modules
{
    public interface ISearchable
    {
        SearchItemInfoCollection GetSearchItems(ModuleInfo ModInfo);
    }
}
