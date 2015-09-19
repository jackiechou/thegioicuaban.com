using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Content.Data;
using CommonLibrary.Entities.Content.Common;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Entities.Content.Taxonomy
{
    public class VocabularyController : IVocabularyController
    {

        #region "Private Members"

        private string _CacheKey = "Vocabularies";
        private int _CacheTimeOut = 20;

        private IDataService _DataService;
        #endregion

        #region "Constructors"

        public VocabularyController()
            : this(Util.GetDataService())
        {
        }

        public VocabularyController(IDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion

        #region "Private Methods"

        private object GetVocabulariesCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillQueryable<Vocabulary>(_DataService.GetVocabularies());
        }

        #endregion

        #region "Public Methods"

        public int AddVocabulary(Vocabulary vocabulary)
        {
            //Argument Contract
            Requires.NotNull("vocabulary", vocabulary);
            Requires.PropertyNotNullOrEmpty("vocabulary", "Name", vocabulary.Name);
            Requires.PropertyNotNegative("vocabulary", "ScopeTypeId", vocabulary.ScopeTypeId);

            vocabulary.VocabularyId = _DataService.AddVocabulary(vocabulary, UserController.GetCurrentUserInfo().UserID);

            //Refresh Cache
            DataCache.RemoveCache(_CacheKey);

            return vocabulary.VocabularyId;
        }

        public void ClearVocabularyCache()
        {
            DataCache.RemoveCache(_CacheKey);
        }

        public void DeleteVocabulary(Vocabulary vocabulary)
        {
            //Argument Contract
            Requires.NotNull("vocabulary", vocabulary);
            Requires.PropertyNotNegative("vocabulary", "VocabularyId", vocabulary.VocabularyId);

            _DataService.DeleteVocabulary(vocabulary);

            //Refresh Cache
            DataCache.RemoveCache(_CacheKey);
        }

        public IQueryable<Vocabulary> GetVocabularies()
        {
            return CBO.GetCachedObject<IQueryable<Vocabulary>>(new CacheItemArgs(_CacheKey, _CacheTimeOut), GetVocabulariesCallBack);
        }

        public void UpdateVocabulary(Vocabulary vocabulary)
        {
            //Argument Contract
            Requires.NotNull("vocabulary", vocabulary);
            Requires.PropertyNotNegative("vocabulary", "VocabularyId", vocabulary.VocabularyId);
            Requires.PropertyNotNullOrEmpty("vocabulary", "Name", vocabulary.Name);

            //Refresh Cache
            DataCache.RemoveCache(_CacheKey);

            _DataService.UpdateVocabulary(vocabulary, UserController.GetCurrentUserInfo().UserID);
        }

        #endregion

    }

}
