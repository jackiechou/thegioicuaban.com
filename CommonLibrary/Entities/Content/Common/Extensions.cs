using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Content.Taxonomy;
using System.Collections.Specialized;
using CommonLibrary.Common.Utilities;



namespace CommonLibrary.Entities.Content.Common
{
    public static class Extensions
    {

        #region "Term Extensions"

        static internal List<Term> GetChildTerms(this Term Term, int termId, int vocabularyId)
        {
            ITermController ctl = CommonLibrary.Entities.Content.Common.Util.GetTermController();

            IQueryable<Term> terms = from term in ctl.GetTermsByVocabulary(vocabularyId)
                                     where term.ParentTermId == termId
                                     select term;

            return terms.ToList();
        }

        static internal Vocabulary GetVocabulary(this Term term, int vocabularyId)
        {
            IVocabularyController ctl = CommonLibrary.Entities.Content.Common.Util.GetVocabularyController();

            return (from v in ctl.GetVocabularies()
                    where v.VocabularyId == vocabularyId
                    select v)
                    .SingleOrDefault();
        }

        public static string ToDelimittedString(this List<Term> terms, string delimitter)
        {
            StringBuilder sb = new StringBuilder();
            if (terms != null)
            {
                foreach (Term _Term in (from term in terms orderby term.Name ascending select term))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(delimitter);
                    }
                    sb.Append(_Term.Name);
                }
            }
            return sb.ToString();
        }

        public static string ToDelimittedString(this List<Term> terms, string format, string delimitter)
        {
            StringBuilder sb = new StringBuilder();
            if (terms != null)
            {
                foreach (Term _Term in (from term in terms orderby term.Name ascending select term))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(delimitter);
                    }
                    sb.Append(string.Format(format, _Term.Name));
                }
            }
            return sb.ToString();
        }

        #endregion

        #region "Vocabulary Extensions"

        static internal ScopeType GetScopeType(this Vocabulary voc, int scopeTypeId)
        {
            IScopeTypeController ctl = CommonLibrary.Entities.Content.Common.Util.GetScopeTypeController();

            return ctl.GetScopeTypes().Where(s => s.ScopeTypeId == scopeTypeId).SingleOrDefault();
        }

        static internal List<Term> GetTerms(this Vocabulary voc, int vocabularyId)
        {
            ITermController ctl = CommonLibrary.Entities.Content.Common.Util.GetTermController();

            return ctl.GetTermsByVocabulary(vocabularyId).ToList();
        }

        #endregion

        #region "ContentItem Extensions"
        internal static NameValueCollection GetMetaData(this ContentItem item, int contentItemId)
        {
            IContentController ctl = Util.GetContentController();

            NameValueCollection _MetaData;
            if (contentItemId == Null.NullInteger)
            {
                _MetaData = new NameValueCollection();
            }
            else
            {
                _MetaData = ctl.GetMetaData(contentItemId);
            }

            return _MetaData;
        }
        internal static List<Term> GetTerms(this ContentItem item, int contentItemId)
        {
            ITermController ctl = CommonLibrary.Entities.Content.Common.Util.GetTermController();

            List<Term> _Terms = null;
            if (contentItemId == Null.NullInteger)
            {
                _Terms = new List<Term>();
            }
            else
            {
                _Terms = ctl.GetTermsByContent(contentItemId).ToList();
            }

            return _Terms;
        }
        #endregion
    }
}
