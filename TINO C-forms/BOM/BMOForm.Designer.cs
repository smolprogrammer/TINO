
namespace BMO
{
    partial class BMOForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.pickOtherButton = new System.Windows.Forms.Button();
            this.TableLabel = new System.Windows.Forms.Label();
            this.Delete = new System.Windows.Forms.Button();
            this.AddNewButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.EStationControl = new DevExpress.XtraGrid.GridControl();
            this.EStationView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.popupContainerControl1 = new DevExpress.XtraEditors.PopupContainerControl();
            this.popupContainerControl2 = new DevExpress.XtraEditors.PopupContainerControl();
            this.LineControl = new DevExpress.XtraGrid.GridControl();
            this.LineView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.popupContainerControl3 = new DevExpress.XtraEditors.PopupContainerControl();
            this.StationControl = new DevExpress.XtraGrid.GridControl();
            this.StationView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EStationControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EStationView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerControl1)).BeginInit();
            this.popupContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerControl2)).BeginInit();
            this.popupContainerControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LineControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LineView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerControl3)).BeginInit();
            this.popupContainerControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StationControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StationView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.TableLabel);
            this.panel1.Controls.Add(this.pickOtherButton);
            this.panel1.Controls.Add(this.Delete);
            this.panel1.Controls.Add(this.AddNewButton);
            this.panel1.Controls.Add(this.exportButton);
            this.panel1.Controls.Add(this.refreshButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1122, 34);
            this.panel1.TabIndex = 0;
            // 
            // pickOtherButton
            // 
            this.pickOtherButton.Location = new System.Drawing.Point(412, 3);
            this.pickOtherButton.Name = "pickOtherButton";
            this.pickOtherButton.Size = new System.Drawing.Size(150, 28);
            this.pickOtherButton.TabIndex = 2;
            this.pickOtherButton.Text = "Switch to Other Tables";
            this.pickOtherButton.UseVisualStyleBackColor = true;
            this.pickOtherButton.Click += new System.EventHandler(this.pickOtherButton_Click);
            // 
            // TableLabel
            // 
            this.TableLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.TableLabel.AutoSize = true;
            this.TableLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TableLabel.Location = new System.Drawing.Point(987, 7);
            this.TableLabel.Name = "TableLabel";
            this.TableLabel.Size = new System.Drawing.Size(123, 22);
            this.TableLabel.TabIndex = 3;
            this.TableLabel.Text = "EReferences";
            this.TableLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Delete
            // 
            this.Delete.Location = new System.Drawing.Point(309, 3);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(100, 28);
            this.Delete.TabIndex = 5;
            this.Delete.Text = "Delete row";
            this.Delete.UseVisualStyleBackColor = true;
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // AddNewButton
            // 
            this.AddNewButton.Location = new System.Drawing.Point(207, 3);
            this.AddNewButton.Name = "AddNewButton";
            this.AddNewButton.Size = new System.Drawing.Size(100, 28);
            this.AddNewButton.TabIndex = 4;
            this.AddNewButton.Text = "Add New Row";
            this.AddNewButton.UseVisualStyleBackColor = true;
            this.AddNewButton.Click += new System.EventHandler(this.addNewRowButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(105, 3);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(100, 28);
            this.exportButton.TabIndex = 1;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(3, 3);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(100, 28);
            this.refreshButton.TabIndex = 0;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 34);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1122, 457);
            this.gridControl1.TabIndex = 1;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gridView1_ShowingEditor);
            this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.MainView_FocusedRowChanged);
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            // 
            // EStationControl
            // 
            this.EStationControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EStationControl.Location = new System.Drawing.Point(0, 0);
            this.EStationControl.MainView = this.EStationView;
            this.EStationControl.Name = "EStationControl";
            this.EStationControl.Size = new System.Drawing.Size(1018, 296);
            this.EStationControl.TabIndex = 0;
            this.EStationControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.EStationView});
            // 
            // EStationView
            // 
            this.EStationView.GridControl = this.EStationControl;
            this.EStationView.Name = "EStationView";
            this.EStationView.OptionsBehavior.Editable = false;
            // 
            // popupContainerControl1
            // 
            this.popupContainerControl1.Controls.Add(this.EStationControl);
            this.popupContainerControl1.Location = new System.Drawing.Point(12, 232);
            this.popupContainerControl1.Name = "popupContainerControl1";
            this.popupContainerControl1.Size = new System.Drawing.Size(1018, 296);
            this.popupContainerControl1.TabIndex = 2;
            // 
            // popupContainerControl2
            // 
            this.popupContainerControl2.Controls.Add(this.LineControl);
            this.popupContainerControl2.Location = new System.Drawing.Point(439, 37);
            this.popupContainerControl2.Name = "popupContainerControl2";
            this.popupContainerControl2.Size = new System.Drawing.Size(723, 252);
            this.popupContainerControl2.TabIndex = 3;
            // 
            // LineControl
            // 
            this.LineControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LineControl.Location = new System.Drawing.Point(0, 0);
            this.LineControl.MainView = this.LineView;
            this.LineControl.Name = "LineControl";
            this.LineControl.Size = new System.Drawing.Size(723, 252);
            this.LineControl.TabIndex = 0;
            this.LineControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.LineView});
            this.LineControl.Visible = false;
            // 
            // LineView
            // 
            this.LineView.GridControl = this.LineControl;
            this.LineView.Name = "LineView";
            this.LineView.OptionsBehavior.Editable = false;
            // 
            // popupContainerControl3
            // 
            this.popupContainerControl3.Controls.Add(this.StationControl);
            this.popupContainerControl3.Location = new System.Drawing.Point(363, 19);
            this.popupContainerControl3.Name = "popupContainerControl3";
            this.popupContainerControl3.Size = new System.Drawing.Size(415, 193);
            this.popupContainerControl3.TabIndex = 4;
            // 
            // StationControl
            // 
            this.StationControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StationControl.Location = new System.Drawing.Point(0, 0);
            this.StationControl.MainView = this.StationView;
            this.StationControl.Name = "StationControl";
            this.StationControl.Size = new System.Drawing.Size(415, 193);
            this.StationControl.TabIndex = 0;
            this.StationControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.StationView});
            // 
            // StationView
            // 
            this.StationView.GridControl = this.StationControl;
            this.StationView.Name = "StationView";
            this.StationView.OptionsBehavior.Editable = false;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(859, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 22);
            this.label1.TabIndex = 6;
            this.label1.Text = "Active Table :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BMOForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 491);
            this.Controls.Add(this.popupContainerControl3);
            this.Controls.Add(this.popupContainerControl2);
            this.Controls.Add(this.popupContainerControl1);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.panel1);
            this.Name = "BMOForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BOM Editing";
            this.Shown += new System.EventHandler(this.BMOForm_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EStationControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EStationView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerControl1)).EndInit();
            this.popupContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerControl2)).EndInit();
            this.popupContainerControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LineControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LineView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerControl3)).EndInit();
            this.popupContainerControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.StationControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StationView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button pickOtherButton;
        private System.Windows.Forms.Label TableLabel;
        private System.Windows.Forms.Button Delete;
        private System.Windows.Forms.Button AddNewButton;
        private DevExpress.XtraGrid.GridControl EStationControl;
        private DevExpress.XtraGrid.Views.Grid.GridView EStationView;
        private DevExpress.XtraEditors.PopupContainerControl popupContainerControl1;
        private DevExpress.XtraEditors.PopupContainerControl popupContainerControl2;
        private DevExpress.XtraGrid.GridControl LineControl;
        private DevExpress.XtraGrid.Views.Grid.GridView LineView;
        private DevExpress.XtraEditors.PopupContainerControl popupContainerControl3;
        private DevExpress.XtraGrid.GridControl StationControl;
        private DevExpress.XtraGrid.Views.Grid.GridView StationView;
        private System.Windows.Forms.Label label1;
    }
}