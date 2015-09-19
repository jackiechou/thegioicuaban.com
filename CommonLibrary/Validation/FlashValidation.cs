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
    class FlashValidation
    {      
        private bool _IsFlash = false;            
        private bool _ValidFileSize = false;      
        private string _FileType = string.Empty;
        private int _FileSize = 0;
        //private int _MaxWidth = 600;
        //private int _MaxHeight = 600;

        public bool IsFlash
        {
            get
            {
                return _IsFlash;
            }
            set
            {
                _IsFlash = value;
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
      
        public bool ValidateFileIsFlash(string fileType)
        {
            this._FileType = fileType;

            switch (fileType)
            {
                case ("application/x-shockwave-flash"): IsFlash = true; break;
                default: IsFlash = false; break;
            }
            return IsFlash;
        }

        public bool ValidateUserFlashSize(int maxFileSize, int fileSize)
        {
            this._FileSize = fileSize;

            if (maxFileSize > fileSize)
                return ValidFileSize = false;

            return ValidFileSize;
        }              

    }
}
