using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Content
{
    public interface IContentTypeController
    {
        int AddContentType(ContentType contentType);
        void ClearContentTypeCache();
        void DeleteContentType(ContentType contentType);
        IQueryable<ContentType> GetContentTypes();
        void UpdateContentType(ContentType contentType);
    }
}
