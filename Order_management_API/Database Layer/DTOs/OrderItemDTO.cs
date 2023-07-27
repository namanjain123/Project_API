using Database_Layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Layer.DTOs
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }

        
        public int OrderId { get; set; }
        public virtual OrderDTO Order { get; set; }

        
        public int ItemId { get; set; }
        public virtual ItemDTO Item { get; set; }
    }
}
