using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Validation
{
    public class DomainValidation
    {
        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            //string _path;
            //string _filterAttribute;
            //string servername = "ServerName"; // Give the server Name
            //string domainAndUsername = domain + "\\" + username;

            //DirectoryEntry entry = new DirectoryEntry("LDAP://" + servername, domainAndUsername, pwd);

            //try
            //{
            //    object obj = entry.NativeObject;
            //    DirectorySearcher search = new DirectorySearcher(entry);
            //    search.Filter = "(SAMAccountName=" + username + ")";
            //    search.PropertiesToLoad.Add("cn");

            //    SearchResult result = search.FindOne();

            //    if (result == null)
            //    {
            //        return false;
            //    }

            //    _path = result.Path;
            //    _filterAttribute = ((string)(result.Properties["cn"][0]));
            //}
            //catch (Exception ex) { return false; }
            return true;
        }
    }
}
