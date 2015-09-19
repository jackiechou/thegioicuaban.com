using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace CommonLibrary.Entities.Content
{
    public interface IContentController
    {
        int AddContentItem(ContentItem contentItem);
        void DeleteContentItem(ContentItem contentItem);
        ContentItem GetContentItem(int contentItemId);
        IQueryable<ContentItem> GetContentItemsByTerm(string term);
        IQueryable<ContentItem> GetUnIndexedContentItems();
        void UpdateContentItem(ContentItem contentItem);
        void AddMetaData(ContentItem contentItem, string name, string value);
        void DeleteMetaData(ContentItem contentItem, string name, string value);
        NameValueCollection GetMetaData(int contentItemId);
    }
}
