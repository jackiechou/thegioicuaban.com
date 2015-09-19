using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using CommonLibrary;
using System.Data;
using CommonLibrary.Common.Utilities;

namespace GalleryLibrary
{
    public class GalleryTopic
    {
        private static string IP = NetworkUtils.GetAddress(AddressType.IPv4);

        public static List<Gallery_Topic> GetList(char status)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.CommandTimeout = Settings.CommandTimeout;
                var query = from g in context.Gallery_Topics
                            where g.Status == status
                            select g;
               
                List<Gallery_Topic> lst = query.ToList();
                return lst;
            }
        }

        public static Gallery_Topic GetDetails(int Idx)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.CommandTimeout = Settings.CommandTimeout;
                context.DeferredLoadingEnabled = false;
                return (from g in context.Gallery_Topics
                        where g.Gallery_TopicId == Idx
                        select g).FirstOrDefault();
            }
        }

        public void Delete(int Idx)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.CommandTimeout = Settings.CommandTimeout;
                context.DeferredLoadingEnabled = false;
                Gallery_Topic topic_obj = context.Gallery_Topics.Single(p => p.Gallery_TopicId == Idx);
                context.Gallery_Topics.DeleteOnSubmit(topic_obj);
                context.SubmitChanges();
            }
        }

        public static int? InsertData(Gallery_Topic entity)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                int? o_return = 0;
                string UserLog = Convert.ToString(entity.UserLog);
                var q = context.Gallery_Topics_Insert(UserLog, IP, entity.Gallery_TopicName,
                   entity.Alias, entity.ParentId, entity.Description, entity.Status, ref o_return);
                context.SubmitChanges();
                return o_return;
            }
        }

        public static int? UpdateData(Gallery_Topic entity)
        {
            int? o_return = 0;
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                string UserLog = Convert.ToString(entity.UserLog);
                var q = context.Gallery_Topics_Update(UserLog, IP, entity.Gallery_TopicId, entity.Gallery_TopicName,
                   entity.Alias, entity.ParentId, entity.Description, entity.Status, ref o_return);
                context.SubmitChanges();
                return o_return;
            }
        }

        public void UpdateStatus(string userid, int topicid, char status)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                var query = from x in context.Gallery_Topics
                            where x.Gallery_TopicId == topicid
                            select x;

                foreach (Gallery_Topic topic in query)
                {
                    topic.Status = status;
                    topic.IPLastUpdate = IP;
                    topic.UserLastUpdate = Guid.Parse(userid);
                    topic.LastUpdatedDate = System.DateTime.Now;
                }
                context.SubmitChanges();
            }
        }
    }
}
