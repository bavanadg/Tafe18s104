using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    class ShoppingList
    {
        [PrimaryKey, AutoIncrement]
        public int shoppingItemID { get; set; }

        [NotNull]
        public string shopName { get; set; }

        [NotNull]
        public string nameOfItem { get; set; }

        [NotNull]
        public string shoppingDate { get; set; }

        [NotNull]
        public double priceQuoted { get; set; }

    }
}