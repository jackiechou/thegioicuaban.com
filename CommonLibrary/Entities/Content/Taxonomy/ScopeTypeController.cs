using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Content.Common;
using CommonLibrary.Entities.Content.Data;

namespace CommonLibrary.Entities.Content.Taxonomy
{
    public class ScopeTypeController : IScopeTypeController
    {

        #region "Private Members"


        private IDataService _DataService;
        private string _CacheKey = "ScopeTypes";

        private int _CacheTimeOut = 20;
        #endregion

        #region "Constructors"

        public ScopeTypeController()
            : this(Util.GetDataService())
        {
        }

        public ScopeTypeController(IDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion

        #region "Private Methods"

        private object GetScopeTypesCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillQueryable<ScopeType>(_DataService.GetScopeTypes());
        }

        #endregion

        #region "Public Methods"

        public int AddScopeType(ScopeType scopeType)
        {
            //Argument Contract
            Requires.NotNull("scopeType", scopeType);
            Requires.PropertyNotNullOrEmpty("scopeType", "ScopeType", scopeType.Type);

            scopeType.ScopeTypeId = _DataService.AddScopeType(scopeType);

            //Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey);

            return scopeType.ScopeTypeId;
        }

        public void ClearScopeTypeCache()
        {
            DataCache.RemoveCache(_CacheKey);
        }

        public void DeleteScopeType(ScopeType scopeType)
        {
            //Argument Contract
            Requires.NotNull("scopeType", scopeType);
            Requires.PropertyNotNegative("scopeType", "ScopeTypeId", scopeType.ScopeTypeId);

            _DataService.DeleteScopeType(scopeType);

            //Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey);
        }

        public IQueryable<ScopeType> GetScopeTypes()
        {
            return CBO.GetCachedObject<IQueryable<ScopeType>>(new CacheItemArgs(_CacheKey, _CacheTimeOut), GetScopeTypesCallBack);
        }

        public void UpdateScopeType(ScopeType scopeType)
        {
            //Argument Contract
            Requires.NotNull("scopeType", scopeType);
            Requires.PropertyNotNegative("scopeType", "ScopeTypeId", scopeType.ScopeTypeId);
            Requires.PropertyNotNullOrEmpty("scopeType", "ScopeType", scopeType.Type);

            _DataService.UpdateScopeType(scopeType);

            //Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey);
        }

        #endregion

    }
}
