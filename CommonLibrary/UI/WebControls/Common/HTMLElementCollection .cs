﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.UI.WebControls.Common
{
    public class HTMLElementCollection : CollectionBase
    {

        public HTMLElementCollection()
            : base()
        {
        }

        public HTMLElement this[int index]
        {
            get { return (HTMLElement)this.List[index]; }
            set { this.List[index] = value; }
        }

        public int Add(HTMLElement value)
        {
            int index = this.List.Add(value);
            return index;
        }

        public void AddRange(HTMLElement[] value)
        {
            for (int i = 0; i <= value.Length - 1; i++)
            {
                Add(value[i]);
            }
        }

        public int IndexOf(HTMLElement value)
        {
            return this.List.IndexOf(value);
        }

        public bool Contains(HTMLElement value)
        {
            return this.List.Contains(value);
        }

        public void Insert(int index, HTMLElement value)
        {
            List.Insert(index, value);
        }

        public void Remove(HTMLElement value)
        {
            List.Remove(value);
        }
        //Remove

        public void CopyTo(HTMLElement[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public override string ToString()
        {
            System.Text.StringBuilder objSB = new System.Text.StringBuilder();
            foreach (HTMLElement objElement in List)
            {
                objSB.Append(objElement.Raw);
            }
            return objSB.ToString();
        }

        public string ToJSON()
        {
            return ToJSON("");
        }

        public string ToJSON(string KeyAttribute)
        {
            string strKey;
            System.Text.StringBuilder objSB = new System.Text.StringBuilder();
            int iNotFoundCount = 0;
            objSB.Append("{");
            foreach (HTMLElement objElement in List)
            {
                if (objSB.Length > 1) objSB.Append(",");
                if (objElement.Attributes.Contains(KeyAttribute))
                {
                    strKey = (string)objElement.Attributes[KeyAttribute];
                }
                else
                {
                    iNotFoundCount += 1;
                    strKey = "__" + iNotFoundCount.ToString();
                }
                objSB.Append(strKey + ":" + objElement.ToJSON());
            }
            objSB.Append("}");

            return objSB.ToString();
        }

    }

}
