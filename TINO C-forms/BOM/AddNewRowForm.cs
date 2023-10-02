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

namespace BMO
{
    public partial class AddNewRowForm : Form
    {
        public Dictionary<string, object> EnteredValues { get; private set; } = new Dictionary<string, object>();
        List<Control> dataControls = new List<Control>();
        private SqlConnection SqlConnection;
        private string ActiveTable;

        public AddNewRowForm(DataTable tableStructure, SqlConnection sqlConnection, string activeTable)
        {
            InitializeComponent();

            SqlConnection = sqlConnection;
            ActiveTable = activeTable;

            label2.Text = $"Enter data for new row in {ActiveTable} Table";

            int x = GenerateControlsForColumns(tableStructure, this);
            this.Width = x + 60;
        }

        private int GenerateControlsForColumns(DataTable tableStructure, Form formToAddTo)
        {
            int lastx = 300;
            // Starting positions
            int startX = 60;
            int startY = 80;
            int currentX = startX;
            int currentY = startY;
            int controlHeight = 12;
            int controlSpacing = 20;
            int controlWidth = 180;
            int maxControlsInRow = 3;
            int currentControlCount = 0;

            foreach (DataColumn column in tableStructure.Columns)
            {
                string columnName = column.ColumnName.ToString();
                if (ControlOutput(columnName))
                {
                    // Create label
                    Label label = new Label();
                    label.Width = 130;
                    label.Text = column.ColumnName;
                    label.Font = new Font("Arial", 12);
                    label.Location = new Point(currentX, currentY);
                    formToAddTo.Controls.Add(label);

                    Control currentControl;

                    // Update X position for controls
                    currentX += label.Width + controlSpacing;

                    if ((ActiveTable == "EReferences" 
                        && (column.ColumnName == "NameOfEStation" || column.ColumnName == "NameOfComp" || column.ColumnName == "NameOfProject")) ||
                        (ActiveTable == "EStation" 
                        && (column.ColumnName == "NameOfLine" || column.ColumnName == "NameOfStation")) 
                        )
                    {
                        DevExpress.XtraEditors.GridLookUpEdit gridLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
                        gridLookUpEdit.Name = column.ColumnName; // Naming the control for later reference
                        gridLookUpEdit.Location = new Point(currentX, currentY);
                        gridLookUpEdit.Font = new Font("Arial", 10);
                        gridLookUpEdit.Size = new Size(100, 26);
                        gridLookUpEdit.Width = controlWidth;
                        PopulateGridLookUpEdit(gridLookUpEdit, column.ColumnName);
                        formToAddTo.Controls.Add(gridLookUpEdit);
                        currentControl = gridLookUpEdit;
                    }
                    else
                    {
                        // Create TextBox for other columns
                        TextBox textBox = new TextBox();
                        textBox.Name = columnName;
                        textBox.Location = new Point(currentX, currentY);
                        textBox.Font = new Font("Arial", 12);
                        textBox.Width = controlWidth;
                        formToAddTo.Controls.Add(textBox);

                        currentControl = textBox;
                    }
                    dataControls.Add(currentControl);

                    // Update X for next control
                    currentX += currentControl.Width + controlSpacing;
                    if (currentX > lastx)
                        lastx = currentX;
                    currentControlCount++;

                    // Check if we have added the maximum number of controls in the current row
                    if (currentControlCount == maxControlsInRow)
                    {
                        // Reset X, update Y for next row, and reset the control counter
                        if (currentX > lastx)
                            lastx = currentX;
                        currentX = startX;
                        currentY += controlHeight + controlSpacing;
                        currentControlCount = 0;
                    }

                }
            }
            return lastx;
        }

        private void PopulateGridLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit gridLookUpEdit, string columnName)
        {
            DataTable dt = new DataTable();

            if (columnName == "NameOfProject")
            {
                string query = $"SELECT * FROM [dbo].[Project] ORDER BY Name";
                dt = Helpers.HelperClass.callQuery(SqlConnection, query);
                gridLookUpEdit.Properties.DisplayMember = "Name";
                gridLookUpEdit.Properties.ValueMember = dt.Columns[0].ColumnName;
            }
            else if (columnName == "NameOfEStation")
            {
                string query = $@"SELECT t2.Name1 AS NameOfLine,
                                         t3.Name AS NameOfStation, 
                                         t1.* 
                                FROM [dbo].[EStation] t1
                                jOIN [dbo].[Line] t2 ON t2.ID = t1.ID_Line
                                JOIN [dbo].[Station] t3 ON t3.ID = t1.ID_Station
                                ORDER BY Name";
                dt = Helpers.HelperClass.callQuery(SqlConnection, query);
                gridLookUpEdit.Properties.DisplayMember = "Name";
                gridLookUpEdit.Properties.ValueMember = "ID";
            }
            else if (columnName == "NameOfComp")
            {
                string query = $"SELECT * FROM [dbo].[CompPrice] ORDER BY NameOfComp";
                dt = Helpers.HelperClass.callQuery(SqlConnection, query);
                gridLookUpEdit.Properties.DisplayMember = "NameOfComp";
                gridLookUpEdit.Properties.ValueMember = dt.Columns[0].ColumnName;
            }
            else if (columnName == "NameOfLine")
            {
                string query = $"SELECT * FROM [dbo].[Line] ORDER BY Name1";
                dt = Helpers.HelperClass.callQuery(SqlConnection, query);
                gridLookUpEdit.Properties.DisplayMember = "Name1";
                gridLookUpEdit.Properties.ValueMember = dt.Columns[0].ColumnName;
            }
            else if (columnName == "NameOfStation")
            {
                string query = $"SELECT * FROM [dbo].[Station] ORDER BY Name";
                dt = Helpers.HelperClass.callQuery(SqlConnection, query);
                gridLookUpEdit.Properties.DisplayMember = "Name";
                gridLookUpEdit.Properties.ValueMember = dt.Columns[0].ColumnName;
            }

            gridLookUpEdit.Properties.DataSource = dt;

            // Ensure that the dropdown displays all columns
            gridLookUpEdit.Properties.View.OptionsView.ShowAutoFilterRow = true;
            gridLookUpEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            gridLookUpEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;

            gridLookUpEdit.Properties.View.Columns.Clear();
            foreach (DataColumn column in dt.Columns)
            {
                DevExpress.XtraGrid.Columns.GridColumn gridCol = new DevExpress.XtraGrid.Columns.GridColumn
                {
                    FieldName = column.ColumnName,
                    Visible = true
                };
                gridLookUpEdit.Properties.View.Columns.Add(gridCol);
            }
        }

        //If needed to hide irrelevant data
        private bool ControlOutput(string column)
        {
            if (!column.Contains("ID"))
            {
                if (ActiveTable == "EReferences" &&
                    (column == "Component" ||
                     column == "SerialNumber" ||
                     column == "NameOfLine" ||
                     column == "NameOfStation"))
                    return false;

                return true;
            }
            return false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Iterate over all controls in the form
            foreach (Control control in dataControls)
            {
                if (control is DevExpress.XtraEditors.GridLookUpEdit)
                {
                    DevExpress.XtraEditors.GridLookUpEdit gridLookUpEdit = control as DevExpress.XtraEditors.GridLookUpEdit;
                    DataRowView rowView = gridLookUpEdit.GetSelectedDataRow() as DataRowView;

                    if (control.Name == "NameOfProject")
                    {
                        EnteredValues["Project_ID"] = rowView["ID"];
                    }
                    else if (control.Name == "NameOfEStation")
                    {
                        EnteredValues["Estation_ID"] = rowView["ID"];
                    }
                    else if (control.Name == "NameOfComp")
                    {
                        EnteredValues["Comp_ID"] = rowView["ID"];
                    }
                    else if (control.Name == "NameOfLine")
                    {
                        EnteredValues["ID_Line"] = rowView["ID"];
                    }
                    else if (control.Name == "NameOfStation")
                    {
                        EnteredValues["ID_Station"] = rowView["ID"];
                    }
                }
                else if (control is TextBox)
                {
                    EnteredValues[control.Name] = control.Text;
                }
            }

            // Close the form after storing the values
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
