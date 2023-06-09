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
    public partial class btnProduct : UserControl
    {
        public string ItemId { get; set; }

        public string ItemCategory
        {
            get { return lblCategory.Text; }
            set { lblCategory.Text = value; }
        }
        public string ItemName
        {
            get { return lblItemName.Text; }
            set { lblItemName.Text = value; }
        }
        public string ItemPrice
        {
            get { return lblPrice.Text; }
            set { lblPrice.Text = value; }
        }

        public btnProduct()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }
    }
}
