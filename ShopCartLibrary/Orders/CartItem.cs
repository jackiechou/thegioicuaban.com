using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShopCartLibrary.Products;

namespace ShopCartLibrary.Orders
{
    /**
     * CartItem Class
     * 
     * Tạo cấu trúc của giỏ hàng
     */

    public class CartItem : IEquatable<CartItem>
    {
        #region Properties
        //Tạo thuộc tính getter và setter.
        // A place to store the quantity in the cart      
        public int Quantity { get; set; }
        private int _productId;
        public int ProductId
        {
            get { return _productId; }
            set
            {
                _product = null;
                _productId = value;
            }
        }

        private ProductComponents _product = null;
        public ProductComponents Prod
        {
            get
            {
                // Lazy initialization - the object won't be created until it is needed
                if (_product == null)
                {
                    _product = new ProductComponents(ProductId);                    
                }
                return _product;
            }
        }

        public string Description
        {
            get { return Prod.Description; }
        }
        public string Name
        {
            get { return Prod.Name; }
        }
        public decimal NetPrice
        {
            get { return Prod.NetPrice; }
        }
        public decimal GrossPrice
        {
            get { return Prod.GrossPrice; }
        }
        public decimal Tax
        {
            get { return Prod.Tax; }
        }
        public decimal Discount
        {
            get { return Prod.Discount; }
        }
        public decimal TotalPrice
        {
            get { return (Prod.NetPrice + Prod.Tax - Prod.Discount) * Quantity; }
        }    
        public int VendorId
        {
            get { return Prod.VendorId; }
        }
        public int Weight
        {
            get { return Prod.Weight; }
        }  
        #endregion

        // CartItem constructor just needs a productId or productNo
        public CartItem(int productId)
        {
            this.ProductId = productId;
        }
        
        /**
         * Equals() - Needed to implement the IEquatable interface
         *    Tests whether or not this item is equal to the parameter
         *    This method is called by the Contains() method in the List class
         *    We used this Contains() method in the ShoppingCart AddItem() method
         */
        public bool Equals(CartItem item)
        {
            return item.ProductId == this.ProductId;
        }
    }
}
