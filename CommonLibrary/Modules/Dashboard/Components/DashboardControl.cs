using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Localization;

namespace CommonLibrary.Modules.Dashboard.Components
{
    [Serializable()]
    public class DashboardControl : IComparable
    {
        private string _ControllerClass;
        private int _DashboardControlID;
        private string _DashboardControlKey;
        private string _DashboardControlLocalResources;
        private string _DashboardControlSrc;
        private bool _IsDirty;
        private bool _IsEnabled;
        private int _PackageID;
        private int _ViewOrder;
        public string ControllerClass
        {
            get { return _ControllerClass; }
            set { _ControllerClass = value; }
        }
        public int DashboardControlID
        {
            get { return _DashboardControlID; }
            set { _DashboardControlID = value; }
        }
        public string DashboardControlKey
        {
            get { return _DashboardControlKey; }
            set { _DashboardControlKey = value; }
        }
        public string DashboardControlLocalResources
        {
            get { return _DashboardControlLocalResources; }
            set { _DashboardControlLocalResources = value; }
        }
        public string DashboardControlSrc
        {
            get { return _DashboardControlSrc; }
            set { _DashboardControlSrc = value; }
        }
        public bool IsDirty
        {
            get { return _IsDirty; }
        }
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (_IsEnabled != value)
                    _IsDirty = true;
                _IsEnabled = value;
            }
        }
        public string LocalizedTitle
        {
            get { return Localization.GetString(DashboardControlKey + ".Title", "~/" + DashboardControlLocalResources); }
        }
        public int PackageID
        {
            get { return _PackageID; }
            set { _PackageID = value; }
        }
        public int ViewOrder
        {
            get { return _ViewOrder; }
            set
            {
                if (_ViewOrder != value)
                    _IsDirty = true;
                _ViewOrder = value;
            }
        }
        public int CompareTo(object obj)
        {
            DashboardControl dashboardControl = obj as DashboardControl;
            if (dashboardControl == null)
            {
                throw new ArgumentException("object is not a DashboardControl");
            }
            return this.ViewOrder.CompareTo(dashboardControl.ViewOrder);
        }
    }
}
