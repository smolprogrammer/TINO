using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using System.ComponentModel;
using DevExpress.XtraGrid.Views.Base;

// BOM 
namespace BMO
{
    public partial class BMOForm : Form
    {
        private SqlConnection _DBConnection = null;
        private string activeTable = "EReferences";
        private string lastOpenedTable = "EReferences";
        private int EStationId, LineId, StationId;
        private string lastFetchedNameOfLine = string.Empty;

        public BMOForm()
        {
            InitializeComponent();
        }

        public bool init(SqlConnection sqlConnection)
        {
            _DBConnection = sqlConnection;
            return true;
        }

        //Buttons functionality
        private void refreshButton_Click(object sender, EventArgs e)
        {
            // Check if a new table has been opened
            if (lastOpenedTable != activeTable)
            {
                gridView1.ClearColumnsFilter();
                gridView1.Columns.Clear();
                activeTable = lastOpenedTable;  // Update the last opened table
                FetchData();
            }
            else
            {
                // If not a new table, save the current layout to a memory stream
                using (MemoryStream stream = new MemoryStream())
                {
                    gridView1.SaveLayoutToStream(stream, DevExpress.Utils.OptionsLayoutBase.FullLayout);
                    stream.Seek(0, SeekOrigin.Begin);

                    // Clear the data source
                    gridControl1.DataSource = null;
                    gridView1.Columns.Clear();
                    FetchData();

                    // Restore the layout from the memory stream
                    gridView1.RestoreLayoutFromStream(stream, DevExpress.Utils.OptionsLayoutBase.FullLayout);
                }
            }
        }

        private void FetchData() 
        {
            Cursor currentCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            if (activeTable == "EReferences")
            {
                gridControl1.DataSource = FetchEReferencesData(_DBConnection);
                SetupAllControls();
                HideIDs();
            }
            else
            {
                gridControl1.DataSource = FetchOtherTableData(_DBConnection);
                if (activeTable == "CompPrice")
                    SetupComboBox(gridControl1, _DBConnection, "Component");
                if (activeTable == "EStation")
                    SetupPopupContainerEdit();
            }
            this.Cursor = currentCursor;
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveDialog.FilterIndex = 0;

            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                gridView1.ExportToXlsx(saveDialog.FileName);
            }
        }
 
        private void addNewRowButton_Click(object sender, EventArgs e)
        {
            DataTable activeTableStructure = GetActiveTableStruct();
            using (AddNewRowForm addForm = new AddNewRowForm(activeTableStructure, _DBConnection, activeTable))
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    Dictionary<string, object> newValues = addForm.EnteredValues;
                    SaveNewRowToDatabase(newValues);
                    refreshButton_Click(sender, e);
                }
            }
        }

        public void SaveNewRowToDatabase(Dictionary<string, object> columnData)
        {
            List<string> valuesList = new List<string>();

            DataView dv = gridView1.DataSource as DataView;
            DataTable dt = dv.Table;

            valuesList = ControlInput(columnData, dt);

            string columns = string.Join(", ", columnData.Keys);
            string values = string.Join(", ", valuesList);

            string query = $"INSERT INTO [dbo].[{activeTable}] ({columns}) VALUES ({values})";

            try
            {
                string result = Helpers.HelperClass.callCmd(_DBConnection, query);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("You've successfully created a new row.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error while inserting data: " + result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Display any other exception
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> ControlInput(Dictionary<string, object> columnData, DataTable dt)
        {
            var valuesList = new List<string>();
            foreach (var entry in columnData)
            {
                if (!entry.Key.Contains("ID"))
                {
                    Type columnType = dt.Columns[entry.Key].DataType;

                    if (entry.Value == null)
                    {
                        valuesList.Add("NULL");
                    }
                    else if (string.IsNullOrEmpty(entry.Value.ToString()) && (columnType == typeof(int) || columnType == typeof(float) || columnType == typeof(double)))
                    {
                        valuesList.Add("NULL");
                    }
                    else if (columnType == typeof(int) || columnType == typeof(float) || columnType == typeof(double))
                    {
                        valuesList.Add(entry.Value.ToString());
                    }
                    else if (columnType == typeof(string) && string.IsNullOrEmpty(entry.Value.ToString()))
                    {
                        valuesList.Add("NULL");
                    }
                    else
                    {
                        valuesList.Add($"'{entry.Value}'");
                    }
                }
                else
                    valuesList.Add(entry.Value.ToString());
            }
            return valuesList;
        }

        private DataTable GetActiveTableStruct()
        {
            if (gridControl1.DataSource is DataTable dataTable)
            {
                DataTable clonedTable = dataTable.Clone();
                if (clonedTable.Columns.Contains("ID"))
                    clonedTable.Columns.Remove("ID");
                if (activeTable == "EStation")
                {
                    clonedTable.Columns.Add("NameOfLine");
                    clonedTable.Columns.Add("NameOfStation");
                }
                foreach (var columnName in clonedTable.Columns)
                {
                    Console.WriteLine(columnName);
                }
                return clonedTable;
            }

            return null;
        }

        //Deleting row from database 
        private void Delete_Click(object sender, EventArgs e)
        {
            if (gridView1.GetSelectedRows().Length > 0)
            {
                if (ConfirmDelete()) 
                {
                    DataRowView selectedRow = (DataRowView)gridView1.GetRow(gridView1.GetSelectedRows()[0]);
                    int selectedID = Convert.ToInt32(selectedRow["ID"]);

                    string result = DeleteFromTable(selectedID);
                    if (result == "")
                    {                         
                        MessageBox.Show("You've successfully deleted selected row.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        refreshButton_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Error while deleting data: " + result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private string DeleteFromTable(int id)
        {
            string deleteQuery = $"DELETE FROM [dbo].[{activeTable}] WHERE ID = {id}";
            return Helpers.HelperClass.callCmd(_DBConnection, deleteQuery);
        }


        private void pickOtherButton_Click(object sender, EventArgs e)
        {
            string selectedValue = null;
            using (OtherTableSelector selectionForm = new OtherTableSelector())
            {
                selectionForm.ActiveTableName = activeTable;
                selectionForm.ShowDialog();

                selectedValue = selectionForm.SelectedValue;
            }

            if (selectedValue != null)
            {
                lastOpenedTable = selectedValue;
                //activeTable = selectedValue;
                TableLabel.Text = $"{lastOpenedTable}";
                refreshButton_Click(sender, e);
            }
        }

        // Fetch data functions 
        public DataTable FetchEReferencesData(SqlConnection dbConnection)
        {
            string query = $@"SELECT t1.ID, t1.PartNumber, t1.NIP, t1.Comp_ID, t1.Estation_ID, t1.No_Comp, t1.Index_page,
                            t2.ID, t2.Name as NameOfProject,
                            t3.Component, t3.NameOfComp, 
                            t4.Name AS NameOfEStation , t4.SerialNumber, t4.ID_Line, t4.ID_Station,
                            t5.Name1 AS NameOfLine, 
                            t6.Name AS NameOfStation
                     FROM [dbo].[EReferences] t1
                     left join [dbo].[Project] t2 on t1.Project_ID = t2.ID
                     left join [dbo].[CompPrice] t3 on t1.Comp_ID = t3.ID
                     left join [dbo].[EStation] t4 on t1.Estation_ID = t4.ID
                     left join [dbo].[Line] t5 on t4.ID_Line = t5.ID
                     left join [dbo].[Station] t6 on t4.ID_Station = t6.ID";
            return Helpers.HelperClass.callQuery(dbConnection, query);
        }

        private DataTable FetchOtherTableData(SqlConnection dbConnection)
        {
            DataTable dataTable = new DataTable();
            string query = $"SELECT * FROM [dbo].[{activeTable}]";

            return Helpers.HelperClass.callQuery(dbConnection, query);

        }

        // Methods for editing 
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DataRow row = gridView.GetDataRow(e.RowHandle);
            if (row != null && ConfirmUpdate() && e.Column.FieldName != "NameOfEstation")
            {
                DataView dv = gridView1.DataSource as DataView;
                DataTable dt = dv.Table;
                if (activeTable == "EReferences")
                    EReferencesEdit(row, e.Column.FieldName, dt);
                else
                    OtherTableEdit(row, e.Column.FieldName);
                refreshButton_Click(sender, e);
            }
        }

        private string ControlInput(string columnName, object value, DataTable dt)
        {
            Type columnType = dt.Columns[columnName].DataType;

            if (value == null)
            {
                return "NULL";
            }
            else if (string.IsNullOrEmpty(value.ToString()) && (columnType == typeof(int) || columnType == typeof(float) || columnType == typeof(double)))
            {
                return "NULL";
            }
            else if (columnType == typeof(int) || columnType == typeof(float) || columnType == typeof(double))
            {
                return value.ToString();
            }
            else if (columnType == typeof(string) && string.IsNullOrEmpty(value.ToString()))
            {
                return "NULL";
            }
            else
            {
                return $"'{value}'".Trim();
            }
        }

        private void EReferencesEdit(DataRow row, string editedColumnName, DataTable dt)
        {

            int id = Convert.ToInt32(row["ID"]);
            string query = "";

            string formattedValue = ControlInput(editedColumnName, row[editedColumnName], dt);

            if (editedColumnName == "Component")
            {
                query = $@"UPDATE EReferences 
                   SET Comp_ID = (SELECT ID FROM CompPrice WHERE Component = {formattedValue})
                   WHERE ID = {id}";
            }
            else if (editedColumnName == "NameOfProject")
            {
                query = $@"UPDATE EReferences 
                   SET Project_ID = (SELECT ID FROM Project WHERE Name = {formattedValue})
                   WHERE ID = {id}";
            }
            else if (editedColumnName == "NameOfEStation")
            {
                query = $@"UPDATE EReferences 
                   SET Estation_ID = {EStationId}
                   WHERE ID = {id}";
            }
            else
            {
                query = $"UPDATE EReferences SET {editedColumnName} = {formattedValue} WHERE ID = {id}";
            }

            string result = Helpers.HelperClass.callCmd(_DBConnection, query);
            if (result == "")
                MessageBox.Show("You've successfully edited selected row.", "Successful Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Error while editing data: " + result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OtherTableEdit(DataRow row, string editedColumnName)
        {
            DataView dv = gridView1.DataSource as DataView;
            DataTable dt = dv.Table;
            string value = ControlInput(editedColumnName, row[editedColumnName], dt);
            string updateQuery = $"UPDATE [dbo].[{activeTable}] SET {editedColumnName} = {value} WHERE ID = {row["ID"]}";

            string result = Helpers.HelperClass.callCmd(_DBConnection, updateQuery);
            if (result == "")
                MessageBox.Show("You've successfully edited selected row.", "Successful Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Error while editing data: " + result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetupAllControls()
        {
            SetupComboBox(gridControl1, _DBConnection, "Component");
            SetupComboBox(gridControl1, _DBConnection, "PartNumber");
            SetupComboBox(gridControl1, _DBConnection, "NameOfProject");
            SetupPopupContainerEdit();
        }

        private void SetupPopupContainerEdit()
        {
            GridView view = gridControl1.MainView as GridView;
            var repoPopup = new RepositoryItemPopupContainerEdit();

            if (activeTable == "EReferences")
            {
                repoPopup.PopupControl = popupContainerControl1;
                view.Columns["NameOfEStation"].ColumnEdit = repoPopup;
                DataRow selectedRow = view.GetDataRow(view.FocusedRowHandle);
                string selectedNameOfLine = selectedRow["NameOfLine"].ToString();
                EStationControl.DataSource = FetchEStationData("popup", selectedNameOfLine);

                EStationView.Columns["Name"].Width = 300;
                EStationView.Columns["NameOfStation"].Width = 150;
                EStationView.Columns["NameOfLine"].Width = 150;
                EStationView.Columns["ID_Station"].Width = 120;
                EStationView.Columns["SerialNumber"].Width = 130;
            }
            else if (activeTable == "EStation")
            {
                var repoPopup2 = new RepositoryItemPopupContainerEdit();
                repoPopup.PopupControl = popupContainerControl2;
                repoPopup2.PopupControl = popupContainerControl3;

                EStationControl.Visible = true;
                LineControl.Visible = true;

                view.Columns["ID_Line"].ColumnEdit = repoPopup;
                view.Columns["ID_Station"].ColumnEdit = repoPopup2;

                LineControl.DataSource = FetchLineData();
                StationControl.DataSource = FetchStationData();
            }


            // Handle the EditValueChanged event
            EStationControl.DoubleClick -= GridControl2_DoubleClick;
            EStationControl.DoubleClick += GridControl2_DoubleClick;

            LineControl.DoubleClick -= GridControl2_DoubleClick;
            LineControl.DoubleClick += GridControl2_DoubleClick;

            StationControl.DoubleClick -= GridControl2_DoubleClick;
            StationControl.DoubleClick += GridControl2_DoubleClick;
        }

        private void GridControl2_DoubleClick(object sender, EventArgs e)
        {
            GridControl control = sender as GridControl; // Cast the sender to GridControl
            GridView view = control.MainView as GridView; // Get the main view of the triggering control

            DataRow row = view.GetDataRow(view.FocusedRowHandle);

            GridView mainView = gridControl1.MainView as GridView;
            var edit = mainView.ActiveEditor as PopupContainerEdit;

            if (edit != null && edit.IsPopupOpen && row != null)
            {
                if (ConfirmUpdate())
                {
                    if (activeTable == "EReferences")
                        editPopupEReferences(row, mainView);
                    else if (activeTable == "EStation")
                    {
                        editPopupEStation(row, mainView);
                    }

                    // Close the PopupContainerEdit
                    if (edit != null)
                    {
                        edit.ClosePopup();
                    }

                    refreshButton_Click(sender, e);
                }
            }
        }

        private void editPopupEStation(DataRow row, GridView mainView)
        {
            DataRow mainRow = mainView.GetDataRow(mainView.FocusedRowHandle);
            string editedColumnName = mainView.FocusedColumn.FieldName;
            if (editedColumnName == "ID_Line")
            {
                LineId = Convert.ToInt32(row["ID"]);
                mainRow["ID_Line"] = row["ID"];
            } 
            else if (editedColumnName == "ID_Station")
            {
                StationId = Convert.ToInt32(row["ID"]);
                mainRow["ID_Station"] = row["ID"];
            }

            OtherTableEdit(mainRow, editedColumnName);
        }

        private void editPopupEReferences(DataRow row, GridView mainView) 
        {
            // Get the ID and Name from the selected row
            EStationId = Convert.ToInt32(row["ID"]);

            string selectedName = row["Name"].ToString();

            // Update the main grid's data source
            DataRow mainRow = mainView.GetDataRow(mainView.FocusedRowHandle);
            mainRow["NameOfEStation"] = selectedName;

            DataView dv = gridView1.DataSource as DataView;
            DataTable dt = dv.Table;

            // Invoke the EReferencesEdit function
            EReferencesEdit(mainRow, "NameOfEStation", dt);
        }

        private DataTable FetchStationData()
        {
            string query = $"SELECT * FROM [dbo].[Station] ORDER BY Name";
            return Helpers.HelperClass.callQuery(_DBConnection, query);
        }

        private DataTable FetchLineData()
        {
            string query = $"SELECT * FROM [dbo].[Line] ORDER BY Name1";
            return Helpers.HelperClass.callQuery(_DBConnection, query);
        }

        private DataTable FetchEStationData(string popupOrNot = null, string nameOfLine = null)
        {
            if (popupOrNot == "popup")
            {
                lastFetchedNameOfLine = nameOfLine;

                // Use this value in the SQL query
                string query = $@"SELECT t2.Name1 AS NameOfLine,
                  t3.Name AS NameOfStation,
                  t1.*
                  FROM [dbo].[EStation] t1
                  JOIN [dbo].[Line] t2 ON t2.ID = t1.ID_Line
                  JOIN [dbo].[Station] t3 ON t3.ID = t1.ID_Station
                  WHERE t2.Name1 = @nameOfLine";  // Filter by the selected NameOfLine

                // Use parameterized query to avoid SQL injection
                using (SqlCommand cmd = new SqlCommand(query, _DBConnection))
                {
                    cmd.Parameters.AddWithValue("@nameOfLine", nameOfLine);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;

                }
            }
            else
            {
                string query = $@"SELECT t2.Name1 AS NameOfLine,
                  t3.Name AS NameOfStation,
                  t1.*
                  FROM [dbo].[EStation] t1
                  JOIN [dbo].[Line] t2 ON t2.ID = t1.ID_Line
                  JOIN [dbo].[Station] t3 ON t3.ID = t1.ID_Station";
                return Helpers.HelperClass.callQuery(_DBConnection, query);
            }
        }

        public void SetupComboBox(GridControl gridControl, SqlConnection dbConnection, string neededRow)
        {
            // 1. Create the RepositoryItemComboBox
            RepositoryItemComboBox comboBox = CreateComponentComboBox();

            // 2. Populate the comboBox with distinct Component values
            PopulateComponentComboBox(comboBox, dbConnection, neededRow);

            // 3. Assign the comboBox to the Component column in the grid
            AssignComboBoxToGridColumn(comboBox, gridControl, neededRow);
        }

        //Setting up the combobox for components
        private RepositoryItemComboBox CreateComponentComboBox()
        {
            return new RepositoryItemComboBox();
        }

        private void PopulateComponentComboBox(RepositoryItemComboBox comboBox, SqlConnection dbConnection, string neededRow)
        {
            List<string> componentValues = ReturnAllValues(dbConnection, neededRow);
            foreach (string value in componentValues)
            {
                comboBox.Items.Add(value);
            }
        }

        private void AssignComboBoxToGridColumn(RepositoryItemComboBox comboBox, GridControl gridControl, string neededRow)
        {
            GridView view = gridControl.MainView as GridView;
            if (view != null)
            {
                view.Columns[neededRow].ColumnEdit = comboBox;
            }
        }

        //HelperMethods
        private void HideIDs()
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn column in gridView1.Columns)
            {
                if (column.FieldName.Contains("ID"))
                {
                    column.Visible = false;
                    column.OptionsColumn.AllowEdit = false;
                }
            }
        }

        public List<string> ReturnAllValues(SqlConnection dbConnection, string neededRow)
        {
            List<string> distinctValues = new List<string>();
            string activeTable = "CompPrice";
            if (neededRow == "PartNumber")
                activeTable = "EReferences";
            if (neededRow == "NameOfProject") 
            {
                activeTable = "Project";
                neededRow = "Name";
            }
            string query = $"SELECT DISTINCT {neededRow} FROM [dbo].[{activeTable}]";

            DataTable dt = Helpers.HelperClass.callQuery(dbConnection, query);

            foreach (DataRow row in dt.Rows)
            {
                if (row[neededRow] != DBNull.Value)
                {
                    distinctValues.Add(row[neededRow].ToString());
                }
            }

            return distinctValues;
        }
        //Block editing function
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view != null)
            {
                List<string> disabledColumns = new List<string>() { "NameOfComp", "NameOfLine", "NameOfStation", "SerialNumber" };
                if (disabledColumns.Contains(view.FocusedColumn.FieldName) && activeTable == "EReferences")
                {
                    e.Cancel = true; // Prevents editing
                }
            } 
        }
        //Fetch estation each time focused row is changed
        private void MainView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView mainView = sender as GridView;
            DataRow selectedRow = mainView.GetDataRow(e.FocusedRowHandle);

            // Ensure the selectedRow is not null
            if (selectedRow != null && activeTable == "EReferences")
            {
                string selectedNameOfLine = selectedRow["NameOfLine"].ToString();

                // Check if the selected NameOfLine has changed
                if (selectedNameOfLine != lastFetchedNameOfLine)
                {
                    // Update the last fetched NameOfLine
                    lastFetchedNameOfLine = selectedNameOfLine;

                    // Fetch data based on the selected NameOfLine and set it as the data source for your PopupContainerEdit
                    EStationControl.DataSource = FetchEStationData("popup", selectedNameOfLine);
                }
            }
        }

        private void BMOForm_Shown(object sender, EventArgs e)
        {
            gridControl1.DataSource = FetchEReferencesData(_DBConnection);
            SetupAllControls();
            HideIDs();
        }

        //Confirmations or UpdatesMessage
        public static bool ConfirmUpdate()
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to update this record?", "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return dialogResult == DialogResult.Yes;
        }

        public static bool ConfirmDelete()
        { 
            DialogResult dialog = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return dialog == DialogResult.Yes;
        }

    }
}
