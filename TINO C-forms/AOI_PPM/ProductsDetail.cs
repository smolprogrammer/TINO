using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

// count size of the screen, or maybe set it manually.
// double click option.

namespace AOI_PPM
{
    public partial class ProductsDetail : Form
    {
        public string SelectedProductName { get; private set; }
        private string AOIDbName;
        private SqlConnection sqlConnection = new SqlConnection("server=10.129.38.65;Database=AOI_SUPERVISOR_3D;User=vitaoidb;Pwd=VITP@ssw0RD");

        public ProductsDetail(string AOISimpleName, string AOIname, string start, string end)
        {
            InitializeComponent();

            AOIDbName = AOIname;
            label1.Text = $"Products made on {AOISimpleName}";

            FetchCardData(start, end);
            AdjustFormWidthForCards();
            CustomizeCardView();

        }

        private void FetchCardData(string start, string end) 
        {
            string sql = $@"select
                    Machine_Name, Product_Name_Product, COUNT(*) as Counter
                    from PanelViewWithProgramUTC tPanelCard
                    left join Tested_Object tTestedObj with(NOLOCK) on tTestedObj.Card_id = tPanelCard.Card_id
                    where                    
                    Machine_Name = '{AOIDbName}' AND
                    (StartTime between '{start}' AND '{end}') AND Card_Status <> 0 AND (panel_status <> 0 or (anomaly_BR / 256) % 2 = 1) AND 
                    tTestedObj.Topology is not null
                    GROUP BY Product_Name_Product, Machine_Name";

            DataTable dt = HelperClass.callQuery(sqlConnection, sql);
            gridControl1.DataSource = dt;

        }

        private void CustomizeCardView() 
        {
            CardView cardView = gridControl1.DefaultView as CardView;

            // Customize the card caption
            cardView.CardCaptionFormat = "{2}";

            // Adjust the layout to show desired fields
            cardView.Columns["Machine_Name"].VisibleIndex = 0;
            cardView.Columns["Product_Name_Product"].VisibleIndex = 1;
            cardView.Columns["Counter"].VisibleIndex = 2;

        }

        private void AdjustFormWidthForCards()
        {
            int cardCount = cardView1.RowCount; // Assuming cardView1 is your CardView
            int defaultCardWidth = 300; // Replace with your default card width
            int intervalBetweenCards = 10; // Replace with your interval/margin between cards
            int totalCardWidth = 340;

            // Calculate total width required for the cards
            if (cardCount > 6)
                totalCardWidth = (2 * defaultCardWidth) + ((cardCount - 1) * intervalBetweenCards);

            // Adjust the form's width (considering form padding, borders, etc.)
            this.Width = totalCardWidth; // The '40' is an arbitrary number to account for form borders/padding. Adjust as needed.

        }

        // This method should be called when a product card is double-clicked in the ProductDetail form
        private void Card_DoubleClick(object sender, EventArgs e)
        {
            // Assuming the product name is in a label on the card. Modify as needed.
            Label clickedLabel = sender as Label;
            SelectedProductName = clickedLabel.Text;

            // Close the ProductDetail form after selection
            this.Close();
        }

        private void cardView1_DoubleClick(object sender, EventArgs e)
        {
            // Get the card that was clicked
            Point clickPoint = cardView1.GridControl.PointToClient(Control.MousePosition);
            CardHitInfo hitInfo = cardView1.CalcHitInfo(clickPoint);

            // If a card was clicked
            if (hitInfo.InCard)
            {
                // Get the handle of the clicked card
                int cardHandle = hitInfo.RowHandle;

                // Fetch the value of the "Product_Name_Product" field for the clicked card
                string potentialProductName = cardView1.GetRowCellValue(cardHandle, "Product_Name_Product").ToString();

                // Show the confirmation popup
                DialogResult result = MessageBox.Show($"Are you sure you want to choose {potentialProductName}?",
                                                      "Confirm Product Selection",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SelectedProductName = potentialProductName;

                    // Close the ProductDetail form after selection
                    this.Close();
                }
            }
        }

    }
}
