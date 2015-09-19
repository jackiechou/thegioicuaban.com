using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Content.Taxonomy
{
    public interface ITermController
    {
        int AddTerm(Term term);
        void AddTermToContent(Term term, ContentItem contentItem);
        void DeleteTerm(Term term);
        Term GetTerm(int termId);
        IQueryable<Term> GetTermsByContent(int contentItemId);
        //Function GetTermsByPortal(ByVal portalId As Integer) As IQueryable(Of Term)
        IQueryable<Term> GetTermsByVocabulary(int vocabularyId);
        void RemoveTermsFromContent(ContentItem contentItem);
        void UpdateTerm(Term term);
    }
}
