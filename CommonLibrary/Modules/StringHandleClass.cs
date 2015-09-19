using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonLibrary.Modules
{
    public class StringHandleClass
    {      
        public static string GenerateCode(string strInput, int max_number)
        {
            int total = 0;
            if (strInput.Length <= max_number)
            {
                total = max_number - strInput.Length;
                for (int i = 0; i < total; i++)
                {
                    strInput = "0" + strInput;
                }
            }
            return strInput;
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

        public string GetAlphanumericString(string s)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in s)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
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

        /*** Strips accents off words = Loai bo dau trong tieng viet*/
        public string StripDiacritics(string accented)
        {
            string result = null;
            if (accented != null)
            {
                string pattern = @"\\pp{IsCombiningDiacriticalMarks}+";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                string strFormD = accented.Normalize(System.Text.NormalizationForm.FormD);
                result = regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
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
            string result = null;
            if (strIput != null)
            {
                string pattern = @"\s+";
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
                source = source.Substring(0, endtext2) + " ...";
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
            string result = UTF8_Encode(removeHTMLtab(sb.ToString(), ""));
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
            //sb.Replace("-", "");
            sb.Replace(' ', '-');
            sb.Replace(":", "");
            sb.Replace("\"", "");
            sb.Replace(",", "-");
            sb.Replace("&amp", "_");
            sb.Replace("--", "-");
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


        public static string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890.,";           
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        public static string RemoveExtraTextWithoutPointOrComma(string value)
        {            
            var allowedChars = "01234567890";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        public string ConvertVNIToUnicode(string VNI)
        {
            string sUnicode = "";
            int iChieuDai = 0;
            if (VNI.Length == 0) return "";
            iChieuDai = VNI.Length - 1;
            int i = 0;
            while (i <= iChieuDai)
            {
                switch ((byte)VNI[i])
                {
                    case 97: //, 101, 105, 111, 117 'a,e,i,o,u
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 249: sUnicode += "á"; i += 2; //dấu sắc
                                    break;
                                case 248: sUnicode += "à"; i += 2; //dấu huyền
                                    break;
                                case 251: sUnicode += "ả"; i += 2; //dấu hỏi
                                    break;
                                case 245: sUnicode += "ã"; i += 2; //dấu ngã
                                    break;
                                case 239: sUnicode += "ạ"; i += 2; //dấu nặng
                                    break;
                                case 226: sUnicode += "â"; i += 2; //^
                                    break;
                                case 225: sUnicode += "ấ"; i += 2; //ấ
                                    break;
                                case 224: sUnicode += "ầ"; i += 2; //ầ
                                    break;
                                case 229: sUnicode += "ẩ"; i += 2; //ẩ
                                    break;
                                case 227: sUnicode += "ẫ"; i += 2; //ẫ
                                    break;
                                case 228: sUnicode += "ậ"; i += 2; //ậ
                                    break;
                                case 234: sUnicode += "ă"; i += 2; //ă
                                    break;
                                case 233: sUnicode += "ắ"; i += 2; //ắ
                                    break;
                                case 232: sUnicode += "ằ"; i += 2; //ằ
                                    break;
                                case 250: sUnicode += "ẳ"; i += 2; //ẳ
                                    break;
                                case 252: sUnicode += "ẵ"; i += 2; //ẵ
                                    break;
                                case 235: sUnicode += "ặ"; i += 2; //ặ
                                    break;
                                default: sUnicode += "a"; i += 1;
                                    break;
                            }
                        }
                        else
                        {
                            sUnicode += "a"; i += 1;
                        }
                        break;
                    case 65: //A
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 217: sUnicode += "Á"; i += 2; //dấu sắc
                                    break;
                                case 216: sUnicode += "À"; i += 2; //dấu huyền
                                    break;
                                case 219: sUnicode += "Ả"; i += 2; //dấu hỏi
                                    break;
                                case 213: sUnicode += "Ã"; i += 2; //dấu ngã
                                    break;
                                case 207: sUnicode += "Ạ"; i += 2; //dấu nặng
                                    break;
                                case 194: sUnicode += "Â"; i += 2; //^
                                    break;
                                case 193: sUnicode += "Ấ"; i += 2; //ấ
                                    break;
                                case 192: sUnicode += "Ầ"; i += 2; //ầ
                                    break;
                                case 197: sUnicode += "Ẩ"; i += 2; //ẩ
                                    break;
                                case 195: sUnicode += "Ẫ"; i += 2; //ẫ
                                    break;
                                case 196: sUnicode += "Ậ"; i += 2; //ậ
                                    break;
                                case 202: sUnicode += "Ă"; i += 2; //ă
                                    break;
                                case 201: sUnicode += "Ắ"; i += 2; //ắ
                                    break;
                                case 200: sUnicode += "Ằ"; i += 2; //ằ
                                    break;
                                case 218: sUnicode += "Ẳ"; i += 2; //ẳ
                                    break;
                                case 220: sUnicode += "Ẵ"; i += 2; //ẵ
                                    break;
                                case 203: sUnicode += "Ặ"; i += 2; //ặ
                                    break;
                                //Trường hợp bị lỗi
                                case 249: sUnicode += "Á"; i += 2; //dấu sắc
                                    break;
                                case 248: sUnicode += "À"; i += 2; //dấu huyền
                                    break;
                                case 251: sUnicode += "Ả"; i += 2; //dấu hỏi
                                    break;
                                case 245: sUnicode += "Ã"; i += 2;  //dấu ngã
                                    break;
                                case 239: sUnicode += "Ạ"; i += 2; //dấu nặng
                                    break;
                                case 226: sUnicode += "Â"; i += 2; //^
                                    break;
                                case 225: sUnicode += "Ấ"; i += 2; //ấ
                                    break;
                                case 224: sUnicode += "Ầ"; i += 2; //ầ
                                    break;
                                case 229: sUnicode += "Ẩ"; i += 2; //ẩ
                                    break;
                                case 227: sUnicode += "Ẫ"; i += 2; //ẫ
                                    break;
                                case 228: sUnicode += "Ậ"; i += 2; //ậ
                                    break;
                                case 234: sUnicode += "Ă"; i += 2; //ă
                                    break;
                                case 233: sUnicode += "Ắ"; i += 2; //ắ
                                    break;
                                case 232: sUnicode += "Ằ"; i += 2; //ằ
                                    break;
                                case 250: sUnicode += "Ẳ"; i += 2; //ẳ
                                    break;
                                case 252: sUnicode += "Ẵ"; i += 2; //ẵ
                                    break;
                                case 235: sUnicode += "Ặ"; i += 2; //ặ
                                    break;
                                default: sUnicode += "A"; i += 1; break;
                            }
                        }
                        else { sUnicode += "A"; i += 1; } break;
                    case 101: //e
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 249: sUnicode += "é"; i += 2; //dấu sắc
                                    break;
                                case 248: sUnicode += "è"; i += 2; //dấu huyền
                                    break;
                                case 251: sUnicode += "ẻ"; i += 2; //dấu hỏi
                                    break;
                                case 245: sUnicode += "ẽ"; i += 2; //dấu ngã
                                    break;
                                case 239: sUnicode += "ẹ"; i += 2; //dấu nặng
                                    break;
                                case 226: sUnicode += "ê"; i += 2; //^
                                    break;
                                case 225: sUnicode += "ế"; i += 2; //ấ
                                    break;
                                case 224: sUnicode += "ề"; i += 2; //ầ
                                    break;
                                case 229: sUnicode += "ể"; i += 2; //ẩ
                                    break;
                                case 227: sUnicode += "ễ"; i += 2; //ẫ
                                    break;
                                case 228: sUnicode += "ệ"; i += 2; //ậ
                                    break;
                                default: sUnicode += "e"; i += 1;
                                    break;
                            }
                        }
                        else { sUnicode += "e"; i += 1; } break;
                    case 69:
                        //E
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 217: sUnicode += "É"; i += 2; //dấu sắc
                                    break;
                                case 216: sUnicode += "È"; i += 2; //dấu huyền
                                    break;
                                case 219: sUnicode += "Ẻ"; i += 2; //dấu hỏi
                                    break;
                                case 213: sUnicode += "Ẽ"; i += 2; //dấu ngã
                                    break;
                                case 207: sUnicode += "Ẹ"; i += 2; //dấu nặng
                                    break;
                                case 194: sUnicode += "Ê"; i += 2; //^
                                    break;
                                case 193: sUnicode += "Ế"; i += 2; //ấ
                                    break;
                                case 192: sUnicode += "Ề"; i += 2; //ầ
                                    break;
                                case 197: sUnicode += "Ể"; i += 2; //ẩ
                                    break;
                                case 195: sUnicode += "Ễ"; i += 2; //ẫ
                                    break;
                                case 196: sUnicode += "Ệ"; i += 2; //ậ
                                    break;
                                default: sUnicode += "E"; i += 1;
                                    break;
                            }
                        }
                        else { sUnicode += "E"; i += 1; } break;
                    case 111:
                        //o
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 249: sUnicode += "ó"; i += 2; //dấu sắc
                                    break;
                                case 248: sUnicode += "ò"; i += 2; //dấu huyền
                                    break;
                                case 251: sUnicode += "ỏ"; i += 2; //dấu hỏi
                                    break;
                                case 245: sUnicode += "õ"; i += 2; //dấu ngã
                                    break;
                                case 239: sUnicode += "ọ"; i += 2; //dấu nặng
                                    break;
                                case 226: sUnicode += "ô"; i += 2; //^
                                    break;
                                case 225: sUnicode += "ố"; i += 2; //ấ
                                    break;
                                case 224: sUnicode += "ồ"; i += 2; //ầ
                                    break;
                                case 229: sUnicode += "ổ"; i += 2; //ẩ
                                    break;
                                case 227: sUnicode += "ỗ"; i += 2; //ẫ
                                    break;
                                case 228: sUnicode += "ộ"; i += 2; //ậ
                                    break;
                                default: sUnicode += "o"; i += 1;
                                    break;
                            }
                        }
                        else { sUnicode += "o"; i += 1; } break;
                    case 79: //O
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 217: sUnicode += "Ó"; i += 2; //dấu sắc
                                    break;
                                case 216: sUnicode += "Ò"; i += 2; //dấu huyền
                                    break;
                                case 219: sUnicode += "Ỏ"; i += 2; //dấu hỏi
                                    break;
                                case 213: sUnicode += "Õ"; i += 2; //dấu ngã
                                    break;
                                case 207: sUnicode += "Ọ"; i += 2; //dấu nặng
                                    break;
                                case 194: sUnicode += "Ô"; i += 2; //^
                                    break;
                                case 193: sUnicode += "Ố"; i += 2; //ấ
                                    break;
                                case 192: sUnicode += "Ồ"; i += 2; //ầ
                                    break;
                                case 197: sUnicode += "Ổ"; i += 2; //ẩ
                                    break;
                                case 195: sUnicode += "Ỗ"; i += 2; //ẫ
                                    break;
                                case 196: sUnicode += "Ộ"; i += 2; //ậ
                                    break;
                                default: sUnicode += "O"; i += 1; break;
                            }
                        }
                        else { sUnicode += "O"; i += 1; } break;
                    case 117: //u
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 249: sUnicode += "ú"; i += 2; //dấu sắc
                                    break;
                                case 248: sUnicode += "ù"; i += 2; //dấu huyền
                                    break;
                                case 251: sUnicode += "ủ"; i += 2; //dấu hỏi
                                    break;
                                case 245: sUnicode += "ũ"; i += 2; //dấu ngã
                                    break;
                                case 239: sUnicode += "ụ"; i += 2; //dấu nặng
                                    break;
                                default: sUnicode += "u"; i += 1; break;
                            }
                        }
                        else { sUnicode += "u"; i += 1; } break;
                    case 85: //U
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 217: sUnicode += "Ú"; i += 2; //dấu sắc
                                    break;
                                case 216: sUnicode += "Ù"; i += 2; //dấu huyền
                                    break;
                                case 219: sUnicode += "Ủ"; i += 2; //dấu hỏi
                                    break;
                                case 213: sUnicode += "Ũ"; i += 2; //dấu ngã
                                    break;
                                case 207: sUnicode += "Ụ"; i += 2; //dấu nặng
                                    break;
                                default: sUnicode += "U"; i += 1; break;
                            }
                        }
                        else { sUnicode += "U"; i += 1; } break;
                    case 244: //ơ
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 249: sUnicode += "ớ"; i += 2; //dấu sắc
                                    break;
                                case 248: sUnicode += "ờ"; i += 2; //dấu huyền
                                    break;
                                case 251: sUnicode += "ở"; i += 2; //dấu hỏi
                                    break;
                                case 245: sUnicode += "ỡ"; i += 2; //dấu ngã
                                    break;
                                case 239: sUnicode += "ợ"; i += 2; //dấu nặng
                                    break;
                                default: sUnicode += "ơ"; i += 1; break;
                            }
                        }
                        else
                        {
                            sUnicode += "ơ";
                            i += 1;
                        }
                        break;
                    case 212: //Ơ
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 217: sUnicode += "Ớ"; i += 2; //dấu sắc
                                    break;
                                case 216: sUnicode += "Ờ"; i += 2; //dấu huyền
                                    break;
                                case 219: sUnicode += "Ở"; i += 2; //dấu hỏi
                                    break;
                                case 213: sUnicode += "Ỡ"; i += 2; //dấu ngã
                                    break;
                                case 207: sUnicode += "Ợ"; i += 2; //dấu nặng
                                    break;
                                default: sUnicode += "Ơ"; i += 1; break;
                            }
                        }
                        else { sUnicode += "Ơ"; i += 1; } break;
                    case 246: //ư
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 249: sUnicode += "ứ"; i += 2; //dấu sắc
                                    break;
                                case 248: sUnicode += "ừ"; i += 2; //dấu huyền
                                    break;
                                case 251: sUnicode += "ử"; i += 2; //dấu hỏi
                                    break;
                                case 245: sUnicode += "ữ"; i += 2; //dấu ngã
                                    break;
                                case 239: sUnicode += "ự"; i += 2; //dấu nặng
                                    break;
                                default: sUnicode += "ư"; i += 1;
                                    break;
                            }
                        }
                        else { sUnicode += "ư"; i += 1; } break;
                    case 214: //Ư
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 217: sUnicode += "Ứ"; i += 2; //dấu sắc
                                    break;
                                case 216: sUnicode += "Ừ"; i += 2; //dấu huyền
                                    break;
                                case 219: sUnicode += "Ử"; i += 2; //dấu hỏi
                                    break;
                                case 213: sUnicode += "Ữ"; i += 2; //dấu ngã
                                    break;
                                case 207: sUnicode += "Ự"; i += 2; //dấu nặng
                                    break;
                                default: sUnicode += "Ư"; i += 1; break;
                            }
                        }
                        else { sUnicode += "Ư"; i += 1; } break;
                    case 121: //y
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 249: sUnicode += "ý"; i += 2; //dấu sắc
                                    break;
                                case 248: sUnicode += "ỳ"; i += 2; //dấu huyền
                                    break;
                                case 251: sUnicode += "ỷ"; i += 2; //dấu hỏi
                                    break;
                                case 245: sUnicode += "ỹ"; i += 2; //dấu ngã
                                    break;
                                case 239: sUnicode += "ỵ"; i += 2; //dấu nặng
                                    break;
                                default: sUnicode += "y"; i += 1; break;
                            }
                        }
                        else { sUnicode += "y"; i += 1; }
                        break;
                    case 89: //Y
                        if (i < iChieuDai)
                        {
                            switch ((byte)VNI[i + 1])
                            {
                                case 217: sUnicode += "Ý"; i += 2; //dấu sắc
                                    break;
                                case 216: sUnicode += "Ỳ"; i += 2; //dấu huyền
                                    break;
                                case 219: sUnicode += "Ỷ"; i += 2; //dấu hỏi
                                    break;
                                case 213: sUnicode += "Ỹ"; i += 2; //dấu ngã
                                    break;
                                case 207: sUnicode += "Ỵ"; i += 2; //dấu nặng
                                    break;
                                default: sUnicode += "Y"; i += 1; break;
                            }
                        }
                        else { sUnicode += "Y"; i += 1; } break;
                    case 237: sUnicode += "í"; i += 1; break;
                    case 236: sUnicode += "ì"; i += 1; break;
                    case 230: sUnicode += "ỉ"; i += 1; break;
                    case 243: sUnicode += "ĩ"; i += 1; break;
                    case 242: sUnicode += "ị"; i += 1; break;
                    case 205: sUnicode += "Í"; i += 1; break;
                    case 204: sUnicode += "Ì"; i += 1; break;
                    case 198: sUnicode += "Ỉ"; i += 1; break;
                    case 211: sUnicode += "Ĩ"; i += 1; break;
                    case 210: sUnicode += "Ị"; i += 1; break;
                    case 241: sUnicode += "đ"; i += 1; break;
                    case 209: sUnicode += "Đ"; i += 1; break;
                    case 238:
                    case 255: sUnicode += "ỵ"; i += 1; break;
                    case 159:
                    case 206: sUnicode += "Ỵ"; i += 1; break;
                    default:
                        sUnicode += VNI[i]; i += 1; break;
                }
            }
            return sUnicode;
        }
    }
}
