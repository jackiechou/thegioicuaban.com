using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ShopCartLibrary.Products
{
    public class ProductComponents
    {
        public int VendorId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal NetPrice { get; set; }
        public decimal GrossPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public int Weight { get; set; }

        public ProductComponents(int id)
        {
            this.Id = id;
            ProductController product_obj = new ProductController();
            DataTable dt = product_obj.GetDetailByID(id);
            if (dt.Rows.Count > 0)
            {
                this.NetPrice = Convert.ToDecimal(dt.Rows[0]["NetPrice"].ToString());
                this.GrossPrice = Convert.ToDecimal(dt.Rows[0]["GrossPrice"].ToString());
                this.Tax = Convert.ToDecimal(dt.Rows[0]["TaxRate"].ToString());
                this.Discount = Convert.ToDecimal(dt.Rows[0]["DiscountRate"].ToString());
                this.Description = dt.Rows[0]["ShortDescription"].ToString();
                this.Name = dt.Rows[0]["Product_Name"].ToString();
                this.Image = dt.Rows[0]["PhotoFileName"].ToString();
                this.AvailableQuantity = Convert.ToInt32(dt.Rows[0]["UnitsInStock"].ToString()) - Convert.ToInt32(dt.Rows[0]["UnitsOnOrder"].ToString());
                this.VendorId = Convert.ToInt32(dt.Rows[0]["Vendor_Id"].ToString());
                this.Weight = Convert.ToInt32(dt.Rows[0]["Weight"].ToString());
            }
        }
    }
}
