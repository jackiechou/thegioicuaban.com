using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonLibrary.Entities.Content.Taxonomy;
using CommonLibrary.Data;

namespace CommonLibrary.Entities.Content.Data
{
    public class DataService : IDataService
    {

        #region "Private Members"


        private DataProvider provider = DataProvider.Instance();
        #endregion

        #region "ContentItem Methods"

        public int AddContentItem(ContentItem contentItem, int createdByUserId)
        {
            return provider.ExecuteScalar<int>("AddContentItem", contentItem.Content, contentItem.ContentTypeId, contentItem.TabID, contentItem.ModuleID, contentItem.ContentKey, contentItem.Indexed, createdByUserId);
        }

        public void DeleteContentItem(ContentItem contentItem)
        {
            provider.ExecuteNonQuery("DeleteContentItem", contentItem.ContentItemId);
        }

        public IDataReader GetContentItem(int contentItemId)
        {
            return provider.ExecuteReader("GetContentItem", contentItemId);
        }

        public IDataReader GetContentItemsByTerm(string term)
        {
            return provider.ExecuteReader("GetContentItemsByTerm", term);
        }

        public IDataReader GetUnIndexedContentItems()
        {
            return provider.ExecuteReader("GetUnIndexedContentItems");
        }

        public void UpdateContentItem(ContentItem contentItem, int createdByUserId)
        {
            provider.ExecuteNonQuery("UpdateContentItem", contentItem.ContentItemId, contentItem.Content, contentItem.ContentTypeId, contentItem.TabID, contentItem.ModuleID, contentItem.ContentKey, contentItem.Indexed, createdByUserId);
        }

        #endregion

        #region "MetaData Methods"
        public void AddMetaData(ContentItem contentItem, string name, string value)
        {
            provider.ExecuteNonQuery("AddMetaData", contentItem.ContentItemId, name, value);
        }
        public void DeleteMetaData(ContentItem contentItem, string name, string value)
        {
            provider.ExecuteNonQuery("DeleteMetaData", contentItem.ContentItemId, name, value);
        }
        public IDataReader GetMetaData(int contentItemId)
        {
            return provider.ExecuteReader("GetMetaData", contentItemId);
        }
        #endregion

        #region "ContentType Methods"

        public int AddContentType(ContentType contentType)
        {
            return provider.ExecuteScalar<int>("AddContentType", contentType.Type);
        }

        public void DeleteContentType(ContentType contentType)
        {
            provider.ExecuteNonQuery("DeleteContentType", contentType.ContentTypeId);
        }

        public IDataReader GetContentTypes()
        {
            return provider.ExecuteReader("GetContentTypes");
        }

        public void UpdateContentType(ContentType contentType)
        {
            provider.ExecuteNonQuery("UpdateContentType", contentType.ContentTypeId, contentType.Type);
        }

        #endregion

        #region "ScopeType Methods"

        public int AddScopeType(ScopeType scopeType)
        {
            return provider.ExecuteScalar<int>("AddScopeType", scopeType.Type);
        }

        public void DeleteScopeType(ScopeType scopeType)
        {
            provider.ExecuteNonQuery("DeleteScopeType", scopeType.ScopeTypeId);
        }

        public IDataReader GetScopeTypes()
        {
            return provider.ExecuteReader("GetScopeTypes");
        }

        public void UpdateScopeType(ScopeType scopeType)
        {
            provider.ExecuteNonQuery("UpdateScopeType", scopeType.ScopeTypeId, scopeType.Type);
        }

        #endregion

        #region "Term Methods"

        public int AddHeirarchicalTerm(Term term, int createdByUserId)
        {
            return provider.ExecuteScalar<int>("AddHeirarchicalTerm", term.VocabularyId, term.ParentTermId, term.Name, term.Description, term.Weight, createdByUserId);
        }

        public int AddSimpleTerm(Term term, int createdByUserId)
        {
            return provider.ExecuteScalar<int>("AddSimpleTerm", term.VocabularyId, term.Name, term.Description, term.Weight, createdByUserId);
        }

        public void AddTermToContent(Term term, ContentItem contentItem)
        {
            provider.ExecuteNonQuery("AddTermToContent", term.TermId, contentItem.ContentItemId);
        }

        public void DeleteSimpleTerm(Term term)
        {
            provider.ExecuteNonQuery("DeleteSimpleTerm", term.TermId);
        }

        public void DeleteHeirarchicalTerm(Term term)
        {
            provider.ExecuteNonQuery("DeleteHeirarchicalTerm", term.TermId);
        }

        public IDataReader GetTerm(int termId)
        {
            return provider.ExecuteReader("GetTerm", termId);
        }

        public IDataReader GetTermsByContent(int contentItemId)
        {
            return provider.ExecuteReader("GetTermsByContent", contentItemId);
        }

        public IDataReader GetTermsByVocabulary(int vocabularyId)
        {
            return provider.ExecuteReader("GetTermsByVocabulary", vocabularyId);
        }

        public void RemoveTermsFromContent(ContentItem contentItem)
        {
            provider.ExecuteNonQuery("RemoveTermsFromContent", contentItem.ContentItemId);
        }

        public void UpdateHeirarchicalTerm(Term term, int lastModifiedByUserId)
        {
            provider.ExecuteNonQuery("UpdateHeirarchicalTerm", term.TermId, term.VocabularyId, term.ParentTermId, term.Name, term.Description, term.Weight, lastModifiedByUserId);
        }

        public void UpdateSimpleTerm(Term term, int lastModifiedByUserId)
        {
            provider.ExecuteNonQuery("UpdateSimpleTerm", term.TermId, term.VocabularyId, term.Name, term.Description, term.Weight, lastModifiedByUserId);
        }

        #endregion

        #region "Vocabulary Methods"

        public int AddVocabulary(Vocabulary vocabulary, int createdByUserId)
        {
            return provider.ExecuteScalar<int>("AddVocabulary", vocabulary.Type, vocabulary.Name, vocabulary.Description, vocabulary.Weight, provider.GetNull(vocabulary.ScopeId), vocabulary.ScopeTypeId, createdByUserId);
        }

        public void DeleteVocabulary(Vocabulary vocabulary)
        {
            provider.ExecuteNonQuery("DeleteVocabulary", vocabulary.VocabularyId);
        }

        public IDataReader GetVocabularies()
        {
            return provider.ExecuteReader("GetVocabularies");
        }

        public void UpdateVocabulary(Vocabulary vocabulary, int lastModifiedByUserId)
        {
            provider.ExecuteNonQuery("UpdateVocabulary", vocabulary.VocabularyId, vocabulary.Type, vocabulary.Name, vocabulary.Description, vocabulary.Weight, vocabulary.ScopeId, vocabulary.ScopeTypeId, lastModifiedByUserId);
        }

        #endregion

    }
}
