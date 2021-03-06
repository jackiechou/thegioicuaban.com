﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CommonLibrary.Entities.Tabs
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="5eagle_VBA")]
	public partial class TabDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void Insertaspnet_Tab(aspnet_Tab instance);
    partial void Updateaspnet_Tab(aspnet_Tab instance);
    partial void Deleteaspnet_Tab(aspnet_Tab instance);
    #endregion
		
		public TabDataContext() : 
				base(global::CommonLibrary.Properties.Settings.Default._5eagle_VBAConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public TabDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TabDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TabDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TabDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<aspnet_Tab> aspnet_Tabs
		{
			get
			{
				return this.GetTable<aspnet_Tab>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.aspnet_Tabs")]
	public partial class aspnet_Tab : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _TabId;
		
		private int _ContentItemId;
		
		private System.Nullable<int> _PortalId;
		
		private int _TabOrder;
		
		private string _TabName;
		
		private System.Nullable<int> _ParentId;
		
		private System.Nullable<int> _Level;
		
		private string _Lineage;
		
		private System.Nullable<bool> _DisableLink;
		
		private string _Title;
		
		private bool _DisplayTitle;
		
		private string _Description;
		
		private string _Keywords;
		
		private string _Url;
		
		private string _TabPath;
		
		private System.Nullable<int> _RouteId;
		
		private System.Nullable<System.DateTime> _StartDate;
		
		private System.Nullable<System.DateTime> _EndDate;
		
		private string _PageHeadText;
		
		private string _PageFooterText;
		
		private string _PageControlBar;
		
		private bool _IsDeleted;
		
		private bool _IsSecure;
		
		private bool _IsVisible;
		
		private bool _PermanentRedirect;
		
		private double _SiteMapPriority;
		
		private string _CssClass;
		
		private string _IconFile;
		
		private string _IconFileLarge;
		
		private string _CultureCode;
		
		private System.Nullable<System.Guid> _CreatedByUserId;
		
		private System.Nullable<System.DateTime> _CreatedOnDate;
		
		private System.Nullable<System.Guid> _LastModifiedByUserId;
		
		private System.Nullable<System.DateTime> _LastModifiedOnDate;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnTabIdChanging(int value);
    partial void OnTabIdChanged();
    partial void OnContentItemIdChanging(int value);
    partial void OnContentItemIdChanged();
    partial void OnPortalIdChanging(System.Nullable<int> value);
    partial void OnPortalIdChanged();
    partial void OnTabOrderChanging(int value);
    partial void OnTabOrderChanged();
    partial void OnTabNameChanging(string value);
    partial void OnTabNameChanged();
    partial void OnParentIdChanging(System.Nullable<int> value);
    partial void OnParentIdChanged();
    partial void OnLevelChanging(System.Nullable<int> value);
    partial void OnLevelChanged();
    partial void OnLineageChanging(string value);
    partial void OnLineageChanged();
    partial void OnDisableLinkChanging(System.Nullable<bool> value);
    partial void OnDisableLinkChanged();
    partial void OnTitleChanging(string value);
    partial void OnTitleChanged();
    partial void OnDisplayTitleChanging(bool value);
    partial void OnDisplayTitleChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnKeywordsChanging(string value);
    partial void OnKeywordsChanged();
    partial void OnUrlChanging(string value);
    partial void OnUrlChanged();
    partial void OnTabPathChanging(string value);
    partial void OnTabPathChanged();
    partial void OnRouteIdChanging(System.Nullable<int> value);
    partial void OnRouteIdChanged();
    partial void OnStartDateChanging(System.Nullable<System.DateTime> value);
    partial void OnStartDateChanged();
    partial void OnEndDateChanging(System.Nullable<System.DateTime> value);
    partial void OnEndDateChanged();
    partial void OnPageHeadTextChanging(string value);
    partial void OnPageHeadTextChanged();
    partial void OnPageFooterTextChanging(string value);
    partial void OnPageFooterTextChanged();
    partial void OnPageControlBarChanging(string value);
    partial void OnPageControlBarChanged();
    partial void OnIsDeletedChanging(bool value);
    partial void OnIsDeletedChanged();
    partial void OnIsSecureChanging(bool value);
    partial void OnIsSecureChanged();
    partial void OnIsVisibleChanging(bool value);
    partial void OnIsVisibleChanged();
    partial void OnPermanentRedirectChanging(bool value);
    partial void OnPermanentRedirectChanged();
    partial void OnSiteMapPriorityChanging(double value);
    partial void OnSiteMapPriorityChanged();
    partial void OnCssClassChanging(string value);
    partial void OnCssClassChanged();
    partial void OnIconFileChanging(string value);
    partial void OnIconFileChanged();
    partial void OnIconFileLargeChanging(string value);
    partial void OnIconFileLargeChanged();
    partial void OnCultureCodeChanging(string value);
    partial void OnCultureCodeChanged();
    partial void OnCreatedByUserIdChanging(System.Nullable<System.Guid> value);
    partial void OnCreatedByUserIdChanged();
    partial void OnCreatedOnDateChanging(System.Nullable<System.DateTime> value);
    partial void OnCreatedOnDateChanged();
    partial void OnLastModifiedByUserIdChanging(System.Nullable<System.Guid> value);
    partial void OnLastModifiedByUserIdChanged();
    partial void OnLastModifiedOnDateChanging(System.Nullable<System.DateTime> value);
    partial void OnLastModifiedOnDateChanged();
    #endregion
		
		public aspnet_Tab()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TabId", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int TabId
		{
			get
			{
				return this._TabId;
			}
			set
			{
				if ((this._TabId != value))
				{
					this.OnTabIdChanging(value);
					this.SendPropertyChanging();
					this._TabId = value;
					this.SendPropertyChanged("TabId");
					this.OnTabIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ContentItemId", DbType="Int NOT NULL")]
		public int ContentItemId
		{
			get
			{
				return this._ContentItemId;
			}
			set
			{
				if ((this._ContentItemId != value))
				{
					this.OnContentItemIdChanging(value);
					this.SendPropertyChanging();
					this._ContentItemId = value;
					this.SendPropertyChanged("ContentItemId");
					this.OnContentItemIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PortalId", DbType="Int")]
		public System.Nullable<int> PortalId
		{
			get
			{
				return this._PortalId;
			}
			set
			{
				if ((this._PortalId != value))
				{
					this.OnPortalIdChanging(value);
					this.SendPropertyChanging();
					this._PortalId = value;
					this.SendPropertyChanged("PortalId");
					this.OnPortalIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TabOrder", DbType="Int NOT NULL")]
		public int TabOrder
		{
			get
			{
				return this._TabOrder;
			}
			set
			{
				if ((this._TabOrder != value))
				{
					this.OnTabOrderChanging(value);
					this.SendPropertyChanging();
					this._TabOrder = value;
					this.SendPropertyChanged("TabOrder");
					this.OnTabOrderChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TabName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string TabName
		{
			get
			{
				return this._TabName;
			}
			set
			{
				if ((this._TabName != value))
				{
					this.OnTabNameChanging(value);
					this.SendPropertyChanging();
					this._TabName = value;
					this.SendPropertyChanged("TabName");
					this.OnTabNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ParentId", DbType="Int")]
		public System.Nullable<int> ParentId
		{
			get
			{
				return this._ParentId;
			}
			set
			{
				if ((this._ParentId != value))
				{
					this.OnParentIdChanging(value);
					this.SendPropertyChanging();
					this._ParentId = value;
					this.SendPropertyChanged("ParentId");
					this.OnParentIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Level]", Storage="_Level", DbType="Int")]
		public System.Nullable<int> Level
		{
			get
			{
				return this._Level;
			}
			set
			{
				if ((this._Level != value))
				{
					this.OnLevelChanging(value);
					this.SendPropertyChanging();
					this._Level = value;
					this.SendPropertyChanged("Level");
					this.OnLevelChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lineage", DbType="VarChar(250)")]
		public string Lineage
		{
			get
			{
				return this._Lineage;
			}
			set
			{
				if ((this._Lineage != value))
				{
					this.OnLineageChanging(value);
					this.SendPropertyChanging();
					this._Lineage = value;
					this.SendPropertyChanged("Lineage");
					this.OnLineageChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DisableLink", DbType="Bit")]
		public System.Nullable<bool> DisableLink
		{
			get
			{
				return this._DisableLink;
			}
			set
			{
				if ((this._DisableLink != value))
				{
					this.OnDisableLinkChanging(value);
					this.SendPropertyChanging();
					this._DisableLink = value;
					this.SendPropertyChanged("DisableLink");
					this.OnDisableLinkChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Title", DbType="NVarChar(200)")]
		public string Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				if ((this._Title != value))
				{
					this.OnTitleChanging(value);
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					this.OnTitleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DisplayTitle", DbType="Bit NOT NULL")]
		public bool DisplayTitle
		{
			get
			{
				return this._DisplayTitle;
			}
			set
			{
				if ((this._DisplayTitle != value))
				{
					this.OnDisplayTitleChanging(value);
					this.SendPropertyChanging();
					this._DisplayTitle = value;
					this.SendPropertyChanged("DisplayTitle");
					this.OnDisplayTitleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="NVarChar(500)")]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Keywords", DbType="NVarChar(500)")]
		public string Keywords
		{
			get
			{
				return this._Keywords;
			}
			set
			{
				if ((this._Keywords != value))
				{
					this.OnKeywordsChanging(value);
					this.SendPropertyChanging();
					this._Keywords = value;
					this.SendPropertyChanged("Keywords");
					this.OnKeywordsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Url", DbType="NVarChar(255)")]
		public string Url
		{
			get
			{
				return this._Url;
			}
			set
			{
				if ((this._Url != value))
				{
					this.OnUrlChanging(value);
					this.SendPropertyChanging();
					this._Url = value;
					this.SendPropertyChanged("Url");
					this.OnUrlChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TabPath", DbType="NVarChar(255)")]
		public string TabPath
		{
			get
			{
				return this._TabPath;
			}
			set
			{
				if ((this._TabPath != value))
				{
					this.OnTabPathChanging(value);
					this.SendPropertyChanging();
					this._TabPath = value;
					this.SendPropertyChanged("TabPath");
					this.OnTabPathChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RouteId", DbType="Int")]
		public System.Nullable<int> RouteId
		{
			get
			{
				return this._RouteId;
			}
			set
			{
				if ((this._RouteId != value))
				{
					this.OnRouteIdChanging(value);
					this.SendPropertyChanging();
					this._RouteId = value;
					this.SendPropertyChanged("RouteId");
					this.OnRouteIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_StartDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				if ((this._StartDate != value))
				{
					this.OnStartDateChanging(value);
					this.SendPropertyChanging();
					this._StartDate = value;
					this.SendPropertyChanged("StartDate");
					this.OnStartDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EndDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> EndDate
		{
			get
			{
				return this._EndDate;
			}
			set
			{
				if ((this._EndDate != value))
				{
					this.OnEndDateChanging(value);
					this.SendPropertyChanging();
					this._EndDate = value;
					this.SendPropertyChanged("EndDate");
					this.OnEndDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PageHeadText", DbType="NVarChar(500)")]
		public string PageHeadText
		{
			get
			{
				return this._PageHeadText;
			}
			set
			{
				if ((this._PageHeadText != value))
				{
					this.OnPageHeadTextChanging(value);
					this.SendPropertyChanging();
					this._PageHeadText = value;
					this.SendPropertyChanged("PageHeadText");
					this.OnPageHeadTextChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PageFooterText", DbType="NVarChar(MAX)")]
		public string PageFooterText
		{
			get
			{
				return this._PageFooterText;
			}
			set
			{
				if ((this._PageFooterText != value))
				{
					this.OnPageFooterTextChanging(value);
					this.SendPropertyChanging();
					this._PageFooterText = value;
					this.SendPropertyChanged("PageFooterText");
					this.OnPageFooterTextChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PageControlBar", DbType="NVarChar(MAX)")]
		public string PageControlBar
		{
			get
			{
				return this._PageControlBar;
			}
			set
			{
				if ((this._PageControlBar != value))
				{
					this.OnPageControlBarChanging(value);
					this.SendPropertyChanging();
					this._PageControlBar = value;
					this.SendPropertyChanged("PageControlBar");
					this.OnPageControlBarChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsDeleted", DbType="Bit NOT NULL")]
		public bool IsDeleted
		{
			get
			{
				return this._IsDeleted;
			}
			set
			{
				if ((this._IsDeleted != value))
				{
					this.OnIsDeletedChanging(value);
					this.SendPropertyChanging();
					this._IsDeleted = value;
					this.SendPropertyChanged("IsDeleted");
					this.OnIsDeletedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsSecure", DbType="Bit NOT NULL")]
		public bool IsSecure
		{
			get
			{
				return this._IsSecure;
			}
			set
			{
				if ((this._IsSecure != value))
				{
					this.OnIsSecureChanging(value);
					this.SendPropertyChanging();
					this._IsSecure = value;
					this.SendPropertyChanged("IsSecure");
					this.OnIsSecureChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsVisible", DbType="Bit NOT NULL")]
		public bool IsVisible
		{
			get
			{
				return this._IsVisible;
			}
			set
			{
				if ((this._IsVisible != value))
				{
					this.OnIsVisibleChanging(value);
					this.SendPropertyChanging();
					this._IsVisible = value;
					this.SendPropertyChanged("IsVisible");
					this.OnIsVisibleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PermanentRedirect", DbType="Bit NOT NULL")]
		public bool PermanentRedirect
		{
			get
			{
				return this._PermanentRedirect;
			}
			set
			{
				if ((this._PermanentRedirect != value))
				{
					this.OnPermanentRedirectChanging(value);
					this.SendPropertyChanging();
					this._PermanentRedirect = value;
					this.SendPropertyChanged("PermanentRedirect");
					this.OnPermanentRedirectChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SiteMapPriority", DbType="Float NOT NULL")]
		public double SiteMapPriority
		{
			get
			{
				return this._SiteMapPriority;
			}
			set
			{
				if ((this._SiteMapPriority != value))
				{
					this.OnSiteMapPriorityChanging(value);
					this.SendPropertyChanging();
					this._SiteMapPriority = value;
					this.SendPropertyChanged("SiteMapPriority");
					this.OnSiteMapPriorityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CssClass", DbType="NVarChar(100)")]
		public string CssClass
		{
			get
			{
				return this._CssClass;
			}
			set
			{
				if ((this._CssClass != value))
				{
					this.OnCssClassChanging(value);
					this.SendPropertyChanging();
					this._CssClass = value;
					this.SendPropertyChanged("CssClass");
					this.OnCssClassChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IconFile", DbType="NVarChar(100)")]
		public string IconFile
		{
			get
			{
				return this._IconFile;
			}
			set
			{
				if ((this._IconFile != value))
				{
					this.OnIconFileChanging(value);
					this.SendPropertyChanging();
					this._IconFile = value;
					this.SendPropertyChanged("IconFile");
					this.OnIconFileChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IconFileLarge", DbType="NVarChar(100)")]
		public string IconFileLarge
		{
			get
			{
				return this._IconFileLarge;
			}
			set
			{
				if ((this._IconFileLarge != value))
				{
					this.OnIconFileLargeChanging(value);
					this.SendPropertyChanging();
					this._IconFileLarge = value;
					this.SendPropertyChanged("IconFileLarge");
					this.OnIconFileLargeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CultureCode", DbType="NVarChar(10)")]
		public string CultureCode
		{
			get
			{
				return this._CultureCode;
			}
			set
			{
				if ((this._CultureCode != value))
				{
					this.OnCultureCodeChanging(value);
					this.SendPropertyChanging();
					this._CultureCode = value;
					this.SendPropertyChanged("CultureCode");
					this.OnCultureCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedByUserId", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> CreatedByUserId
		{
			get
			{
				return this._CreatedByUserId;
			}
			set
			{
				if ((this._CreatedByUserId != value))
				{
					this.OnCreatedByUserIdChanging(value);
					this.SendPropertyChanging();
					this._CreatedByUserId = value;
					this.SendPropertyChanged("CreatedByUserId");
					this.OnCreatedByUserIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedOnDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> CreatedOnDate
		{
			get
			{
				return this._CreatedOnDate;
			}
			set
			{
				if ((this._CreatedOnDate != value))
				{
					this.OnCreatedOnDateChanging(value);
					this.SendPropertyChanging();
					this._CreatedOnDate = value;
					this.SendPropertyChanged("CreatedOnDate");
					this.OnCreatedOnDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastModifiedByUserId", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> LastModifiedByUserId
		{
			get
			{
				return this._LastModifiedByUserId;
			}
			set
			{
				if ((this._LastModifiedByUserId != value))
				{
					this.OnLastModifiedByUserIdChanging(value);
					this.SendPropertyChanging();
					this._LastModifiedByUserId = value;
					this.SendPropertyChanged("LastModifiedByUserId");
					this.OnLastModifiedByUserIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastModifiedOnDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> LastModifiedOnDate
		{
			get
			{
				return this._LastModifiedOnDate;
			}
			set
			{
				if ((this._LastModifiedOnDate != value))
				{
					this.OnLastModifiedOnDateChanging(value);
					this.SendPropertyChanging();
					this._LastModifiedOnDate = value;
					this.SendPropertyChanged("LastModifiedOnDate");
					this.OnLastModifiedOnDateChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
