using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOI_PPM
{
    public partial class EditRelationForm : Form
    {
        public EditRelationForm()
        {
            InitializeComponent();
        }
        public void init(string product, int pads)
        {
            textBox1.Text = product;
            numericUpDown1.Value = pads;            
        }

        public int getPads()
        {
            try
            {
                return Convert.ToInt32(numericUpDown1.Value);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private void EditRelationForm_Shown(object sender, EventArgs e)
        {
            numericUpDown1.Focus();
        }
    }
}
