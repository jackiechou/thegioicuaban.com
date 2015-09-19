using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.GridView
{
    public class GridViewOption
    {
        private bool showEditButton = true;
        private bool showDeleteButton = true;

        private string editButtonText = "Edit";
        private string deleteButtonText = "Delete";

        private string editAction = "Edit";
        private string deleteAction = "Delete";

        private string[] columns;

        public string[] Columns
        {
            get { return columns; }
            set { columns = value; }
        }


        public bool ShowEditButton
        {
            get { return showEditButton; }
            set { showEditButton = value; }
        }

        public bool ShowDeleteButton
        {
            get { return showDeleteButton; }
            set { showDeleteButton = value; }
        }


        public string EditButtonText
        {
            get { return editButtonText; }
            set { editButtonText = value; }
        }

        public string DeleteButtonText
        {
            get { return deleteButtonText; }
            set { deleteButtonText = value; }
        }


        public string EditAction
        {
            get { return editAction; }
            set { editAction = value; }
        }

        public string DeleteAction
        {
            get { return deleteAction; }
            set { deleteAction = value; }
        }
    }
}
