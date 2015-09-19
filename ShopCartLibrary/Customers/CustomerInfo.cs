using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Library.Controls.ShopCarts.Customers
{
    [Serializable]
    [DataContract]
    public class CustomerInfo
    {
        [DataMember]
        public string Customer_ID { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string CustomerName { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Fax { get; set; }

        [DataMember]
        public string CreatedDate { get; set; }

        [DataMember]
        public string LastModifiedDate { get; set; }
    }
}
