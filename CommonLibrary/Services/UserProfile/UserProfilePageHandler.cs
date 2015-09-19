using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Users;
using System.Web;
using CommonLibrary.Common;
using CommonLibrary.Entities.Portal;

namespace CommonLibrary.Services.UserProfile
{
    public class UserProfilePageHandler : IHttpHandler
    {

        private static int GetUserId(string username, int PortalId)
        {
            int _UserId = Null.NullInteger;
            UserInfo userInfo = UserController.GetUserByName(PortalId, username);
            if (userInfo != null)
            {
                _UserId = userInfo.UserID;
            }
            else
            {
                //The user cannot be found (potential DOS)
                throw new HttpException(404, "Not Found");
            }
            return _UserId;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This handler handles requests for LinkClick.aspx, but only those specifc
        /// to file serving
        /// </summary>
        /// <param name="context">System.Web.HttpContext)</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cpaterra]	4/19/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void ProcessRequest(System.Web.HttpContext context)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            int UserId = Null.NullInteger;
            int PortalId = _portalSettings.PortalId;

            try
            {
                //try UserId
                if (!string.IsNullOrEmpty(context.Request.QueryString["UserId"]))
                {
                    UserId = Int32.Parse(context.Request.QueryString["UserId"]);
                    if (UserController.GetUserById(PortalId, UserId) == null)
                    {
                        //The user cannot be found (potential DOS)
                        throw new HttpException(404, "Not Found");
                    }
                }

                if (UserId == Null.NullInteger)
                {
                    //try userName
                    if (!string.IsNullOrEmpty(context.Request.QueryString["UserName"]))
                    {
                        UserId = GetUserId(context.Request.QueryString["UserName"], PortalId);
                    }
                }

                if (UserId == Null.NullInteger)
                {
                    //try user
                    string user = context.Request.QueryString["User"];
                    if (!string.IsNullOrEmpty(user))
                    {
                        if (!Int32.TryParse(user, out UserId))
                        {
                            //User is not an integer, so try it as a name
                            UserId = GetUserId(user, PortalId);
                        }
                        else
                        {
                            if (UserController.GetUserById(PortalId, UserId) == null)
                            {
                                //The user cannot be found (potential DOS)
                                throw new HttpException(404, "Not Found");
                            }
                        }
                    }
                }

                if (UserId == Null.NullInteger)
                {
                    //The user cannot be found (potential DOS)
                    throw new HttpException(404, "Not Found");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                //The user cannot be found (potential DOS)
                throw new HttpException(404, "Not Found");
            }

            //Redirect to Userprofile Page
            context.Response.Redirect(Globals.UserProfileURL(UserId), true);

        }

        public bool IsReusable
        {
            get { return true; }
        }

    }
}
