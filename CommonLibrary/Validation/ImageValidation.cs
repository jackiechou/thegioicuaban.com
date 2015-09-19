using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Library.Validation
{
    class ImageValidation
    {
        private bool _IsImage = false;      		
		private bool _ValidFileSize = false;
		private bool _ValidImageDimension = false;
		private string _FileType = string.Empty;
		private int _FileSize = 0;
		private int _MaxWidth = 600;
		private int _MaxHeight = 600;       

		public bool IsImage
		{
			get
			{
				return _IsImage;
			}
			set
			{
				_IsImage = value;
			}
		}

		public bool ValidFileSize
		{
			get
			{
				return _ValidFileSize;
			}
			set
			{
				_ValidFileSize = value;
			}
		}

		public string FileType
		{
			get
			{
				return _FileType;
			}
			set
			{
				_FileType = value;
			}
		}

		public bool ValidImageDimension
		{
			get
			{
				return _ValidImageDimension;
			}
			set
			{
				_ValidImageDimension = value;
			}
		}

        public ImageValidation()
		{
			// Default construcor
		}

		public bool ValidateFileIsImage(string fileType)
		{  
			this._FileType = fileType;

			switch (fileType)
			{
                case "image/gif": IsImage = true; break;
                case "image/jpg": IsImage = true; break;
                case "image/jpeg": IsImage = true; break;
                case "image/pjpeg": IsImage = true; break;
                case "image/bmp": IsImage = true; break;
                case "image/png": IsImage = true; break;
                case "image/tiff": IsImage = true; break;                             
                default: IsImage = false; break;				
			}			
			return IsImage;
		}
       
		public bool ValidateUserImageSize(int maxFileSize, int fileSize)
		{
			this._FileSize = fileSize;

			if(maxFileSize > fileSize)
				return ValidFileSize = false;

			return ValidFileSize;
		}    

		public bool ValidateUserImageDimensions(HttpPostedFile file)
		{
			using(Bitmap bitmap = new Bitmap(file.InputStream, false))
			{
				if(bitmap.Width < _MaxWidth && bitmap.Height < _MaxHeight)
					_ValidImageDimension = true;

				return ValidImageDimension;
			}
		}
    }
}
