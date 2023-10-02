
namespace BMO
{
    partial class OtherTableSelector
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.EReferencesButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.EStationButton = new System.Windows.Forms.Button();
            this.StationButton = new System.Windows.Forms.Button();
            this.CompPriceButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.EReferencesButton, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.EStationButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.StationButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.CompPriceButton, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(162, 176);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // EReferencesButton
            // 
            this.EReferencesButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.EReferencesButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EReferencesButton.Location = new System.Drawing.Point(6, 143);
            this.EReferencesButton.Name = "EReferencesButton";
            this.EReferencesButton.Size = new System.Drawing.Size(150, 29);
            this.EReferencesButton.TabIndex = 4;
            this.EReferencesButton.Text = "EReferences";
            this.EReferencesButton.UseVisualStyleBackColor = true;
            this.EReferencesButton.Click += new System.EventHandler(this.EReferencesButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pick a table";
            // 
            // EStationButton
            // 
            this.EStationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.EStationButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EStationButton.Location = new System.Drawing.Point(6, 38);
            this.EStationButton.Name = "EStationButton";
            this.EStationButton.Size = new System.Drawing.Size(150, 29);
            this.EStationButton.TabIndex = 1;
            this.EStationButton.Text = "EStation";
            this.EStationButton.UseVisualStyleBackColor = true;
            this.EStationButton.Click += new System.EventHandler(this.EStationButton_Click);
            // 
            // StationButton
            // 
            this.StationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.StationButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StationButton.Location = new System.Drawing.Point(6, 73);
            this.StationButton.Name = "StationButton";
            this.StationButton.Size = new System.Drawing.Size(150, 29);
            this.StationButton.TabIndex = 2;
            this.StationButton.Text = "Station";
            this.StationButton.UseVisualStyleBackColor = true;
            this.StationButton.Click += new System.EventHandler(this.StationButton_Click);
            // 
            // CompPriceButton
            // 
            this.CompPriceButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CompPriceButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CompPriceButton.Location = new System.Drawing.Point(6, 108);
            this.CompPriceButton.Name = "CompPriceButton";
            this.CompPriceButton.Size = new System.Drawing.Size(150, 29);
            this.CompPriceButton.TabIndex = 3;
            this.CompPriceButton.Text = "CompPrice";
            this.CompPriceButton.UseVisualStyleBackColor = true;
            this.CompPriceButton.Click += new System.EventHandler(this.CompPriceButton_Click);
            // 
            // OtherTableSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(162, 176);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OtherTableSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OtherTableSelector";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button EStationButton;
        private System.Windows.Forms.Button StationButton;
        private System.Windows.Forms.Button CompPriceButton;
        private System.Windows.Forms.Button EReferencesButton;
    }
}