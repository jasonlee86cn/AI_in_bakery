using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PointOfSalesDesignDemo.CustomControls
{
    public partial class CartItem : UserControl
    {
        public string ItemId { get; set; }

        public string ItemPerUnitPrice
        {
            get { return lblPerItemPrice.Text; }
            set { lblPerItemPrice.Text = value; }
        }
        public string ItemName
        {
            get { return lblItemName.Text; }
            set { lblItemName.Text = value; }
        }
        public string ItemPrice
        {
            get { return lblItemPrice.Text; }
            set { lblItemPrice.Text = value; }
        }

        public string ItemQuantity
        {
            get { return btnQuantity.Text; }
            set { btnQuantity.Text = value; }
        }

        public CartItem()
        {
            InitializeComponent();
        }
    }
}
