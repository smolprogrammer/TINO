using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMO
{
    public partial class OtherTableSelector : Form
    {
        public string SelectedValue { get; private set; }
        public string ActiveTableName { get; set; }
        public OtherTableSelector()
        {
            InitializeComponent();
        }

        private void EStationButton_Click(object sender, EventArgs e)
        {
            if (CheckActiveTable(EStationButton.Text))
            {
                SelectedValue = "EStation";
                this.Close();
            }
        }

        private void StationButton_Click(object sender, EventArgs e)
        {
            if (CheckActiveTable(StationButton.Text))
            {
                SelectedValue = "Station";
                this.Close();
            }
        }

        private void CompPriceButton_Click(object sender, EventArgs e)
        {
            if (CheckActiveTable(CompPriceButton.Text))
            {
                SelectedValue = "CompPrice";
                this.Close();
            }
        }

        private void EReferencesButton_Click(object sender, EventArgs e)
        {
            if (CheckActiveTable(EReferencesButton.Text))
            {
                SelectedValue = "EReferences";
                this.Close();
            }
        }

        private bool CheckActiveTable(string buttonText) 
        {
            if (buttonText == ActiveTableName)
            {
                MessageBox.Show("You've already picked that table.");
                return false;
            }
            return true;
        }

    }
}
