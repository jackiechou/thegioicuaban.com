using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls
{
    public class PropertyEditorEventArgs : System.EventArgs
    {
        private bool mChanged;
        private int mIndex;
        private object mKey;
        private string mName;
        private object mOldValue;
        private string mStringValue;
        private object mValue;
        public PropertyEditorEventArgs(string name)
            : this(name, null, null)
        {
        }
        public PropertyEditorEventArgs(string name, object newValue, object oldValue)
        {
            mName = name;
            mValue = newValue;
            mOldValue = oldValue;
        }
        public bool Changed
        {
            get { return mChanged; }
            set { mChanged = value; }
        }
        public int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }
        public object Key
        {
            get { return mKey; }
            set { mKey = value; }
        }
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        public object OldValue
        {
            get { return mOldValue; }
            set { mOldValue = value; }
        }
        public string StringValue
        {
            get { return mStringValue; }
            set { mStringValue = value; }
        }
        public object Value
        {
            get { return mValue; }
            set { mValue = value; }
        }
    }
}
