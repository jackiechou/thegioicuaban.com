using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ArticleLibrary
{
    /// <summary>
    /// Converts an HTML page to XHTML with Regex.
    /// The original file is saved with an .orig extension.
    /// <para>If the file has an XHTML DOCTYPE, it is not changed.</para>
    /// <para>By default it runs on .aspx extension-
    /// change with the <see cref="ConvertWithRegex.FileExtension"/> property.</para>
    /// <para>It can be run by directory (recursively) <see cref="ConvertWithRegex.ConvertDirectory"/>,
    /// or by individual file/stream <see cref="ConvertWithRegex.Load(string)"/></para>
    /// <para>The XHTML conversion adds a XHTML transitional DOCTYPE, converts element
    /// and attribute names to lowercase (not attribute values).</para>
    /// </summary>
    /// <remarks>
    /// <para>Singleton tags like br, br and input are automatically closed
    /// (it does not automatically close other tags).
    /// Unquoted attribute values are enclosed in quotes.
    /// Attributes with no values get values (selected="selected")
    /// Embedded server controls (in &lt;% /%&gt;) and asp server controls (&lt;asp:TextBox/&gt;)
    /// are ignored (and so is html inside them).
    /// The script attribute "language" is removed.
    /// </para>
    /// <para>NB the regex assumes moderately well-formed HTML, and will probably skip certain markup.
    /// It may wrongly change certain things it shouldn't-
    /// hence the original source is saved with the .orig extension.</para>
    /// </remarks>
    /// <example>
    ///    //Folder.Text contains the directory
    ///    ConvertWithRegex c = new ConvertWithRegex();
    ///    c.ConvertDirectory(Folder.Text);
    ///    //show the last file converted in a TextBox
    ///    textBox1.Text = c.LastHtml;
    /// </example>
    public class ConvertWithRegex
    {
        /// <summary>
        /// String holding the html
        /// </summary>
        protected string _fileContents;

        //this extension is used to rename the original source
        private const string originalExtension = ".orig";
        private bool _changed = false;

        #region public ctor and run methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConvertWithRegex()
        {
            _FileExtension = "aspx";
        }

        /// <summary>
        /// Convert a directory (recursive).
        /// </summary>
        /// <param name="directory"></param>
        public void ConvertDirectory(string directory)
        {
            if (String.IsNullOrEmpty(directory))
                throw new ArgumentNullException(directory);

            if (!Directory.Exists(directory))
                throw new ArgumentException(directory);

            List<string> files = GetFileList(directory);
            foreach (string file in files)
            {
                Load(file);
                Save(file);
            }
        }

        #region Load Public Methods

        /// <summary>
        /// Load an individual file.
        /// Extension should agree with <see cref="ConvertWithRegex.FileExtension"/> (default .aspx).
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(path);

            if (!File.Exists(path))
                throw new ArgumentException(path);

            using (StreamReader sr = new StreamReader(path))
            {
                Load(sr);
            }
        }


        /// <summary>
        /// Load html from a stream
        /// </summary>
        /// <param name="sm"></param>
        public void Load(Stream sm)
        {
            if (sm == null)
                throw new ArgumentNullException("sm");
            using (StreamReader sr = new StreamReader(sm))
            {
                Load(sr);
            }
        }

        /// <summary>
        /// Load html from a TextReader
        /// </summary>
        /// <param name="reader"></param>
        public void Load(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            _fileContents = reader.ReadToEnd();

            if (!_fileContents.Contains(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML"))
            {
                //don't process if already xhtml
                Process();
            }
        }

        /// <summary>
        /// Load html from a string
        /// </summary>
        /// <param name="html"></param>
        public void LoadHtml(string html)
        {
            if (html == null)
                throw new ArgumentNullException("html");

            using (StringReader sr = new StringReader(html))
            {
                Load(sr);
            }
        }

        #endregion

        #region Save Public Methods

        /// <summary>
        /// Save output to a file path (NB: only if changed). Automatically saves original file with .orig extension.
        /// </summary>
        /// <param name="file"></param>
        public void Save(string file)
        {
            if (_fileContents == null)
                throw new ApplicationException("Have not loaded yet");
            //don't change it if nothing different
            if (_changed)
            {
                string origfile = Path.ChangeExtension(file, originalExtension);
                //only create the origin file once. if this is run twice, leave it
                if (!File.Exists(origfile))
                    File.Move(file, origfile);
                using (StreamWriter writer = new StreamWriter(file))
                {
                    Save(writer);
                }
            }
        }

        /// <summary>
        /// Save output to a Stream
        /// </summary>
        /// <param name="sm"></param>
        public void Save(Stream sm)
        {
            if (sm == null)
                throw new ArgumentNullException("sm");
            using (StreamWriter sw = new StreamWriter(sm))
            {
                Save(sw);
            }
        }

        /// <summary>
        /// Save output to a TextWriter
        /// </summary>
        /// <param name="writer"></param>
        public void Save(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (!String.IsNullOrEmpty(_fileContents))
                writer.Write(_fileContents);
        }

        /// <summary>
        /// Saves output to a string
        /// </summary>
        /// <returns></returns>
        public string SaveToString()
        {
            return _fileContents;
        }

        #endregion

        #endregion

        /// <summary>
        /// Runs the conversion. Can be overridden.
        /// </summary>
        protected virtual void Process()
        {
            string orig = _fileContents;
            _changed = false;
            LowerCase();
            FixHeader();
            CleanObsolete();
            if (orig != _fileContents)
                _changed = true;
            _LastFileHtml = _fileContents;
        }

        private void CleanObsolete()
        {
            //language='JavaScript' is obsolete
            Regex regex = new Regex(@"\s+language\s*=\s*[""']javascript[""']",
                                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            _fileContents = regex.Replace(_fileContents, "");
            //you could change <FONT> to <span> plus SIZE/FACE etc
        }

        /// <summary>
        /// Lowercases the elements and attributes, and cleans attributes
        /// </summary>
        private void LowerCase()
        {
            //this regular expression should get attributes as well (not http-equiv as an attribute though)
            //regex from http://haacked.com/archive/2005/04/22/Matching_HTML_With_Regex.aspx
            //fixed with allowing for server blocks (note assumes fairly well formed)
            Regex regex =
                new Regex(
                    @"</?(?<tag>\w+)((?<attpair>(\s+(?<attribute>[A-Za-z0-9-]+)(?<equals>\s*=\s*(?:(?<quote2value>"".*?"")|<%.*?%>|(?<quote1value>'.*?')|(?<nakedvalue>[^'"">\s]+)))?)|(\s*<%.*?%>))+\s*|\s*|\s*<%.*?%>\s*)/?>",
                    RegexOptions.Singleline);
            // multiline affects ^$ only- we want \s to include \n
            _fileContents = regex.Replace(_fileContents, new MatchEvaluator(CheckTags));
        }

        private static string CheckTags(Match m)
        {
            string s = m.Value;

            //check the tag for lowercase
            string v = m.Groups["tag"].Value;
            Debug.WriteLine("GroupTag " + v);
            int i = m.Groups["tag"].Index - m.Index;
            if (v.ToLower() != v)
                s = s.Remove(i, v.Length).Insert(i, v.ToLower());

            //check the attributes for lowercase
            foreach (Capture c in m.Groups["attribute"].Captures)
            {
                Debug.WriteLine("Capture " + c.Value);
                v = c.Value;
                i = c.Index - m.Index;
                if (v.ToLower() != v)
                    s = s.Remove(i, v.Length).Insert(i, v.ToLower());
            }

            int offset = 0; //track the offset as we may be inserting quotes
            //check for attributes without quotes
            foreach (Capture c in m.Groups["nakedvalue"].Captures)
            {
                Debug.WriteLine("Naked attribute " + c.Value);
                v = c.Value;
                i = c.Index - m.Index + offset;
                s = s.Insert(i, "\"").Insert(i + v.Length + 1, "\"");
                offset = offset + 2;
            }

            foreach (Capture c in m.Groups["attpair"].Captures)
            {
                v = c.Value; //should have been lowercased
                //the previous insertions will mean the indexes could be crap, so recalculate this
                i = c.Index - m.Index;
                if (v.IndexOf("=") == -1)
                {
                    Debug.WriteLine("Attribute with no value " + c.Value);
                    //create the value (same name as attribute, trimmed and lowercase, in quotes)
                    v = v.ToLower(); //should be lowercase already
                    i = s.IndexOf(v, i) + v.Length; //refind the attribute start and skip to the end
                    string value = "=\"" + v.ToLower().Trim() + "\"";
                    //insert it after the value (and reset the offset)
                    s = s.Insert(i, value);
                }
            }

            //finally close singletons manually
            s = FixSingletons(s);
            return s;
        }


        private static string FixSingletons(string s)
        {
            bool needsClose = false;
            if (s.StartsWith("<hr")) needsClose = true;
            if (s.StartsWith("<br")) needsClose = true;
            if (s.StartsWith("<meta")) needsClose = true;
            if (s.StartsWith("<link")) needsClose = true;
            if (s.StartsWith("<img")) needsClose = true;
            if (s.StartsWith("<input")) needsClose = true;
            if (needsClose && !s.EndsWith("/>"))
                s = s.Insert(s.Length - 1, "/");

            return s;
        }

        #region FixHeader

        /// <summary>
        /// Replaces or adds a DOCTYPE with the XHTML transitional doctype
        /// </summary>
        protected void FixHeader()
        {
            const string doctype =
                @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">";
            //replace or add the doctype
            if (_fileContents.Contains(@"<!DOCTYPE"))
                _fileContents = ReplaceTagStarting(_fileContents, "<!DOCTYPE", doctype);
            else
                _fileContents = ReplaceTagStarting(_fileContents, "<html>", doctype + Environment.NewLine + "<html>");
            //ensure it has the namespace
            if (!_fileContents.Contains("<html xmlns"))
                _fileContents = _fileContents.Replace("<html", @"<html xmlns=""http://www.w3.org/1999/xhtml""");
        }

        /// <summary>
        /// Utility to manually change a tag to another one, given just the start of the tag
        /// </summary>
        /// <param name="s">the full string</param>
        /// <param name="find">the substring to find (the start of the tag)</param>
        /// <param name="replace">the substring to replace</param>
        /// <returns></returns>
        private string ReplaceTagStarting(string s, string find, string replace)
        {
            int start = s.IndexOf(find, StringComparison.CurrentCultureIgnoreCase);
            if (start == -1)
                return s;

            int end = FindEndOfTag(s, start);
            Debug.WriteLine("Replace [" + start + " to " + end + "] =" +
                            s.Substring(start + 1, end - start) +
                            " \nreplaced by " + replace);
            s = s.Substring(0, start) + replace + s.Substring(end + 1);
            return s;
        }

        private int FindEndOfTag(string htmlFragment, int start)
        {
            //assuming we're at the start of the tag just find the next >
            int end = htmlFragment.IndexOf('>', start);
            //if there are nested asp tags, ignore 'em
            if (end != -1 && htmlFragment.Substring(end - 1, 2) == "%>")
                //add one to skip to next endbrace
                end = FindEndOfTag(htmlFragment, end + 1);
            if (end != -1 && htmlFragment.Substring(end - 1, 2) == "<>")
                //add one to skip to next endbrace
                end = FindEndOfTag(htmlFragment, end + 1);
            return end;
        }

        #endregion

        #region Properties

        private string _FileExtension;

        /// <summary>
        /// holds the last converted html
        /// </summary>
        protected string _LastFileHtml;

        /// <summary>
        /// The last Html that was converted
        /// </summary>
        public string LastHtml
        {
            get { return _LastFileHtml; }
            set { _LastFileHtml = value; }
        }

        ///<summary>
        ///FileExtension
        ///</summary>
        public string FileExtension
        {
            get { return _FileExtension; }
            set { _FileExtension = value; }
        }

        #endregion

        #region Get File List

        private List<string> GetFileList(string directory)
        {
            List<string> files = new List<string>();
            DirSearch(directory, ref files);
            return files;
        }

        //Recursive file search from Visual Studio code snippet
        private void DirSearch(string sDir, ref List<string> files)
        {
            FindFiles(files, sDir);
            foreach (string d in Directory.GetDirectories(sDir))
            {
                DirSearch(d, ref files);
            }
        }

        private void FindFiles(ICollection<string> files, string d)
        {
            foreach (string f in Directory.GetFiles(d, "*." + _FileExtension))
            {
                files.Add(f);
            }
        }

        #endregion
    }
}
