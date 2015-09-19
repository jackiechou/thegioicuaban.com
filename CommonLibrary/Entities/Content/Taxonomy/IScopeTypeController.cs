using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Content.Taxonomy
{
    public interface IScopeTypeController
    {
        int AddScopeType(ScopeType scopeType);
        void ClearScopeTypeCache();
        void DeleteScopeType(ScopeType scopeType);
        IQueryable<ScopeType> GetScopeTypes();
        void UpdateScopeType(ScopeType scopeType);
    }
}
