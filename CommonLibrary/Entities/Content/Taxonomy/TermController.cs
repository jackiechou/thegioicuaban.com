using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Content.Data;
using CommonLibrary.Entities.Content.Common;
using System.Web.Caching;

namespace CommonLibrary.Entities.Content.Taxonomy
{
    public class TermController : ITermController
    {

        #region "Private Members"

        private string _CacheKey = "Terms_{0}";
        private CacheItemPriority _CachePriority = CacheItemPriority.Normal;

        private int _CacheTimeOut = 20;

        private IDataService _DataService;
        #endregion

        #region "Constructors"

        public TermController()
            : this(Util.GetDataService())
        {
        }

        public TermController(IDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion

        #region "Private Methods"

        private object GetTermsCallBack(CacheItemArgs cacheItemArgs)
        {
            int vocabularyId = (int)cacheItemArgs.ParamList[0];
            return CBO.FillQueryable<Term>(_DataService.GetTermsByVocabulary(vocabularyId));
        }

        #endregion

        #region "Public Methods"

        public int AddTerm(Term term)
        {
            //Argument Contract
            Requires.NotNull("term", term);
            Requires.PropertyNotNegative("term", "VocabularyId", term.VocabularyId);
            Requires.PropertyNotNullOrEmpty("term", "Name", term.Name);

            //if ((term.IsHeirarchical))
            //{
            //    term.TermId = _DataService.AddHeirarchicalTerm(term, UserController.GetCurrentUserInfo().UserID);
            //}
            //else
            //{
            //    term.TermId = _DataService.AddSimpleTerm(term, UserController.GetCurrentUserInfo().UserID);
            //}

            //Clear Cache
            DataCache.RemoveCache(string.Format(_CacheKey, term.VocabularyId));

            return term.TermId;
        }

        public void AddTermToContent(Term term, ContentItem contentItem)
        {
            //Argument Contract
            Requires.NotNull("term", term);
            Requires.NotNull("contentItem", contentItem);

            _DataService.AddTermToContent(term, contentItem);
        }

        public void DeleteTerm(Term term)
        {
            //Argument Contract
            Requires.NotNull("term", term);
            Requires.PropertyNotNegative("term", "TermId", term.TermId);

            //if ((term.IsHeirarchical))
            //{
            //    _DataService.DeleteHeirarchicalTerm(term);
            //}
            //else
            //{
            //    _DataService.DeleteSimpleTerm(term);
            //}

            //Clear Cache
            DataCache.RemoveCache(string.Format(_CacheKey, term.VocabularyId));
        }

        public Term GetTerm(int termId)
        {
            //Argument Contract
            Requires.NotNegative("termId", termId);

            return CBO.FillObject<Term>(_DataService.GetTerm(termId));
        }

        public IQueryable<Term> GetTermsByContent(int contentItemId)
        {
            //Argument Contract
            Requires.NotNegative("contentItemId", contentItemId);

            return CBO.FillQueryable<Term>(_DataService.GetTermsByContent(contentItemId));
        }

        public IQueryable<Term> GetTermsByVocabulary(int vocabularyId)
        {
            //Argument Contract
            Requires.NotNegative("vocabularyId", vocabularyId);

            return CBO.GetCachedObject<IQueryable<Term>>(new CacheItemArgs(string.Format(_CacheKey, vocabularyId), _CacheTimeOut, _CachePriority, vocabularyId), GetTermsCallBack);
        }

        public void RemoveTermsFromContent(ContentItem contentItem)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);

            _DataService.RemoveTermsFromContent(contentItem);
        }

        public void UpdateTerm(Term term)
        {
            //Argument Contract
            Requires.NotNull("term", term);
            Requires.PropertyNotNegative("term", "TermId", term.TermId);
            Requires.PropertyNotNegative("term", "Vocabulary.VocabularyId", term.VocabularyId);
            Requires.PropertyNotNullOrEmpty("term", "Name", term.Name);

            //if ((term.IsHeirarchical))
            //{
            //    //Requires.PropertyNotNull("term", "ParentTermId", term.ParentTermId)
            //    _DataService.UpdateHeirarchicalTerm(term, UserController.GetCurrentUserInfo().UserID);
            //}
            //else
            //{
            //    _DataService.UpdateSimpleTerm(term, UserController.GetCurrentUserInfo().UserID);
            //}

            //Clear Cache
            DataCache.RemoveCache(string.Format(_CacheKey, term.VocabularyId));
        }

        #endregion

    }
}
