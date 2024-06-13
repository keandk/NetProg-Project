namespace DNS_Simulation
{
    partial class ServerForm
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
            clearButton = new Button();
            ipAddressLabel = new Label();
            serverLog = new ListBox();
            groupBox1 = new GroupBox();
            recordGridView = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            groupBox2 = new GroupBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)recordGridView).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // clearButton
            // 
            clearButton.BackColor = Color.RosyBrown;
            clearButton.Location = new Point(568, 376);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(112, 38);
            clearButton.TabIndex = 5;
            clearButton.Text = "Clear";
            clearButton.UseVisualStyleBackColor = false;
            clearButton.Click += clearButton_Click;
            // 
            // ipAddressLabel
            // 
            ipAddressLabel.AutoSize = true;
            ipAddressLabel.Location = new Point(6, 376);
            ipAddressLabel.Name = "ipAddressLabel";
            ipAddressLabel.Size = new Size(85, 20);
            ipAddressLabel.TabIndex = 3;
            ipAddressLabel.Text = "IP Address: ";
            // 
            // serverLog
            // 
            serverLog.FormattingEnabled = true;
            serverLog.HorizontalScrollbar = true;
            serverLog.ItemHeight = 20;
            serverLog.Location = new Point(6, 26);
            serverLog.Name = "serverLog";
            serverLog.SelectionMode = SelectionMode.MultiSimple;
            serverLog.Size = new Size(674, 344);
            serverLog.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(clearButton);
            groupBox1.Controls.Add(ipAddressLabel);
            groupBox1.Controls.Add(serverLog);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(686, 423);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Server Log";
            // 
            // recordGridView
            // 
            recordGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            recordGridView.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5 });
            recordGridView.Dock = DockStyle.Fill;
            recordGridView.Location = new Point(3, 23);
            recordGridView.Name = "recordGridView";
            recordGridView.RowHeadersWidth = 51;
            recordGridView.RowTemplate.Height = 29;
            recordGridView.Size = new Size(680, 292);
            recordGridView.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Domain";
            dataGridViewTextBoxColumn1.MinimumWidth = 6;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Width = 125;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "TTL";
            dataGridViewTextBoxColumn2.MinimumWidth = 6;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 125;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "TTD";
            dataGridViewTextBoxColumn3.MinimumWidth = 6;
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.Width = 125;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Value";
            dataGridViewTextBoxColumn4.MinimumWidth = 6;
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.Width = 125;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "Type";
            dataGridViewTextBoxColumn5.MinimumWidth = 6;
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.Width = 125;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(recordGridView);
            groupBox2.Location = new Point(12, 453);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(686, 318);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Data";
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(709, 782);
            Controls.Add(groupBox1);
            Controls.Add(groupBox2);
            Name = "ServerForm";
            Text = "Server Form";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)recordGridView).EndInit();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button clearButton;
        private Label ipAddressLabel;
        private ListBox serverLog;
        private GroupBox groupBox1;
        private DataGridView recordGridView;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private GroupBox groupBox2;
    }
}