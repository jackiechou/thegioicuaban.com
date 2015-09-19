using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Content.Data;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Content.Common;


namespace CommonLibrary.Entities.Content
{
    public class ContentTypeController : IContentTypeController
    {

        #region "Private Members"


        private IDataService _DataService;
        private string _CacheKey = "ContentTypes";

        private int _CacheTimeOut = 20;
        #endregion

        #region "Constructors"

        public ContentTypeController()
            : this(Util.GetDataService())
        {
        }

        public ContentTypeController(IDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion

        #region "Private Methods"

        private object GetContentTypesCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillQueryable<ContentType>(_DataService.GetContentTypes());
        }

        #endregion

        #region "Public Methods"

        public int AddContentType(ContentType contentType)
        {
            //Argument Contract
            Requires.NotNull("contentType", contentType);
            Requires.PropertyNotNullOrEmpty("contentType", "ContentType", contentType.Type);

            contentType.ContentTypeId = _DataService.AddContentType(contentType);

            //Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey);

            return contentType.ContentTypeId;
        }

        public void ClearContentTypeCache()
        {
            DataCache.RemoveCache(_CacheKey);
        }

        public void DeleteContentType(ContentType contentType)
        {
            //Argument Contract
            Requires.NotNull("contentType", contentType);
            Requires.PropertyNotNegative("contentType", "ContentTypeId", contentType.ContentTypeId);

            _DataService.DeleteContentType(contentType);

            //Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey);
        }

        public IQueryable<ContentType> GetContentTypes()
        {
            return CBO.GetCachedObject<IQueryable<ContentType>>(new CacheItemArgs(_CacheKey, _CacheTimeOut), GetContentTypesCallBack);
        }

        public void UpdateContentType(ContentType contentType)
        {
            //Argument Contract
            Requires.NotNull("contentType", contentType);
            Requires.PropertyNotNegative("contentType", "ContentTypeId", contentType.ContentTypeId);
            Requires.PropertyNotNullOrEmpty("contentType", "ContentType", contentType.Type);

            _DataService.UpdateContentType(contentType);

            //Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey);
        }

        #endregion

    }
}
