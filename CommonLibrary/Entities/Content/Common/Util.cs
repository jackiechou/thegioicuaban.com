using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Content.Taxonomy;
using System.Collections.Specialized;
using CommonLibrary.Entities.Content.Data;
using CommonLibrary.ComponentModel;

namespace CommonLibrary.Entities.Content.Common
{
    public static class Util
    {

        public static IDataService GetDataService()
        {
            IDataService ds = ComponentFactory.GetComponent<IDataService>();

            if (ds == null)
            {
                ds = new DataService();
                ComponentFactory.RegisterComponentInstance<IDataService>(ds);
            }
            return ds;
        }

        public static IContentController GetContentController()
        {
            IContentController ctl = ComponentFactory.GetComponent<IContentController>();

            if (ctl == null)
            {
                ctl = new ContentController();
                ComponentFactory.RegisterComponentInstance<IContentController>(ctl);
            }
            return ctl;
        }

        public static IScopeTypeController GetScopeTypeController()
        {
            IScopeTypeController ctl = ComponentFactory.GetComponent<IScopeTypeController>();

            if (ctl == null)
            {
                ctl = new ScopeTypeController();
                ComponentFactory.RegisterComponentInstance<IScopeTypeController>(ctl);
            }
            return ctl;

        }

        public static ITermController GetTermController()
        {
            ITermController ctl = ComponentFactory.GetComponent<ITermController>();

            if (ctl == null)
            {
                ctl = new TermController();
                ComponentFactory.RegisterComponentInstance<ITermController>(ctl);
            }
            return ctl;

        }

        public static IVocabularyController GetVocabularyController()
        {
            IVocabularyController ctl = ComponentFactory.GetComponent<IVocabularyController>();

            if (ctl == null)
            {
                ctl = new VocabularyController();
                ComponentFactory.RegisterComponentInstance<IVocabularyController>(ctl);
            }
            return ctl;

        }

    }
}
