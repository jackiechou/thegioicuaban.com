using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace CommonLibrary.Common.Utilities
{
    public static class ShellUtils
    {

        //[DllImport("Shell32.dll")]
        //private static extern int ShellExecute(int hwnd, string lpOperation,
        //    string lpFile, string lpParameters,
        //    string lpDirectory, int nShowCmd);

        /// <summary>
        /// Uses the Shell Extensions to launch a program based or URL moniker.
        /// </summary>
        /// <param name="url">Any URL Moniker that the Windows Shell understands (URL, Word Docs, PDF, Email links etc.)</param>
        /// <returns></returns>
        public static int GoUrl(string url)
        {
            string TPath = Path.GetTempPath();

            ProcessStartInfo info = new ProcessStartInfo();
            info.UseShellExecute = true;
            info.Verb = "Open";
            info.WorkingDirectory = TPath;
            info.FileName = url;

            Process process = new Process();
            process.StartInfo = info;
            process.Start();

            return 0;
        }






        /// <summary>
        /// Displays an HTML string in a browser window
        /// </summary>
        /// <param name="HtmlString"></param>
        /// <returns></returns>
        public static int ShowString(string HtmlString, string extension)
        {
            if (extension == null)
                extension = "htm";

            string File = Path.GetTempPath() + "\\__preview." + extension;
            StreamWriter sw = new StreamWriter(File, false, Encoding.Default);
            sw.Write(HtmlString);
            sw.Close();

            return GoUrl(File);
        }

        public static int ShowHtml(string HtmlString)
        {
            return ShowString(HtmlString, null);
        }

        /// <summary>
        /// Displays a large Text string as a text file.
        /// </summary>
        /// <param name="TextString"></param>
        /// <returns></returns>
        public static int ShowText(string TextString)
        {
            string File = Path.GetTempPath() + "\\__preview.txt";

            StreamWriter sw = new StreamWriter(File, false);
            sw.Write(TextString);
            sw.Close();

            return GoUrl(File);
        }

        /// <summary>
        /// Simple method to retrieve HTTP content from the Web quickly
        /// </summary>
        /// <param name="url"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static string HttpGet(string url, ref string errorMessage)
        {
            string responseText = string.Empty;

            WebClient Http = new WebClient();

            // Download the Web resource and save it into a data buffer.
            try
            {
                byte[] Result = Http.DownloadData(url);
                responseText = Encoding.Default.GetString(Result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            return responseText;
        }

        /// <summary>
        /// Retrieves a buffer of binary data from a URL using
        /// a plain HTTP Get.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static byte[] HttpGetBytes(string url, ref string errorMessage)
        {
            byte[] result = null;

            var Http = new WebClient();

            try
            {
                result = Http.DownloadData(url);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            return result;
        }

    }
}
