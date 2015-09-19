using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.IO;
using CommonLibrary;

namespace ShopCartLibrary.Products
{
    public class ProductHostServers
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductHostServers(){}

        public DataTable GetDetail(string Product_No)
        {
            SqlCommand cmd = new SqlCommand("[Production].[ProductsHost_GetDetailByNo]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.Add("@Product_No", SqlDbType.NVarChar, int.MaxValue).Value = Product_No;
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListHostByTypeId(int TypeId)
        {
            SqlCommand cmd = new SqlCommand("[Production].[ProductsHost_GetListByProductTypeId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_TypeId", TypeId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string Product_No, string ServerLocation, string ServerProvider, decimal FeePerMonth, string Duration,
                            string Capacity, string Brandwidth, int Subdomain, int Webmail, int FTP, string NetFrameWork,
                            int PHP5, int MSSQL, int MySQL5, int AddonDomain, int ParkDomain, int CrystalReport, string ControlPanel, string BackupSystem,
                            string HostingSpeed, string TechnicalSupport)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_HostServer_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            cmd.Parameters.AddWithValue("@ServerLocation", ServerLocation);
            cmd.Parameters.AddWithValue("@ServerProvider", ServerProvider);
            cmd.Parameters.AddWithValue("@FeePerMonth", FeePerMonth);
            cmd.Parameters.AddWithValue("@Duration", Duration);
            cmd.Parameters.AddWithValue("@Capacity", Capacity);
            cmd.Parameters.AddWithValue("@Brandwidth", Brandwidth);
            cmd.Parameters.AddWithValue("@Subdomain", Subdomain);
            cmd.Parameters.AddWithValue("@Webmail", Webmail);
            cmd.Parameters.AddWithValue("@FTP", FTP);
            cmd.Parameters.AddWithValue("@NetFrameWork", NetFrameWork);
            cmd.Parameters.AddWithValue("@PHP5", PHP5);
            cmd.Parameters.AddWithValue("@MSSQL", MSSQL);
            cmd.Parameters.AddWithValue("@MySQL5", MySQL5);
            cmd.Parameters.AddWithValue("@AddonDomain", AddonDomain);
            cmd.Parameters.AddWithValue("@ParkDomain", ParkDomain);
            cmd.Parameters.AddWithValue("@CrystalReport", CrystalReport);
            cmd.Parameters.AddWithValue("@ControlPanel", ControlPanel);
            cmd.Parameters.AddWithValue("@BackupSystem", BackupSystem);
            cmd.Parameters.AddWithValue("@HostingSpeed", HostingSpeed);
            cmd.Parameters.AddWithValue("@TechnicalSupport", TechnicalSupport);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int,2) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(string Product_No, string ServerLocation, string ServerProvider, decimal FeePerMonth, string Duration,
                            string Capacity, string Brandwidth, int Subdomain, int Webmail, int FTP, string NetFrameWork,
                            int PHP5, int MSSQL, int MySQL5, int AddonDomain, int ParkDomain, int CrystalReport, string ControlPanel, string BackupSystem,
                            string HostingSpeed, string TechnicalSupport)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_HostServer_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            cmd.Parameters.AddWithValue("@ServerLocation", ServerLocation);
            cmd.Parameters.AddWithValue("@ServerProvider", ServerProvider);
            cmd.Parameters.AddWithValue("@FeePerMonth", FeePerMonth);
            cmd.Parameters.AddWithValue("@Duration", Duration);
            cmd.Parameters.AddWithValue("@Capacity", Capacity);
            cmd.Parameters.AddWithValue("@Brandwidth", Brandwidth);
            cmd.Parameters.AddWithValue("@Subdomain", Subdomain);
            cmd.Parameters.AddWithValue("@Webmail", Webmail);
            cmd.Parameters.AddWithValue("@FTP", FTP);
            cmd.Parameters.AddWithValue("@NetFrameWork", NetFrameWork);
            cmd.Parameters.AddWithValue("@PHP5", PHP5);
            cmd.Parameters.AddWithValue("@MSSQL", MSSQL);
            cmd.Parameters.AddWithValue("@MySQL5", MySQL5);
            cmd.Parameters.AddWithValue("@AddonDomain", AddonDomain);
            cmd.Parameters.AddWithValue("@ParkDomain", ParkDomain);
            cmd.Parameters.AddWithValue("@CrystalReport", CrystalReport);
            cmd.Parameters.AddWithValue("@ControlPanel", ControlPanel);
            cmd.Parameters.AddWithValue("@BackupSystem", BackupSystem);
            cmd.Parameters.AddWithValue("@HostingSpeed", HostingSpeed);
            cmd.Parameters.AddWithValue("@TechnicalSupport", TechnicalSupport);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int,2) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
