using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Content.Taxonomy
{
    public interface IVocabularyController
    {
        int AddVocabulary(Vocabulary vocabulary);
        void ClearVocabularyCache();
        void DeleteVocabulary(Vocabulary vocabulary);
        IQueryable<Vocabulary> GetVocabularies();
        void UpdateVocabulary(Vocabulary vocabulary);
    }
}
