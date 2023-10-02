using DevExpress.XtraGrid;
using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOI_PPM
{
    public partial class RelationsForm : Form
    {
        private SqlConnection _DBConnectionProduction = null;

        public RelationsForm(SqlConnection DBConnectionProduction)
        {
            this._DBConnectionProduction = DBConnectionProduction;
            InitializeComponent();
        }

        private void refreshData()
        {
            string bookmark = getBookmark();
            string sql = "select ID, Product, Pads from VIT_PCB_Pads order by Product";
            gridControl1.DataSource = HelperClass.callQuery(_DBConnectionProduction, sql);
            try
            {
                gridView1.Columns["ID"].Visible = false;
                gridView1.Columns["Pads"].Caption = "Number of Pads for PCB";
                gotoBookmark(bookmark);
            }
            catch(Exception) {; }
        }

        private void RelationsForm_Shown(object sender, EventArgs e)
        {
            refreshData();
        }

        private bool getSelectedValues(ref int ID, ref string product, ref int pads)
        {
            int[] arrayOfSelected = gridView1.GetSelectedRows();

            if (arrayOfSelected.Count() == 0) return false;

            try
            {
                ID = Convert.ToInt32(gridView1.GetDataRow(arrayOfSelected[0])["ID"]);
                product = gridView1.GetDataRow(arrayOfSelected[0])["Product"].ToString();
                pads = Convert.ToInt32(gridView1.GetDataRow(arrayOfSelected[0])["Pads"]);
                
                return true;
            }
            catch (Exception)
            {; }
            return false;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            int ID = -1;
            string product = "";
            int pads = 0;

            if (getSelectedValues(ref ID, ref product, ref pads))
            {
                EditRelationForm form = new EditRelationForm();
                form.init(product, pads);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.getPads() > 0)
                    {
                        string sql = "update VIT_PCB_Pads set Pads = {0} where ID = {1}";
                        sql = string.Format(sql, form.getPads(), ID);

                        string message = HelperClass.callCmd(_DBConnectionProduction, sql);
                        if (message != "") MessageBox.Show("Can not update VIT_PCB_Pads.\nError: " + message);
                        else refreshData();
                    }
                    else MessageBox.Show("Pads : The value is not allowed!");
                }
            }
        }

        private string getBookmark()
        {
            if (gridView1.FocusedRowHandle != GridControl.InvalidRowHandle)
                return gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, gridView1.Columns["Product"]);
            return "";
        }

        private void gotoBookmark(string bookmark)
        {
            try
            {
                int FocusedRowHandle = gridView1.LocateByDisplayText(0, gridView1.Columns["Product"], bookmark);
                if (FocusedRowHandle != GridControl.InvalidRowHandle)
                {
                    gridView1.OptionsSelection.MultiSelect = false;
                    gridView1.ClearSelection();
                    gridView1.FocusedRowHandle = FocusedRowHandle;
                    gridView1.OptionsSelection.MultiSelect = true;
                    gridView1.SelectRow(FocusedRowHandle);
                }
            }
            catch (Exception) {; }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            simpleButton2_Click(sender, e);
        }
    }
}
