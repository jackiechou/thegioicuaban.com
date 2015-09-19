using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Common;
using System.Data;
using Library.Common;

namespace ShopCartLibrary.Orders
{
    /**
     * The ShoppingCart class holds the items that are in the cart and provides methods for their manipulation
     * * Lưu trữ sản phẩm đã mua và thực hiện một số các phương thức khác
     */
    public class ShoppingCart
    {
        #region Properties

        public List<CartItem> Items { get; private set; }

        #endregion

        #region Singleton Implementation

        public static readonly ShoppingCart Instance;
        // Hàm khởi tạo
        static ShoppingCart()
        {
            // Nếu chưa có session chưa giỏ hàng, thì khởi tạo và đưa thông tin giỏ hàng vào 
            // ngược lại thì lấy thông tin trong giỏ hàng
            if (HttpContext.Current.Session["ASPNETShoppingCart"] == null)
            {
                Instance = new ShoppingCart();
                Instance.Items = new List<CartItem>();
                HttpContext.Current.Session["ASPNETShoppingCart"] = Instance;
            }
            else
            {
                Instance = (ShoppingCart)HttpContext.Current.Session["ASPNETShoppingCart"];
            }
        }

        protected ShoppingCart() { }

        #endregion

        #region Item Modification Methods
        /**
	 * AddItem() - Add một item vào trong giỏ hàng
	 */
        public void AddItem(int productId, int num_items)
        {
            // Tạo mới một Cartitem
            CartItem newItem = new CartItem(productId);

            if (Items.Contains(newItem))
            {
                foreach (CartItem item in Items)
                {
                    if (item.Equals(newItem))
                    {
                        //item.Quantity++;
                        item.Quantity += num_items;
                        return;
                    }
                }
            }
            else
            {
                newItem.Quantity = num_items;
                Items.Add(newItem);
            }
        }

        //public void AddItem(int productId)
        //{
        //    // Tạo mới một Cartitem
        //    CartItem newItem = new CartItem(productId);

        //    if (Items.Contains(newItem))
        //    {
        //        foreach (CartItem item in Items)
        //        {
        //            if (item.Equals(newItem))
        //            {
        //                item.Quantity++;
        //                return;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        newItem.Quantity = 1;
        //        Items.Add(newItem);
        //    }
        //}

        /**
         * SetItemQuantity() - Thay đổi số lượng của sản phẩm trong giỏ hàng
         */
        public void SetItemQuantity(int productId, int quantity)
        {
            // Nếu số lượng bằng 0 thì xóa item
            if (quantity == 0)
            {
                RemoveItem(productId);
                return;
            }

            // Tìm và update số lượng cho item trong giỏ hàng
            CartItem updatedItem = new CartItem(productId);
            foreach (CartItem item in Items)
            {
                if (item.Equals(updatedItem))
                {
                    item.Quantity = quantity;
                    return;
                }
            }
        }
        /**
         * RemoveItem() - Xóa item trong giỏ hàng
         */
        public void RemoveItem(int productId)
        {
            CartItem removedItem = new CartItem(productId);
            Items.Remove(removedItem);
        }

        public void RemoveAll()
        {
            Items.Clear();
        }

        #endregion

        #region Reporting Methods
        /**
	 * GetSubTotal() - Tính tổng tiền
	 */
        public decimal GetSubTotal()
        {
            decimal subTotal = 0;
            foreach (CartItem item in Items)
                subTotal += item.TotalPrice;

            return subTotal;
        }
        #endregion

    }
}
