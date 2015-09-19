using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class tiny_mce_plugins_netImageBrowser_up : System.Web.UI.Page
{
    //private string path = AppDomain.CurrentDomain.BaseDirectory + "user_files\\assets\\images";  
    private string path = HttpContext.Current.Server.MapPath("../../../../user_files/assets/images");
    private string imgPath = "../../../../user_files/assets/images";    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request.QueryString["del"]))
        {
            fileDel(Request.QueryString["del"]);
        }
        else if (!string.IsNullOrEmpty(Request.QueryString["fldel"]))
        {
            folderDel(Request.QueryString["fldel"]);
        }

        pageBind();
    }

    #region Function

    /// <summary>
    /// Page Load - page object bind
    /// </summary>
    private void pageBind()
    {
        lnkHome.NavigateUrl = "up.aspx";

        DirectoryInfo drInfo = new DirectoryInfo(path);

        // Main Folder
        bindFolder(drInfo);

        if (!string.IsNullOrEmpty(Request.QueryString["fl"]))
        {
            path += "/" + Request.QueryString["fl"];

            imgPath += "/" + Request.QueryString["fl"];

            // Path Change
            drInfo = new DirectoryInfo(path);

            // Create Folder Panel  !? Next Step 
            pnlCreators.Visible = false;
        }

        // File List
        bindFile(drInfo);
    }

  
    // Folder List 
    private void bindFolder(DirectoryInfo drInfo)
    {
        DirectoryInfo[] drList = drInfo.GetDirectories();

        List<DirectoryInfo> lstDirectory = new List<DirectoryInfo>();

        foreach (DirectoryInfo drc in drList)
        {
            lstDirectory.Add(drc);
        }

        lvFolderList.DataSource = lstDirectory;
        lvFolderList.DataBind();
    }

   
    /// File List   
    private void bindFile(DirectoryInfo drInfo)
    {
        FileInfo[] rgFiles = drInfo.GetFiles();

        List<FileInfo> lstFile = new List<FileInfo>();

        foreach (FileInfo fi in rgFiles)
        {
            lstFile.Add(fi);
        }

        lvFileList.DataSource = lstFile;
        lvFileList.DataBind();

    }

    #region File Delete

    private void fileDel(string fileName)
    {
        if (!string.IsNullOrEmpty(Request.QueryString["fl"]))
        {
            File.Delete(path + "/" + Request.QueryString["fl"] + "/" + fileName);
        }
        else
        {
            File.Delete(path + "/" + fileName);
        }
    }

    #endregion

    #region Folder Delete

    private void folderDel(string folder)
    {
        Directory.Delete(path + "/" + folder, true);

        Response.Redirect("up.aspx");
    }

    #endregion

    #endregion

    #region Event

    protected void btnCreateFolder_click(object sender, EventArgs e)
    {
        if (!Directory.Exists(path + "/" + txtFolder.Text.Trim()))
            Directory.CreateDirectory(path + "/" + txtFolder.Text.Trim());

        pageBind();
    }

    protected void btnAddFile_click(object sender, EventArgs e)
    {
        try
        {
            if (flUpload != null && flUpload.HasFile == true)
            {
              
                string file_path = string.Empty;

                if (txtWidth.Text != string.Empty && txtWidth.Text != string.Empty)
                {
                    int width = Convert.ToInt32(txtWidth.Text);
                    int height = Convert.ToInt32(txtHeight.Text);

                    System.Drawing.Image image = System.Drawing.Image.FromStream(flUpload.PostedFile.InputStream);
                    System.Drawing.Image img = image.GetThumbnailImage((int)width, (int)height, delegate() { return false; }, (IntPtr)0);
                    file_path = Path.Combine(path, string.Format("{0}{1}", Path.GetFileNameWithoutExtension(flUpload.FileName), Path.GetExtension(flUpload.FileName)));
                    img.Save(file_path);
                }
                else
                {
                    file_path = Path.Combine(path, string.Format("{0}{1}", Path.GetFileNameWithoutExtension(flUpload.FileName), Path.GetExtension(flUpload.FileName)));
                    flUpload.SaveAs(file_path);
                }              

                if (!string.IsNullOrEmpty(Request.QueryString["fl"]))
                {
                    Response.Redirect("up.aspx?fl=" + Request.QueryString["fl"]);
                }
                else
                {
                    Response.Redirect("up.aspx");
                }
            }
            else
            {
                Response.Write("<script>alert('Vui lòng chọn hình ảnh');window.history.back();</script>");
                Response.End();
            }
        }
        catch (Exception ex)
        { ex.ToString(); }
    }

    protected void lvFileList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {

        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            Image imgList = (Image)e.Item.FindControl("imgList");

            imgList.ImageUrl = imgPath + "/" + ((FileInfo)dataItem.DataItem).Name;
        }
    }


    #endregion
}