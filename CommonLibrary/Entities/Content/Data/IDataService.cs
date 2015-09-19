using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonLibrary.Entities.Content.Taxonomy;

namespace CommonLibrary.Entities.Content.Data
{
    public interface IDataService
    {

        //Content Item Methods
        int AddContentItem(ContentItem contentItem, int createdByUserId);
        void DeleteContentItem(ContentItem contentItem);
        IDataReader GetContentItem(int contentItemId);
        IDataReader GetContentItemsByTerm(string term);
        IDataReader GetUnIndexedContentItems();

        void UpdateContentItem(ContentItem contentItem, int lastModifiedByUserId);
        //Content MetaData Methods
        void AddMetaData(ContentItem contentItem, string name, string value);
        void DeleteMetaData(ContentItem contentItem, string name, string value);
        IDataReader GetMetaData(int contentItemId);
        //ContentType Methods
        int AddContentType(ContentType contentType);
        void DeleteContentType(ContentType contentType);
        IDataReader GetContentTypes();

        void UpdateContentType(ContentType contentType);
        //ScopeType Methods
        int AddScopeType(ScopeType scopeType);
        void DeleteScopeType(ScopeType scopeType);
        IDataReader GetScopeTypes();

        void UpdateScopeType(ScopeType scopeType);
        //Term Methods
        int AddHeirarchicalTerm(Term term, int createdByUserId);
        int AddSimpleTerm(Term term, int createdByUserId);
        void AddTermToContent(Term term, ContentItem contentItem);
        void DeleteSimpleTerm(Term term);
        void DeleteHeirarchicalTerm(Term term);
        IDataReader GetTerm(int termId);
        IDataReader GetTermsByContent(int contentItemId);
        IDataReader GetTermsByVocabulary(int vocabularyId);
        void RemoveTermsFromContent(ContentItem contentItem);
        void UpdateHeirarchicalTerm(Term term, int lastModifiedByUserId);

        void UpdateSimpleTerm(Term term, int lastModifiedByUserId);
        //Vocabulary Methods
        int AddVocabulary(Vocabulary vocabulary, int createdByUserId);
        void DeleteVocabulary(Vocabulary vocabulary);
        IDataReader GetVocabularies();

        void UpdateVocabulary(Vocabulary vocabulary, int lastModifiedByUserId);
    }
}
