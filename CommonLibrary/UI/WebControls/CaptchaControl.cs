using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.UI;
using System.Web;
using System.Drawing;
using System.Web.UI.WebControls;
using CommonLibrary.Services.Localization;
using System.ComponentModel;
using CommonLibrary.Entities.Portal;
using System.Drawing.Drawing2D;
using System.Web.Security;
using CommonLibrary.Services.Exceptions;

namespace CommonLibrary.UI.WebControls
{
    [ToolboxData("<{0}:CaptchaControl Runat=\"server\" CaptchaHeight=\"100px\" CaptchaWidth=\"300px\" />")]
    public class CaptchaControl : WebControl, INamingContainer, IPostBackDataHandler
    {
        private System.Web.UI.WebControls.Image _image;
        public event ServerValidateEventHandler UserValidated;
        private const int EXPIRATION_DEFAULT = 120;
        private const int LENGTH_DEFAULT = 6;
        private const string RENDERURL_DEFAULT = "ImageChallenge.captcha.aspx";
        private const string CHARS_DEFAULT = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        internal const string KEY = "captcha";
        private bool _Authenticated;
        private Color _BackGroundColor = Color.Transparent;
        private string _BackGroundImage = "";
        private string _CaptchaChars = CHARS_DEFAULT;
        private Unit _CaptchaHeight = Unit.Pixel(100);
        private int _CaptchaLength = LENGTH_DEFAULT;
        private string _CaptchaText;
        private Unit _CaptchaWidth = Unit.Pixel(300);
        private string _ErrorMessage;
        private Style _ErrorStyle = new Style();
        private int _Expiration = EXPIRATION_DEFAULT;
        private bool _IsValid = false;
        private string _RenderUrl = RENDERURL_DEFAULT;
        private string _Text;
        private Style _TextBoxStyle = new Style();
        private string _UserText = "";
        private static string[] _FontFamilies = {
			"Arial",
			"Comic Sans MS",
			"Courier New",
			"Georgia",
			"Lucida Console",
			"MS Sans Serif",
			"Stencil",
			"Tahoma",
			"Times New Roman",
			"Trebuchet MS",
			"Verdana"
		};
        private static Random _Rand = new Random();
        private static string _Separator = ":-:";
        private bool IsDesignMode
        {
            get { return HttpContext.Current == null; }
        }
        //public CaptchaControl()
        //{
        //    _ErrorMessage = Localization.GetString("InvalidCaptcha", Localization.SharedResourceFile);
        //    _Text = Localization.GetString("CaptchaText.Text", Localization.SharedResourceFile);
        //}
        [Category("Appearance"), Description("The Background Color to use for the Captcha Image.")]
        public Color BackGroundColor
        {
            get { return _BackGroundColor; }
            set { _BackGroundColor = value; }
        }
        [Category("Appearance"), Description("A Background Image to use for the Captcha Image.")]
        public string BackGroundImage
        {
            get { return _BackGroundImage; }
            set { _BackGroundImage = value; }
        }
        [Category("Behavior"), DefaultValue(CHARS_DEFAULT), Description("Characters used to render CAPTCHA text. A character will be picked randomly from the string.")]
        public string CaptchaChars
        {
            get { return _CaptchaChars; }
            set { _CaptchaChars = value; }
        }
        [Category("Appearance"), Description("Height of Captcha Image.")]
        public Unit CaptchaHeight
        {
            get { return _CaptchaHeight; }
            set { _CaptchaHeight = value; }
        }
        [Category("Behavior"), DefaultValue(LENGTH_DEFAULT), Description("Number of CaptchaChars used in the CAPTCHA text")]
        public int CaptchaLength
        {
            get { return _CaptchaLength; }
            set { _CaptchaLength = value; }
        }
        [Category("Appearance"), Description("Width of Captcha Image.")]
        public Unit CaptchaWidth
        {
            get { return _CaptchaWidth; }
            set { _CaptchaWidth = value; }
        }
        [Browsable(false)]
        public override bool EnableViewState
        {
            get { return base.EnableViewState; }
            set { base.EnableViewState = value; }
        }
        [Category("Behavior"), Description("The Error Message to display if invalid."), DefaultValue("")]
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }
        [Browsable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter)), Description("Set the Style for the Error Message Control.")]
        public Style ErrorStyle
        {
            get { return _ErrorStyle; }
        }
        [Category("Behavior"), Description("The duration of time (seconds) a user has before the challenge expires."), DefaultValue(EXPIRATION_DEFAULT)]
        public int Expiration
        {
            get { return _Expiration; }
            set { _Expiration = value; }
        }
        [Category("Validation"), Description("Returns True if the user was CAPTCHA validated after a postback.")]
        public bool IsValid
        {
            get { return _IsValid; }
        }
        [Category("Behavior"), Description("The URL used to render the image to the client."), DefaultValue(RENDERURL_DEFAULT)]
        public string RenderUrl
        {
            get { return _RenderUrl; }
            set { _RenderUrl = value; }
        }
        [Category("Captcha"), DefaultValue("Enter the code shown above:"), Description("Instructional text displayed next to CAPTCHA image.")]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }
        [Browsable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter)), Description("Set the Style for the Text Box Control.")]
        public Style TextBoxStyle
        {
            get { return _TextBoxStyle; }
        }
        private string GetUrl()
        {
            string url = ResolveUrl(RenderUrl);
            //url += "?" + KEY + "=" + Encrypt(EncodeTicket(), DateTime.Now.AddSeconds(Expiration));
            //PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            //url += "&alias=" + _portalSettings.PortalAlias.HTTPAlias;
            return url;
        }
        private string EncodeTicket()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(CaptchaWidth.Value.ToString());
            sb.Append(_Separator + CaptchaHeight.Value.ToString());
            sb.Append(_Separator + _CaptchaText);
            sb.Append(_Separator + BackGroundImage);
            return sb.ToString();
        }
        private static Bitmap CreateImage(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g;
            Rectangle rect = new Rectangle(0, 0, width, height);
            RectangleF rectF = new RectangleF(0, 0, width, height);
            g = Graphics.FromImage(bmp);
            Brush b = new LinearGradientBrush(rect, Color.FromArgb(_Rand.Next(192), _Rand.Next(192), _Rand.Next(192)), Color.FromArgb(_Rand.Next(192), _Rand.Next(192), _Rand.Next(192)), Convert.ToSingle(_Rand.NextDouble()) * 360, false);
            g.FillRectangle(b, rectF);
            if (_Rand.Next(2) == 1)
            {
                DistortImage(ref bmp, _Rand.Next(5, 10));
            }
            else
            {
                DistortImage(ref bmp, -_Rand.Next(5, 10));
            }
            return bmp;
        }
        private static GraphicsPath CreateText(string text, int width, int height, Graphics g)
        {
            GraphicsPath textPath = new GraphicsPath();
            FontFamily ff = GetFont();
            int emSize = Convert.ToInt32(width * 2 / text.Length);
            Font f = null;
            try
            {
                SizeF measured = new SizeF(0, 0);
                SizeF workingSize = new SizeF(width, height);
                while ((emSize > 2))
                {
                    f = new Font(ff, emSize);
                    measured = g.MeasureString(text, f);
                    if (!(measured.Width > workingSize.Width || measured.Height > workingSize.Height))
                    {
                        break;
                    }
                    f.Dispose();
                    emSize -= 2;
                }
                emSize += 8;
                f = new Font(ff, emSize);
                StringFormat fmt = new StringFormat();
                fmt.Alignment = StringAlignment.Center;
                fmt.LineAlignment = StringAlignment.Center;
                textPath.AddString(text, f.FontFamily, Convert.ToInt32(f.Style), f.Size, new RectangleF(0, 0, width, height), fmt);
                WarpText(ref textPath, new Rectangle(0, 0, width, height));
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                f.Dispose();
            }
            return textPath;
        }
        private static string Decrypt(string encryptedContent)
        {
            string decryptedText = string.Empty;
            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encryptedContent);
                if ((!ticket.Expired))
                {
                    decryptedText = ticket.UserData;
                }
            }
            catch (ArgumentException exc)
            {
                exc.ToString();
            }
            return decryptedText;
        }
        private static void DistortImage(ref Bitmap b, double distortion)
        {
            int width = b.Width;
            int height = b.Height;
            Bitmap copy = (Bitmap)b.Clone();
            for (int y = 0; y <= height - 1; y++)
            {
                for (int x = 0; x <= width - 1; x++)
                {
                    int newX = Convert.ToInt32(x + (distortion * Math.Sin(Math.PI * y / 64.0)));
                    int newY = Convert.ToInt32(y + (distortion * Math.Cos(Math.PI * x / 64.0)));
                    if ((newX < 0 || newX >= width))
                        newX = 0;
                    if ((newY < 0 || newY >= height))
                        newY = 0;
                    b.SetPixel(x, y, copy.GetPixel(newX, newY));
                }
            }
        }
        private static string Encrypt(string content, DateTime expiration)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, HttpContext.Current.Request.UserHostAddress, DateTime.Now, expiration, false, content);
            return FormsAuthentication.Encrypt(ticket);
        }
        static internal Bitmap GenerateImage(string encryptedText)
        {
            string encodedText = Decrypt(encryptedText);
            Bitmap bmp = null;
            string[] Settings = System.Text.RegularExpressions.Regex.Split(encodedText, _Separator);
            try
            {
                int width = int.Parse(Settings[0]);
                int height = int.Parse(Settings[1]);
                string text = Settings[2];
                string backgroundImage = Settings[3];
                Graphics g;
                Brush b = new SolidBrush(Color.LightGray);
                Brush b1 = new SolidBrush(Color.Black);
                if (String.IsNullOrEmpty(backgroundImage))
                {
                    bmp = CreateImage(width, height);
                }
                else
                {
                    bmp = (Bitmap)Bitmap.FromFile(HttpContext.Current.Request.MapPath(backgroundImage));
                }
                g = Graphics.FromImage(bmp);
                GraphicsPath textPath = CreateText(text, width, height, g);
                if (String.IsNullOrEmpty(backgroundImage))
                {
                    g.FillPath(b, textPath);
                }
                else
                {
                    g.FillPath(b1, textPath);
                }
            }
            catch (Exception exc)
            {
               Exceptions.LogException(exc);
            }
            return bmp;
        }
        private static FontFamily GetFont()
        {
            FontFamily _font = null;
            while (_font == null)
            {
                try
                {
                    _font = new FontFamily(_FontFamilies[_Rand.Next(_FontFamilies.Length)]);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    _font = null;
                }
            }
            return _font;
        }
        private static PointF RandomPoint(int xmin, int xmax, int ymin, int ymax)
        {
            return new PointF(_Rand.Next(xmin, xmax), _Rand.Next(ymin, ymax));
        }
        private static void WarpText(ref GraphicsPath textPath, Rectangle rect)
        {
            int intWarpDivisor;
            RectangleF rectF = new RectangleF(0, 0, rect.Width, rect.Height);
            intWarpDivisor = _Rand.Next(4, 8);
            int intHrange = Convert.ToInt32(rect.Height / intWarpDivisor);
            int intWrange = Convert.ToInt32(rect.Width / intWarpDivisor);
            PointF p1 = RandomPoint(0, intWrange, 0, intHrange);
            PointF p2 = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p1.X)), rect.Width, 0, intHrange);
            PointF p3 = RandomPoint(0, intWrange, rect.Height - (intHrange - Convert.ToInt32(p1.Y)), rect.Height);
            PointF p4 = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p3.X)), rect.Width, rect.Height - (intHrange - Convert.ToInt32(p2.Y)), rect.Height);
            PointF[] points = new PointF[] {
				p1,
				p2,
				p3,
				p4
			};
            Matrix m = new Matrix();
            m.Translate(0, 0);
            textPath.Warp(points, rectF, m, WarpMode.Perspective, 0);
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            if ((this.CaptchaWidth.IsEmpty || this.CaptchaWidth.Type != UnitType.Pixel || this.CaptchaHeight.IsEmpty || this.CaptchaHeight.Type != UnitType.Pixel))
            {
                throw new InvalidOperationException("Must specify size of control in pixels.");
            }
            _image = new System.Web.UI.WebControls.Image();
            _image.BorderColor = this.BorderColor;
            _image.BorderStyle = this.BorderStyle;
            _image.BorderWidth = this.BorderWidth;
            _image.ToolTip = this.ToolTip;
            _image.EnableViewState = false;
            Controls.Add(_image);
        }
        protected virtual string GetNextCaptcha()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Random _rand = new Random();
            int n;
            int intMaxLength = CaptchaChars.Length;
            for (n = 0; n <= CaptchaLength - 1; n++)
            {
                sb.Append(CaptchaChars.Substring(_rand.Next(intMaxLength), 1));
            }
            return sb.ToString();
        }
        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] myState = (object[])savedState;
                if (myState[0] != null)
                {
                    base.LoadViewState(myState[0]);
                }
                if (myState[1] != null)
                {
                    _CaptchaText = Convert.ToString(myState[1]);
                }
            }
        }
        protected override void OnPreRender(EventArgs e)
        {

            _CaptchaText = GetNextCaptcha();

            Page.RegisterRequiresViewStateEncryption();
            base.OnPreRender(e);
        }
        //protected override void Render(HtmlTextWriter writer)
        //{
        //    ControlStyle.AddAttributesToRender(writer);
        //    writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //    writer.AddAttribute(HtmlTextWriterAttribute.Src, GetUrl());
        //    writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
        //    if (!String.IsNullOrEmpty(ToolTip))
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Alt, ToolTip);
        //    }
        //    else
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Alt, Localization.GetString("CaptchaAlt.Text", Localization.SharedResourceFile));
        //    }
        //    writer.RenderBeginTag(HtmlTextWriterTag.Img);
        //    writer.RenderEndTag();
        //    if (!String.IsNullOrEmpty(Text))
        //    {
        //        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        writer.Write(Text);
        //        writer.RenderEndTag();
        //    }
        //    TextBoxStyle.AddAttributesToRender(writer);
        //    writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
        //    writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:" + Width.ToString());
        //    writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, _CaptchaText.Length.ToString());
        //    writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
        //    if (!String.IsNullOrEmpty(AccessKey))
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, AccessKey);
        //    }
        //    if (!Enabled)
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
        //    }
        //    if (TabIndex > 0)
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, TabIndex.ToString());
        //    }
        //    if (_UserText == _CaptchaText)
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Value, _UserText);
        //    }
        //    else
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
        //    }
        //    writer.RenderBeginTag(HtmlTextWriterTag.Input);
        //    writer.RenderEndTag();
        //    if (!IsValid && Page.IsPostBack && !string.IsNullOrEmpty(_UserText))
        //    {
        //        ErrorStyle.AddAttributesToRender(writer);
        //        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        writer.Write(ErrorMessage);
        //        writer.RenderEndTag();
        //    }
        //    writer.RenderEndTag();
        //}
        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] allStates = new object[2];
            allStates[0] = baseState;
            if (string.IsNullOrEmpty(_CaptchaText))
            {
                _CaptchaText = GetNextCaptcha();
            }
            allStates[1] = _CaptchaText;
            return allStates;
        }
        public bool Validate(string userData)
        {
            if (string.Compare(userData, this._CaptchaText, false, CultureInfo.InvariantCulture) == 0)
            {
                _IsValid = true;
            }
            else
            {
                _IsValid = false;
            }
            if (UserValidated != null)
            {
                UserValidated(this, new ServerValidateEventArgs(_CaptchaText, _IsValid));
            }
            return _IsValid;
        }
        public virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            _UserText = postCollection[postDataKey];
            Validate(_UserText);
            if (!_IsValid && !string.IsNullOrEmpty(_UserText))
            {
                _CaptchaText = GetNextCaptcha();
            }
            return false;
        }
        public void RaisePostDataChangedEvent()
        {
        }
    }
}
