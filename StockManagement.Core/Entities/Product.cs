using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockManagement.Core.Interfaces;

namespace StockManagement.Core.Entities
{
    public class Product : ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
    }
}
