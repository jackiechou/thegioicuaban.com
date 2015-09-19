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
    class VideoValidation
    {
        private bool _IsVideo = false;
        private bool _ValidFileSize = false;
        private string _FileType = string.Empty;
        private int _FileSize = 0;
        //private int _MaxWidth = 600;
        //private int _MaxHeight = 600;

        public bool IsVideo
        {
            get
            {
                return _IsVideo;
            }
            set
            {
                _IsVideo = value;
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

        public bool ValidateFileIsVideo(string fileType)
        {
            this._FileType = fileType;

            switch (fileType)
            {
                case ("video/avi"): IsVideo = true; break;
                case ("video/flv"): IsVideo = true; break;
                case ("video/mp4"): IsVideo = true; break;
                case ("video/mpeg"): IsVideo = true; break;
                case ("image/wav"): IsVideo = true; break;
                case ("application/x-shockwave-flash"): IsVideo = true; break;
                default: IsVideo = false; break;
            }
            return IsVideo;
        }

        public bool ValidateUserVideoSize(int maxFileSize, int fileSize)
        {
            this._FileSize = fileSize;

            if (maxFileSize > fileSize)
                return ValidFileSize = false;

            return ValidFileSize;
        }      

    }
}
