using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Net;
using System.Drawing;

namespace CommonLibrary.Modules
{
    public class ModuleClass : System.Web.UI.Page
    {       
        public ModuleClass(){}

        public static string GeneratePageTitle(string title,
                                       string rootTitle,
                                       string pathSeparator)
        {

            List<string> path = new List<string>();
            SiteMapNode currentNode = null;

            currentNode = SiteMap.CurrentNode;

            if (currentNode == null)
            {
                if (String.IsNullOrEmpty(title))
                {
                    return rootTitle;
                }

                return title + pathSeparator + rootTitle;
            }

            if (currentNode != SiteMap.RootNode)
            {
                if (!String.IsNullOrEmpty(title) && title != rootTitle)
                {
                    path.Add(title);
                }
                else
                {
                    path.Add(currentNode.Title);
                }
            }
            else
            {
                path.Add(rootTitle);
            }

            currentNode = currentNode.ParentNode;

            while (!(currentNode == null))
            {

                // Use our own root title for the <title> tag
                if (currentNode.Title != SiteMap.RootNode.Title)
                {
                    path.Add(currentNode.Title);
                }
                else
                {
                    path.Add(rootTitle);
                }

                currentNode = currentNode.ParentNode;
            }

            string[] paths = path.ToArray();

            return string.Join(pathSeparator, paths);

        }

        public bool CheckInput(String s)
        {
            string inWord = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_";
            for (int i = 0; i < s.Length; i++)
            {
                if (inWord.IndexOf(s.Substring(i, 1)) < 0)
                {
                    return false;
                }
            }

            if (s.IndexOf(" insert ") != -1 || s.IndexOf(" update ") != -1 || s.IndexOf(" delete ") != -1 || s.IndexOf(" shutdown ") != -1 || s.IndexOf(" select ") != -1 || s.IndexOf(" and ") != -1 || s.IndexOf(" execute ") != -1 || s.IndexOf(" exec ") != -1 || s.IndexOf(" union ") != -1 || s.IndexOf(" or ") != -1 || s.IndexOf(" drop ") != -1)
                return false;
            return true;
        }

      

        //Load XML File to DataList
        public void LoadXml2DataList(DataList datalist, string xmlPath)
        {
            string strXmlPath = HttpContext.Current.Server.MapPath(xmlPath);
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(strXmlPath);
                datalist.DataSource = ds;
                datalist.DataBind();
            }
            catch (Exception ex) { ex.ToString(); }

        }

        //Load XML File to Gridview        
        public void LoadXml2GridView(GridView gridView, string xmlPath)
        {
            string strXmlPath = HttpContext.Current.Server.MapPath(xmlPath);

            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(xmlPath);
                gridView.DataSource = ds;
                gridView.DataBind();
            }
            catch (Exception ex) { ex.ToString(); }
        }

        public static System.Web.UI.Control CreateMasterUserControl(System.Web.UI.Page mainPage, string strMasterPageUserControl)
        {
            System.Web.UI.Control userControl = null;
            try
            {
                string userControlPath = System.Configuration.ConfigurationManager.AppSettings[strMasterPageUserControl];
                userControl = mainPage.LoadControl(userControlPath);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return userControl;
        }

        public System.Web.UI.UserControl LoadUserControl(string urlUserControl)
        {
            System.Web.UI.UserControl userControl = null;
            try
            {
                userControl = (System.Web.UI.UserControl)Page.LoadControl(urlUserControl);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return userControl;
        }

        public void loadUserControlToPlaceHolder(PlaceHolder myPlaceHolder, string strUserControlPath)
        {
            try
            {
                //Add div 
                HtmlGenericControl gel = new HtmlGenericControl("div");
                //Assign usercontrol on web page
                if (strUserControlPath != "")
                {
                    Control ct = Page.LoadControl(strUserControlPath);
                    //add user control to div
                    gel.Controls.Add(ct);
                    //add div to placeholder                  
                    myPlaceHolder.Controls.Add(gel);
                }
            }
            catch (HttpException ex) { ex.ToString(); }
        }

        public void AddJScript(string src_path)
        {
            string src = Page.ResolveUrl(src_path);
            HtmlGenericControl genCtrl = new HtmlGenericControl();
            genCtrl.TagName = "script";
            genCtrl.Attributes.Add("type", "text/javascript");
            genCtrl.Attributes.Add("language", "javascript");
            genCtrl.Attributes.Add("src", src);
            Page.Header.Controls.Add(genCtrl);
        }
        
        public string Truncate(string input, int characterLimit)
        {
            string output = input;

            // Check if the string is longer than the allowed amount otherwise do nothing
            if (output.Length > characterLimit && characterLimit > 0)
            {
                // cut the string down to the maximum number of characters
                output = output.Substring(0, characterLimit);

                // Check if the character right after the truncate point was a space
                // if not, we are in the middle of a word and need to remove the rest of it
                if (input.Substring(output.Length, 1) != " ")
                {
                    int LastSpace = output.LastIndexOf(" ");

                    // if we found a space then, cut back to that space
                    if (LastSpace != -1)
                    {
                        output = output.Substring(0, LastSpace);
                    }
                }
                // Finally, add the "..."
                output += "...";
            }
            return output;
        }

        public string getRandom(int min, int max)
        {
            Random random = new Random();
            int result = random.Next(min, max);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append(result);
            return builder.ToString();
        }

        public string GetRandomString()
        {
            //use the following string to control your set of alphabetic characters to choose from
            //for example, you could include uppercase too
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";

            // Random is not truly random, 
            // so we try to encourage better randomness by always changing the seed value
            Random rnd = new Random(DateTime.Now.Millisecond);

            // basic 5 digit random number
            string result = rnd.Next(10000, 99999).ToString();

            // single random character in ascii range a-z
            string alphaChar = alphabet.Substring(rnd.Next(0, alphabet.Length - 1), 1);

            // random position to put the alpha character
            int replacementIndex = rnd.Next(0, (result.Length - 1));
            result = result.Remove(replacementIndex, 1).Insert(replacementIndex, alphaChar);

            return result;
        }

        public static string GetRandomString(int length)
        {
            int intZero = 0;
            int intNine = 0;
            int intA = 0;
            int intZ = 0;
            int intCount = 0;
            int intRandomNumber = 0;
            string strRandomString = null;
            Random rRandom = new Random(DateTime.Now.Millisecond);

            intZero = Convert.ToInt32('0');
            intNine = Convert.ToInt32('9');

            intA = Convert.ToInt32('A');
            intZ = Convert.ToInt32('Z');
            strRandomString = "";

            while (intCount < length)
            {
                intRandomNumber = rRandom.Next(intZero, intZ);
                if (((intRandomNumber >= intZero) & (intRandomNumber <= intNine)) | ((intRandomNumber >= intA) & (intRandomNumber <= intZ)))
                {
                    strRandomString = strRandomString + ((char)(intRandomNumber)).ToString();
                }
                else
                {
                    strRandomString = strRandomString + ((char)(rRandom.Next(intZero, intNine))).ToString();
                }
                intCount = intCount + 1;
            }

            return strRandomString;

        }

        protected int GetRandomInt(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(min, max);
        }

        public string GetUniqueKey()
        {
            int maxSize  = 8 ;
            //int minSize = 5 ;
            char[] chars = new char[62];
            string a;
            a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();
            int size  = maxSize ;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider  crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data) ;
            size =  maxSize ;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size) ;
            foreach(byte b in data)
            { 
                result.Append(chars[b % (chars.Length - 1)]);               
            }
            return result.ToString();
        }

        public static Size sizeToBounds(int current_width, int current_height, int new_width, int new_height)
        {
            double width_ratio, height_ratio;

            // Scales image to bounds
            width_ratio = (double)new_width / (double)current_width;
            height_ratio = (double)new_height / (double)current_height;

            // First width
            if (current_width != 0 && new_width > current_width)
            {
                new_width = current_width;
                new_height = (int)(new_width / width_ratio);
            }

            // Then, height
            if (current_height != 0 && new_width > current_height)
            {
                new_height = current_height;
                new_width = (int)(new_width / height_ratio);
            }

            return new Size(new_width, new_height);

            //Size NewSize = sizeToBounds(200, 200, 230, 435);
            //Response.Write("w = " + NewSize.Width + ", h = " + NewSize.Height);
            // outputs :  w = 91, h = 200
        }


        public void deleteFile(string file_name, string dir_path)
        {
            if (file_name != null)
            {
                bool exists = System.IO.Directory.Exists(dir_path);
                if (exists == true)
                {
                    string file_path = System.IO.Path.Combine(dir_path, file_name);

                    if (System.IO.File.Exists(file_path))
                    {
                        try
                        {
                            System.IO.File.Delete(file_path);
                        }
                        catch (IOException e)
                        {
                            e.ToString();
                        }
                    }
                }
                else
                {
                    string scriptText = "<script type='text/javascript'>alert('There is no directory like this path')</script>";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showError", scriptText, false);
                }
            }            
        }

         /*** Strips accents off words = Loai bo dau trong tieng viet*/
        public string StripDiacritics(string accented)
        {
            string result=null;
            if(accented!=null){ 
                string pattern =@"\\pp{IsCombiningDiacriticalMarks}+";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                string strFormD= accented.Normalize(System.Text.NormalizationForm.FormD);
                result=regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            }
            return result;
        }

        public string ConvertToUnSign(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');

            sb = sb.Replace('á', 'a');
            sb = sb.Replace('à', 'a');
            sb = sb.Replace('ả', 'a');
            sb = sb.Replace('ã', 'a');
            sb = sb.Replace('ạ', 'a');

            sb = sb.Replace('ă', 'a');
            sb = sb.Replace('ắ', 'a');
            sb = sb.Replace('ằ', 'a');
            sb = sb.Replace('ẳ', 'a');
            sb = sb.Replace('ẵ', 'a');
            sb = sb.Replace('ặ', 'a');
            
            sb = sb.Replace('é', 'e');
            sb = sb.Replace('è', 'e');
            sb = sb.Replace('ẻ', 'e');
            sb = sb.Replace('ẽ', 'e');
            sb = sb.Replace('ẹ', 'e');

            sb = sb.Replace('ê', 'e');
            sb = sb.Replace('ế', 'e');
            sb = sb.Replace('ề', 'e');
            sb = sb.Replace('ể', 'e');
            sb = sb.Replace('ễ', 'e');
            sb = sb.Replace('ệ', 'e');


            sb = sb.Replace('í', 'i');
            sb = sb.Replace('ì', 'i');
            sb = sb.Replace('ỉ', 'i');
            sb = sb.Replace('ĩ', 'i');
            sb = sb.Replace('ị', 'i');

            sb = sb.Replace('ó', 'o');
            sb = sb.Replace('ò', 'o');
            sb = sb.Replace('ỏ', 'o');
            sb = sb.Replace('õ', 'o');
            sb = sb.Replace('ọ', 'o');

            sb = sb.Replace('ô', 'o');
            sb = sb.Replace('ố', 'o');
            sb = sb.Replace('ồ', 'o');
            sb = sb.Replace('ổ', 'o');
            sb = sb.Replace('ỗ', 'o');
            sb = sb.Replace('ộ', 'o');

            sb = sb.Replace('ú', 'u');
            sb = sb.Replace('ù', 'u');
            sb = sb.Replace('ủ', 'u');
            sb = sb.Replace('ũ', 'u');
            sb = sb.Replace('ụ', 'u');

            sb = sb.Replace('ý', 'y');
            sb = sb.Replace('ỳ', 'y');
            sb = sb.Replace('ỷ', 'y');
            sb = sb.Replace('ỹ', 'y');
            sb = sb.Replace('ỵ', 'y');

            //Capital letter
            sb = sb.Replace('Á', 'A');
            sb = sb.Replace('À', 'A');
            sb = sb.Replace('Ả', 'A');
            sb = sb.Replace('Ã', 'A');
            sb = sb.Replace('Ạ', 'A');

            sb = sb.Replace('Ă', 'A');
            sb = sb.Replace('Ắ', 'A');
            sb = sb.Replace('Ằ', 'A');
            sb = sb.Replace('Ẳ', 'A');
            sb = sb.Replace('Ẵ', 'A');
            sb = sb.Replace('Ặ', 'A');

            sb = sb.Replace('É', 'E');
            sb = sb.Replace('È', 'E');
            sb = sb.Replace('Ẻ', 'E');
            sb = sb.Replace('Ẽ', 'E');
            sb = sb.Replace('Ẹ', 'E');

            sb = sb.Replace('Ê', 'E');
            sb = sb.Replace('Ế', 'E');
            sb = sb.Replace('Ề', 'E');
            sb = sb.Replace('Ể', 'E');
            sb = sb.Replace('Ễ', 'E');
            sb = sb.Replace('Ệ', 'E');

            sb = sb.Replace('Í', 'I');
            sb = sb.Replace('Ì', 'I');
            sb = sb.Replace('Ỉ', 'I');
            sb = sb.Replace('Ĩ', 'I');
            sb = sb.Replace('Ị', 'I');

            sb = sb.Replace('Ó', 'O');
            sb = sb.Replace('Ò', 'O');
            sb = sb.Replace('Ỏ', 'O');
            sb = sb.Replace('Õ', 'O');
            sb = sb.Replace('Ọ', 'O');

            sb = sb.Replace('Ô', 'O');
            sb = sb.Replace('Ố', 'O');
            sb = sb.Replace('Ồ', 'O');
            sb = sb.Replace('Ổ', 'O');
            sb = sb.Replace('Ỗ', 'O');
            sb = sb.Replace('Ộ', 'O');


            sb = sb.Replace('Ú', 'U');
            sb = sb.Replace('Ù', 'U');
            sb = sb.Replace('Ủ', 'U');
            sb = sb.Replace('Ũ', 'U');
            sb = sb.Replace('Ụ', 'U');

            sb = sb.Replace('Ý', 'Y');
            sb = sb.Replace('Ỳ', 'Y');
            sb = sb.Replace('Ỷ', 'Y');
            sb = sb.Replace('Ỹ', 'Y');
            sb = sb.Replace('Ỵ', 'Y');

            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }

       
         /*** Remove white space*/
        public string RemoveWhiteSpace(string strIput)
        {
            string result=null;
            if(strIput!=null){    
                string pattern =@"\s+";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);                      
                result = regex.Replace(strIput, String.Empty);
            }
            return result;         
        }        

        public String ReverseString(String InString)
        {
            // Check null String
            if (InString == null) return null;

            Int32 intSize = InString.Length;
            char[] arrayInString = InString.ToCharArray();
            char[] arrayOutString = new char[intSize];

            for (Int32 i = 0; i < intSize; ++i)
                arrayOutString[i] = arrayInString[intSize - i - 1];

            return new String(arrayOutString);
        }        

        public static string GenerateFriendlyString(string phrase, int maxLength = 50)
        {
            string str = RemoveSign4VietnameseString(phrase.ToLower());
    
            // invalid chars, make into spaces
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");  
            // convert multiple spaces/hyphens into one space       
            str = Regex.Replace(str, @"[\s-]+", " ").Trim(); 
            // cut and trim it
            str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim(); 
            // hyphens
            str = Regex.Replace(str, @"\s", "-"); 

            return str;

            //string title = @"A bunch of ()/*++\'#@$&*^!%     invalid URL characters  ";
            //title.Slugify();
            // outputs : a-bunch-of-invalid-url-characters
        }



        public static string CutLimitText(string source, int limit)
        {
            if (source.Length < limit)
            {
                return source;
            }
            else
            {
                int endtext2 = 0;
                endtext2 = source.IndexOf(" ", limit);

                if (endtext2 > 0)
                {
                    source = source.Substring(0, endtext2);
                }
                else
                {
                    source = source.ToString() + " ...";
                }
                return source;
            }
        }

        public static string removeHTMLtab(string content, string replace)
        {
            string strSource = Regex.Replace(content, @"<(.|\n)*?>", replace);
            strSource = Regex.Replace(strSource, "\r|\t|\n", replace);
            return strSource;
        }

        public static string cutSubString(string content, int num)
        {
            content = removeHTMLtab(content, "");
            content = CutLimitText(content, num);            
            return content;
        }

        public string createTags(string tags)
        {            
            System.Text.StringBuilder sb = new System.Text.StringBuilder(tags.ToLower());            
            sb.Replace("\"", " ");
            sb.Replace(",", " ");
            sb.Replace("&amp", " ");
            sb.Replace("&", " ");
            string result= UTF8_Encode(removeHTMLtab(sb.ToString(), ""));
            return result; 
        }

        public static string convertTitle2Link(string title)
        {
            string result = string.Empty;
            string strSource = removeHTMLtab(title, "");

            System.Text.StringBuilder sb = new System.Text.StringBuilder(strSource.ToLower().TrimStart());
            sb.Replace("-", "");
            sb.Replace(' ', '-');
            sb.Replace(":", "");
            sb.Replace("\"", "");
            sb.Replace("%", "");
            sb.Replace("?", "");            
            sb.Replace("&amp", "-");            
            sb.Replace("&", "-");
            sb.Replace("--", "-");
            result = RemoveSign4VietnameseString(sb.ToString());
            return result;
        }

        public static string convertName2Link(string title)
        {
            string result = string.Empty;
            string strSource = removeHTMLtab(title, "");
            System.Text.StringBuilder sb = new System.Text.StringBuilder(strSource.ToLower());
            sb.Replace(' ', '_');
            sb.Replace("\"", "");
            sb.Replace(",", "_");
            sb.Replace("&amp", "_");
            sb.Replace("&", "_");
            result = RemoveSign4VietnameseString(sb.ToString());
            return result;
        }

        public static string RemoveSign4VietnameseString(string str)
        {
            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi        
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        public static readonly string[] VietnameseSigns = new string[]    
        {   "aAeEoOuUiIdDyY",        
            "áàạảãâấầậẩẫăắằặẳẵ",        
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",        
            "éèẹẻẽêếềệểễ",        
            "ÉÈẸẺẼÊẾỀỆỂỄ",        
            "óòọỏõôốồộổỗơớờợởỡ",        
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",        
            "úùụủũưứừựửữ",        
            "ÚÙỤỦŨƯỨỪỰỬỮ",        
            "íìịỉĩ",        
            "ÍÌỊỈĨ",        
            "đ",        
            "Đ",        
            "ýỳỵỷỹ",        
            "ÝỲỴỶỸ",            
        };

        public static string RandomString(int size)
        {
            string strCaptcha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string strTemp = "";
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                int iRan = random.Next(0, 61);
                strTemp += strCaptcha.Substring(iRan, 1);
            }
            return strTemp;
        }

        public string GetImageDimensions(string fileNameAndPath)
        {
            string size = null;
            if (fileNameAndPath!=null)
            {
                System.Drawing.Image theImage = System.Drawing.Image.FromFile(fileNameAndPath);
                float imageWidth = theImage.PhysicalDimension.Width;
                float imageHeight = theImage.PhysicalDimension.Height;

                size=imageWidth.ToString() + "," + imageHeight.ToString();               
            }
            return size;
        }

        public string ResizeImageDimension(string file_path, int num_scale)
        {
            string img_scale_dimension=null;

            if (file_path != null)
            {
                if (File.Exists(file_path))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(file_path);
                    float imgWidth = img.PhysicalDimension.Width;
                    float imgHeight = img.PhysicalDimension.Height;                                
                    //Scale 1:1
                    float imgSize = imgHeight > imgWidth ? imgHeight : imgWidth;
                    float imgResize = imgSize <= num_scale ? (float)1.0 : num_scale / imgSize;
                    imgWidth *= imgResize; imgHeight *= imgResize;

                    img_scale_dimension = imgWidth.ToString() + "," + imgHeight.ToString();        
                }               
            }
            return img_scale_dimension;
        }

        public string SmartResizeImageAndUpload(FileUpload FileUpload1, string dir_path, double maxWidth, double maxHeight)
        {
            string file_name = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.PostedFile.FileName);
            string file_ext = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower().Trim();
            string file_name_ext = file_name + file_ext;
            string encoded_image = GetEncodeString(file_name) + file_ext;

            try
            {
                // Declare variable for the conversion
                float ratio;

                // Create variable to hold the image            
                string filepath = dir_path + "/" + encoded_image;
                FileUpload1.SaveAs(Server.MapPath(filepath));
                System.Drawing.Image myimage = System.Drawing.Image.FromFile(Server.MapPath(filepath));

                // Get height and width of current image
                int width = (int)myimage.Width;
                int height = (int)myimage.Height;

                // Ratio and conversion for new size
                if (width > maxWidth)
                {
                    ratio = (float)width / (float)maxWidth;
                    width = (int)(width / ratio);
                    height = (int)(height / ratio);
                }

                // Ratio and conversion for new size
                if (height > maxHeight)
                {
                    ratio = (float)height / (float)maxHeight;
                    height = (int)(height / ratio);
                    width = (int)(width / ratio);
                }



                System.Drawing.Bitmap source_bitmap = new System.Drawing.Bitmap(myimage);
                myimage.Dispose();

                System.Drawing.Bitmap thumb_bitmap = new System.Drawing.Bitmap(width, height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(thumb_bitmap);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height);
                g.DrawImage(source_bitmap, 0, 0, width, height);
                g.Dispose();

                System.Drawing.Imaging.ImageCodecInfo[] Info = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                System.Drawing.Imaging.EncoderParameters Params = new System.Drawing.Imaging.EncoderParameters(1);
                Params.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                thumb_bitmap.Save(Server.MapPath(filepath), Info[1], Params);

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return encoded_image;
        }

        public string ResizeImageAndUpload(FileUpload FileUpload1, string dir_path, int height, int width)
        {

            string file_name = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.PostedFile.FileName);
            string file_ext = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower().Trim();
            string file_name_ext = file_name + file_ext;
            string encoded_image = GetEncodeString(file_name) + file_ext;
            try
            {
                //string filename = FileUpload1.FileName;
                string filepath = dir_path + "/" + encoded_image;
                FileUpload1.SaveAs(Server.MapPath(filepath));
                System.Drawing.Image myimage = System.Drawing.Image.FromFile(Server.MapPath(filepath));

                System.Drawing.Bitmap source_bitmap = new System.Drawing.Bitmap(myimage);
                myimage.Dispose();

                System.Drawing.Bitmap thumb_bitmap = new System.Drawing.Bitmap(width, height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(thumb_bitmap);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height);
                g.DrawImage(source_bitmap, 0, 0, width, height);
                g.Dispose();

                System.Drawing.Imaging.ImageCodecInfo[] Info = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                System.Drawing.Imaging.EncoderParameters Params = new System.Drawing.Imaging.EncoderParameters(1);
                Params.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                thumb_bitmap.Save(Server.MapPath(filepath), Info[1], Params);

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return encoded_image;
        }

        public string ImageToBase64(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public System.Drawing.Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }

        public int CountRepeatedCharInString(string str, char c)
        {
            int num_slash = 0;
            for (int index = 0; index < str.Length; index++)
            {
                if (str[index] == c)
                {
                    num_slash++;
                }
            }
            return num_slash;
        }

        public static string EscapeLikeValue(string valueWithoutWildcards)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < valueWithoutWildcards.Length; i++)
            {
                char c = valueWithoutWildcards[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();

            // select all that starts with the value string (in this case with "*")
            //string value = "*";
            // the dataView.RowFilter will be: "Name LIKE '[*]*'"
            //DataTable dt=new DataTable();
            //DataView dv = new DataView(dt);
            //dataView.RowFilter = String.Format("Name LIKE '{0}*'", EscapeLikeValue(value));
        }

        public string GetEncodeString(string strInput)
        {
            //Current Date
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
            culture.DateTimeFormat.DateSeparator = string.Empty;
            culture.DateTimeFormat.ShortDatePattern = "yyyyMMdd";
            culture.DateTimeFormat.LongDatePattern = "yyyyMMdd";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            string current_date_yyyymmdd_hhmmss_mmm = DateTime.Now.ToString("yyyymmdd_hhmmss") + DateTime.Now.Millisecond.ToString();

            //Random String 
            Random rnd = new Random();
            string strRandom = rnd.Next(10000, 99999).ToString() + GetRandomString();
           
            strInput = new Regex(@"[\s+\\\/:\*\?\&""\''<>|]").Replace(ConvertToUnSign(strInput), string.Empty);
            string encode_result = current_date_yyyymmdd_hhmmss_mmm + "_" + strRandom + "_" + strInput;
            return encode_result;
        }

        public void DownloadFile(string _URL, string _SaveAs)
        {
            try
            {
                System.Net.WebClient _WebClient = new System.Net.WebClient();
                // Downloads the resource with the specified URI to a local file.
                _WebClient.DownloadFile(_URL, _SaveAs);
            }
            catch (Exception _Exception)
            {
                _Exception.ToString();
            }
        }

        public string DownloadHTMLPage(string _URL)
        {
            string _PageContent = null;
            try
            {
                // Open a connection
                System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                // You can also specify additional header values like the user agent or the referer: (Optional)
                _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                _HttpWebRequest.Referer = "http://www.google.com/";

                // set timeout for 10 seconds (Optional)
                _HttpWebRequest.Timeout = 10000;

                // Request response:
                System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();

                // Open data stream:
                System.IO.Stream _WebStream = _WebResponse.GetResponseStream();

                // Create reader object:
                System.IO.StreamReader _StreamReader = new System.IO.StreamReader(_WebStream);

                // Read the entire stream content:
                _PageContent = _StreamReader.ReadToEnd();

                // Cleanup
                _StreamReader.Close();
                _WebStream.Close();
                _WebResponse.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                //Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }

            return _PageContent;
        }

        public string[] ExtractURLs(string str)
        {
            // match.Groups["name"].Value - URL Name
            // match.Groups["url"].Value - URI
            string RegexPattern = @"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>";

            // Find matches.
            System.Text.RegularExpressions.MatchCollection matches
                = System.Text.RegularExpressions.Regex.Matches(str, RegexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            string[] MatchList = new string[matches.Count];

            // Report on each match.
            int c = 0;
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                MatchList[c] = match.Groups["url"].Value;
                c++;
            }

            return MatchList;
        }

        public string StripHTML(string inputStr)
        {
            string outStr = "";
            if (inputStr != null)
            {
                outStr = Regex.Replace(inputStr, @"</?\w+((\s+\w+(\s*=\s*(?:""(.|\n)*?""|'(.|\n)*?'|[^'"">\s]+))?)+\s*|\s*)/?>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                outStr = outStr.Replace("'", "''");
            }

            return outStr;
        }

        public string ConvertHTML2PlainText(string source)
        {           
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove &nbsp;
                result = System.Text.RegularExpressions.Regex.Replace(result, "nbsp;", " ");
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,@"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>","<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)","</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)",string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>","<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)","</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)",string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>","<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)","</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)",string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>","\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>","\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>","\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>","\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>","\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>","\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>",string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" "," ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;"," * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;","<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;",">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;","(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;","/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;","<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;",">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;","(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;","(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)","\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)","\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)","\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)","\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)","\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+","\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index=0; index<result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch(Exception ex)
            {
                ex.ToString();
                return source;
            }
        }

        public System.Collections.ArrayList SplitSentences(string sSourceText)
        {
            // create a local string variable
            // set to contain the string passed it
            string sTemp = sSourceText;

            // create the array list that will be used to hold the sentences
            System.Collections.ArrayList al = new System.Collections.ArrayList();

            // split the sentences with a regular expression
            string[] splitSentences =Regex.Split(sTemp, @"(?<=['""A-Za-z0-9][\.\!\?])\s+(?=[A-Z])");

            // loop the sentences
            for (int i = 0; i < splitSentences.Length; i++)
            {
                // clean up the sentence one more time, trim it, and add it to the array list
                string sSingleSentence = splitSentences[i].Replace(Environment.NewLine,string.Empty);
                al.Add(sSingleSentence.Trim());
            }           
            // return the arraylist with all sentences added
            return al;
        }

        public string GetFirstCapitalizedLetterOfWords(string strInput)
        {           
            string result = null;
            string[] words = strInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                //first_capitialized_letter_of_words
                result += word.Substring(0, 1).ToUpper();

            }
            return result;
        }

        //check number
        public static bool CheckNumber(string strInput)
        {
            bool result = false;
            if (strInput != null)
            {
                //string pattern=@"^\d+$"; //The input consists of one or more decimal digits; for example "5", or "5683874674".
                string pattern = @"\d+";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result = regex.IsMatch(strInput);
            }
            return result;
        }

        public static bool IsGuid(string stringValue)
        {
            //string guidPattern = @"[a-fA-F0-9]{8}(-[a-fA-F0-9]{4}){3}-[a-fA-F0-9]{12}";
            string guidPattern = @"^\{?[0-9a-fA-F]{8}-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{12}\}?$";
            if (string.IsNullOrEmpty(stringValue))
                return false;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(guidPattern);
            return regex.IsMatch(stringValue);
        }
        


        public bool CheckLengthOfNumberInput(string strInput, int numeric_length){
            
            bool result = false;
            if (strInput != null)
            {                
                string pattern = @"^\d{" + numeric_length + "}$";  
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result = regex.IsMatch(strInput);
            }
            return result;

        }

        public bool CheckNumberInputLetters(string strInput,int strLength)
        {
            bool result = false;
            if (strInput != null)
            {
                string pattern = @"\w{0," + strLength + "}";  
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result = regex.IsMatch(strInput);
            }
            return result;
        }

        public static bool validateMoney(string ParMoney)
        {
            try
            {
                Convert.ToDouble(ParMoney);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateInput(string regex, string input)
        {
            //Alphaneumeric :^[^;>;&;<;%?*!~'`;:,."";+=|]*$
            //Address :^[^;>;&;<;%?$@{}*!~'`;:"";+=|]{1,200}$
            //User ID:^[a-zA-Z0-9._]{1,50}$
            //Alphanumeric with Spaces:^[a-zA-Z0-9_]{1,100}$
            //Phone No. :^[+0-9\s,-]{1,200}$
            //Numeric : ^[0-9]{1,18}$
            //Email :^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$
            //Decimal:^\d{0,7}(\.\d{1,2})?$
            //Time:((1+[012]+)|([123456789]{1}))(((:|\s)[0-5]+[0- 9]+))?(\s)?((a|A|p|P)(m|M))
            //Date Format:dd/MM/yyyy
            //Financial Year :^[\d]{4}[\-][\d]{2}$|^[\d]{4}[\-][\d]{4}$
            //URL: ^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$
            //Email: ^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$
            //Currency (non- negative) : ^\d+(\.\d\d)?$
            //Currency : ^(-)?\d+(\.\d\d)?$
            //Charactors [a-z][A-Z] only : ^[a-zA-Z'.\s]{1,40}$
            //Number Only: ^\d*([.]\d*)?|[.]\d+$
            //Money : RegExp( /^\$?(?:\d+|\d{1,3}(?:,\d{3})*)(?:\.\d{1,2}){0,1}$/ );    <=>  ^\$?[0-9]+(,[0-9]{3})*(\.[0-9]{2})?$
            //

            // Create a new Regex based on the specified regular expression.
            Regex r = new Regex(regex);

            // Test if the specified input matches the regular expression.
            return r.IsMatch(input);
        }

        //check validate HTTP or HTTPS URL
        public bool checkHttps(string strInput)
        {
            bool result=false;
            if(strInput!=null){
                string pattern = @"^https?://([\w- ]+\.)+[\w-]+(/[\w- ./ ?%=]*)?$";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result = regex.IsMatch(strInput);
            }
            return result;
        }

        //Internet e-mail address. 
        public bool checkEmail(string strInput)
        {
            bool result = false;           
           
            if (strInput != null)
            {
                string pattern = @"^[\w-]+@([\w- ]+\.)+[\w-]+$"; //The [\w-]+ expression indicates that each address element must consist of one or more word characters or hyphens 
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result = regex.IsMatch(strInput);
            }
            return result;
        }

        public bool checkCreditCardNumber(string strInput)
        {            
            bool result = false;                
            if (strInput != null)
            {
                string pattern = @"^\d{4}-?\d{4}-?\d{4}- ?\d{4}$";  //The credit card numbers string likes "4921835221552042" or "4921-8352- 2155-2042".
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result = regex.IsMatch(strInput);
            }
            return result;
        }

        public bool checkSimplePassword(string strInput)
        {
            bool result = false;
            if (strInput != null)
            {
                string pattern = @"^\w{6,8}$";  //The input consists of between 6 through 8 characters; for example "ghtd6f" or "b8c7hogh".^\w{6,8}$
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result = regex.IsMatch(strInput);
            }
            return result;
        }

        public bool IsValidPassword(string strPassword)
        {
            //^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,40}$ =>The following regular expression is for a 6 to 40 char password and containing at least an alphabet and one Number.         

            bool result = false;
            if (strPassword != null)
            {
                string pattern = @"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{6,15})$";  //The input consists of between 6 through 15 characters
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result=regex.IsMatch(strPassword);               
            }
            return result;       
        }

        public bool checkComplexPassword(string strPassword, int num_letters)
        {
            //The regular expression enforces the following rules:
            //    * (?=.{"+num_letters+@",}) => Passwords will contain at least num_letters 
            //    * (?=.*[a-z])(?=.*[A-Z]) => Passwords will contain at least (1) upper case letter , (1) lower case letter
            //    * (?=.*[!@#$%^&_+-*/=])  => Passwords will contain at least (1) special character [.,;:'""~^><@#$%^&{}?!|\_+-*/=]
            //      (?=.*\W) => Passwords with no spaces
            //check combination upper/lowercase, no spaces, and some special characters ($#_!@). Min 6, max 10. ^(?=[\w$#_!@]{6,10})[\w$#_!@]{6,10}$
            //By the way on MSDN ^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,10}$ is what the MVP is talking about which is listed as a custom expresion.

            //(?=^(?=.{14,256}$)(?=.*[A-Z].*[A-Z])(?=.*[a-z].*[a-z])(?=.*[0-9].*[0-9])(?=^[A-Za-z]{1})(?=.*[!@#$%^*_:].*[!@#$%^*_:])(?!.*[\s&quot;\s&amp;()+}{;=`~:\\|'?/>.<,])).*$ ////The following regular expressions are being used for password validation requiring two uppercase characters, two lowercase characters, two numbers, no spaces and two of the following special characters !@#$%^*_: with minimum of password length of 14          
            //(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$   =>Passwords will contain at least (1) upper case letter, (1) lower case letter, (1) number or special character, (8) characters in length, and maximum length should not be arbitrarily limited
            //^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$
            //^\w*(?=\w*\d)(?=\w*[a-z])(?=\w*[A-Z])\w*$/   //discovered that the previous one allows spaces
            //^(?=.*(\d|[^a-zA-Z]))(?=.*[a-z])(?=.*[A-Z])(?!.*\s).{6,12}$
            //^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$
            //(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[a-z])(?=.*[A-Z]).*$  //(1) number or special character, (8) characters in length, maximum length should not be arbitrarily limited
            //(?=^[\w$#]{6,20}$)(?=.*?\d)(?=.*?[A-Z])(?=.*?[a-z])^[\w$#]*$ 

            //To turn off case sensitivity use (?-i) and to turn on multiline use (?m). So the regular expression would be: (?-i)(?m)^(?=.*?\d)(?=.*?[a-z])(?=.*?[A-Z])[a-zA-Z].{8,20}$ (I also added code to require a password to start with a character) 
            //Simply do EnableClientScript="False" and use single backslash(\), ie. ValidationExpression="^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$" (do not change the .* position otherwise it won't work.)
            //Regular expression to validate a password. Password must contain 6 characters and no more than 20, at least one upper case letter (A-Z), one lower case letter (a-z), and one numeric character (0-9). The other characters may be from the set A-Za-z0-9$#_\ plus blank. ^(?=[\w$#_ ]{6,20})(?=.*?\d)(?=.*?[A-Z])(?=.*?[a-z])[\w$#_ ]*$

            //Password must be at least 8 characters long, contain at least one one lower case letter, one upper case letter, one digit and one special character." 
            //PasswordRegularExpression = '^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$' 
            //PasswordRegularExpressionErrorMessage="Error: Your password must be at least 8 characters long, contain at least one one lower case letter, one upper case letter, one digit and one special character, Valid special characters."


            bool result = false;
            if (strPassword != null)
            {             
                //string pattern = @"^.*(?=.{" + num_letters + @",})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[.,;'""~><?!@#$%^&{}|\_+-*/=:])(?=.*\W).*$";  
                string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[.,;'""~><?!@#$%^&{}|\_+-*/=:])(?=.*\W).{"+ num_letters +",}$";  
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result=regex.IsMatch(strPassword);               
            }
            return result;
        }

        public static string ToXml()
        {
            StringBuilder builder = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(builder))
            {
                using (XmlTextWriter writer = new XmlTextWriter(stringWriter))
                {
                    // This produces UTF16 XML
                    writer.Indentation = 4;
                    writer.IndentChar = '\t';
                    writer.Formatting = Formatting.Indented;

                    writer.WriteStartDocument();

                    writer.WriteStartElement("Root");
                    writer.WriteAttributeString("myattrib", "123");
                    writer.WriteEndElement();

                    writer.WriteEndDocument();
                }
            }


            return builder.ToString();
        }

        public static string ToUTF8Xml()
        {
            string result;

            MemoryStream stream = new MemoryStream(); // The writer closes this for us
            using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                writer.Indentation = 4;
                writer.IndentChar = '\t';
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();

                writer.WriteStartElement("Root");
                writer.WriteAttributeString("myattrib", "123");
                writer.WriteEndElement();

                writer.WriteEndDocument();
                writer.Flush();

                // Make sure you flush or you only get half the text
                writer.Flush();

                // Use a StreamReader to get the byte order correct
                StreamReader reader = new StreamReader(stream, Encoding.UTF8, true);
                stream.Seek(0, SeekOrigin.Begin);
                result = reader.ReadToEnd();

                // #2 - doesn't write the byte order reliably
                //result = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
            }

            // Make sure you use File.WriteAllText("myfile", xml, Encoding.UTF8);
            // or equivalent to write your file back.
            return result;
        }

        ///
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        ///
        /// Unicode Byte Array to be converted to String /// String converted from Unicode Byte Array
        private String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public static string FormatDate(string strDate)
        {
            int pos = strDate.IndexOf(" ");
            string strRe = "";
            string strChar = "";
            if (pos > 0)
            {
                string Temp = strDate.Substring(0, pos);
                string[] arrDate = Temp.Split('/');

                for (int i = 0; i < arrDate.Length; i++)
                {
                    if (i != 0)
                    {
                        strChar = "-";
                    }
                    if (arrDate[i].Length == 1)
                    {
                        strRe += strChar + "0" + arrDate[i];
                    }
                    else if (arrDate[i].Length == 2)
                    {
                        strRe += strChar + arrDate[i];
                    }
                    else if (arrDate[i].Length >= 4)
                    {
                        strRe += strChar + arrDate[i].Substring(2, 2);
                    }
                }
            }
            else
            {
                strRe = strDate;
            }

            string[] fm = strRe.Split('-');
            if (fm.Length > 1)
                strRe = fm[1] + "-" + fm[0] + "-" + fm[2];
            return strRe;
        }

        ///
        /// Converts the String to UTF8 Byte array and is used in De-serialization
        ///      
        private Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        public string UTF8_Encode(string strInput)
        {
            string result = null;
            try
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                byte[] byte_array = enc.GetBytes(strInput.Replace("'", "''"));
                result = enc.GetString(byte_array);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return result;
        }

        static private string FixCode(string html)
        {
            html = html.Replace("  ", "&nbsp; ");
            html = html.Replace("  ", " &nbsp;");
            html = html.Replace("\t", "&nbsp; &nbsp;&nbsp;");
            html = html.Replace("[", "&#91;");
            html = html.Replace("]", "&#93;");
            html = html.Replace("<", "&lt;");
            html = html.Replace(">", "&gt;");
            html = html.Replace("\r\n", "<br/>");
            return html;
        }

        public void getAppPath()
        {
            string path = HttpContext.Current.Request.ApplicationPath;
            if (!path.EndsWith("/")) path += "/";

            //if (Config.Root != null)
            //{
            //    // use specified root
            //    path = Config.Root;
            //    if (path[0] != '/') path = path.Insert(0, "/");

            //}
            //else if (Config.IsDotNetNuke)
            //{
            //    path += "DesktopModules/YetAnotherForumDotNet/";
            //}
        }

        private static string getContent(string url)
        {
            string result = "";
            System.Net.HttpWebRequest wReq;
            System.Net.HttpWebResponse wResp;
            System.IO.Stream rStream;
            // Place the web request to the server by specifying the URL
            wReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            // No need for a persistant connection
            wReq.KeepAlive = false;
            // The link that referred us to the URL
            wReq.Referer = url;
            // The user agent of the browser
            wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET C" + "LR 2.0.50215)";
            // Get the response from the server
            wResp = (System.Net.HttpWebResponse)wReq.GetResponse();
            // Display the header of the response
            //wResp.Headers.ToString();
            // Get a stream to read the body of the response
            rStream = wResp.GetResponseStream();
            // Needed for looping through the buffer
            int bufCount = 0;
            // Buffer in which we're going to store data coming from the server
            byte[] byteBuf = new byte[1024];
            // Loop as long as there's data in the buffer
            bufCount = rStream.Read(byteBuf, 0, byteBuf.Length);
            while (bufCount > 0)
            {
                // Fill the buffer with data
                if (bufCount != 0)
                {
                    // Transform the bytes into ASCII text and append to the textbox
                    result += System.Text.Encoding.UTF8.GetString(byteBuf, 0, bufCount);
                }
                bufCount = rStream.Read(byteBuf, 0, byteBuf.Length);
            }

            return result;
        }

        private string GetMD5Hash(string input)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            StringBuilder s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public string SetRandomCaptchaString(int size, bool lowerCase)
        {
            string s = null;
            Random r = new Random();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * r.NextDouble() + 65)));
                s += ch.ToString();
            }
            // storing the password in the DataSet
            s = ((lowerCase ? s.ToString().ToLower() : s.ToString()));
            return s;
        }
               
        public static string ConverId_Id_rewrite(string id)
        {
            string result = "";
            if (id != "")
            {
                if (id.IndexOf("-") > -1)
                    result = id.Substring(0, id.IndexOf("-"));
                else
                    result = id;
            }
            return result;
        }

        public static string Conver_rewrite_GetId(string link)
        {
            string result = "";

            if (link.IndexOf("_") > -1)
                result = link.Substring(4, link.IndexOf("_") - 1);

            return result;
        }


        //public string HighlightKeyWords(string text, string keywords, string cssClass)
        //{
        //    if (text == String.Empty || keywords == String.Empty || cssClass == String.Empty)
        //        return text;
        //    var words = keywords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        //    //return words.Select(word => word.Trim()).Aggregate(text,(current, pattern) => Regex.Replace(current,pattern,string.Format("<span style=\"background-color:{0}\">{1}</span>",cssClass,"$0"),RegexOptions.IgnoreCase));
        //    //return words.Select(word => "\\b" + word.Trim() + "\\b").Aggregate(text, (current, pattern) => Regex.Replace(current, pattern, string.Format("<span style=\"background-color:{0}\">{1}</span>", cssClass, "$0"), RegexOptions.IgnoreCase));
        //    return words.Select(word => word.Trim()).Aggregate(text, (current, pattern) => Regex.Replace(current, pattern, string.Format("<span class=\"{0}\">{1}</span>", cssClass, "$0"), RegexOptions.IgnoreCase));
        //    //return words.Select(word => "\\b" + word.Trim() + "\\b").Aggregate(text, (current, pattern) => Regex.Replace(current, pattern, string.Format("<span class=\"{0}\">{1}</span>", cssClass, "$0"), RegexOptions.IgnoreCase));
        //}

        //public static string ResolveStrUrl(string originalUrl)
        //{
        //    string resolve_url = string.Empty;

        //    if (string.IsNullOrEmpty(originalUrl))
        //    {
        //        resolve_url = null;
        //    }

        //    // *** Fix up image path for ~ root app dir directory
        //    if (originalUrl.StartsWith("~"))
        //    {
        //        if (HttpContext.Current != null)
        //        {
        //            resolve_url = HttpContext.Current.Request.ApplicationPath + originalUrl.Substring(1).Replace("//", "/");
        //        }
        //        else{throw new ArgumentException("Invalid URL: Relative URL not allowed.");}
        //    }
        //    else
        //    {
        //        resolve_url = originalUrl;
        //    }

        //    // *** Absolute path - just return
        //    if (originalUrl.IndexOf("://") != -1)
        //    {
        //        resolve_url = originalUrl;
        //    }

        //    if (originalUrl.IndexOf('?') != 1)
        //    {
        //        string queryString = originalUrl.Substring(originalUrl.IndexOf('?'));
        //        string baseUrl = originalUrl.Substring(0, originalUrl.IndexOf('?'));

        //        resolve_url = string.Concat(VirtualPathUtility.ToAbsolute(baseUrl), queryString);
        //    }
        //    else
        //    {
        //        resolve_url = VirtualPathUtility.ToAbsolute(originalUrl);
        //    }
        //    return resolve_url;
        //}

        //public static string ResolveServerUrl(string serverUrl, bool forceHttps)
        //{         
        //    Uri originalUri = HttpContext.Current.Request.Url;
        //    if (!string.IsNullOrEmpty(serverUrl))
        //    {
        //        if (serverUrl.IndexOf("://") > -1)
        //        {
        //            serverUrl = ResolveStrUrl(serverUrl);
        //            originalUri = new Uri(originalUri, serverUrl);                   
        //        }                
        //    }
        //    if (forceHttps && !string.Equals(originalUri, Uri.UriSchemeHttps))
        //    {
        //        UriBuilder builder = new UriBuilder(originalUri);
        //        builder.Scheme = Uri.UriSchemeHttps;
        //        builder.Port = 443;
        //        originalUri = builder.Uri;
        //    }
        //    return originalUri.ToString();

        //}

        //public static string FixResolveStrUrl(string relativeUrl)
        //{
        //    if (relativeUrl == null) throw new ArgumentNullException("relativeUrl");

        //    if (relativeUrl.Length == 0 || relativeUrl[0] == '/' || relativeUrl[0] == '\\')
        //        return relativeUrl;

        //    int idxOfScheme = relativeUrl.IndexOf(@"://", StringComparison.Ordinal);
        //    if (idxOfScheme != -1)
        //    {
        //        int idxOfQM = relativeUrl.IndexOf('?');
        //        if (idxOfQM == -1 || idxOfQM > idxOfScheme) return relativeUrl;
        //    }

        //    StringBuilder sbUrl = new StringBuilder();
        //    sbUrl.Append(HttpRuntime.AppDomainAppVirtualPath);
        //    if (sbUrl.Length == 0 || sbUrl[sbUrl.Length - 1] != '/') sbUrl.Append('/');

        //    // found question mark already? query string, do not touch!
        //    bool foundQM = false;
        //    bool foundSlash; // the latest char was a slash?
        //    if (relativeUrl.Length > 1 && relativeUrl[0] == '~' && (relativeUrl[1] == '/' || relativeUrl[1] == '\\'))
        //    {
        //        relativeUrl = relativeUrl.Substring(2);
        //        foundSlash = true;
        //    }
        //    else foundSlash = false;
        //    foreach (char c in relativeUrl)
        //    {
        //        if (!foundQM)
        //        {
        //            if (c == '?') foundQM = true;
        //            else
        //            {
        //                if (c == '/' || c == '\\')
        //                {
        //                    if (foundSlash) continue;
        //                    else
        //                    {
        //                        sbUrl.Append('/');
        //                        foundSlash = true;
        //                        continue;
        //                    }
        //                }
        //                else if (foundSlash) foundSlash = false;
        //            }
        //        }
        //        sbUrl.Append(c);
        //    }

        //    return sbUrl.ToString();
        //}

    }    
}