namespace DNS_Simulation
{
    partial class Server
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
            serverLog = new ListBox();
            listenButton = new Button();
            serverLogGroup = new GroupBox();
            dataGroup = new GroupBox();
            recordGridView = new DataGridView();
            domain = new DataGridViewTextBoxColumn();
            ttl = new DataGridViewTextBoxColumn();
            ttd = new DataGridViewTextBoxColumn();
            ipAddress = new DataGridViewTextBoxColumn();
            type = new DataGridViewTextBoxColumn();
            value = new DataGridViewTextBoxColumn();
            serverLogGroup.SuspendLayout();
            dataGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)recordGridView).BeginInit();
            SuspendLayout();
            // 
            // serverLog
            // 
            serverLog.FormattingEnabled = true;
            serverLog.HorizontalScrollbar = true;
            serverLog.ItemHeight = 20;
            serverLog.Location = new Point(6, 26);
            serverLog.Name = "serverLog";
            serverLog.SelectionMode = SelectionMode.MultiSimple;
            serverLog.Size = new Size(777, 344);
            serverLog.TabIndex = 0;
            // 
            // listenButton
            // 
            listenButton.BackColor = SystemColors.ActiveCaption;
            listenButton.Location = new Point(347, 396);
            listenButton.Name = "listenButton";
            listenButton.Size = new Size(94, 29);
            listenButton.TabIndex = 1;
            listenButton.Text = "Listen";
            listenButton.UseVisualStyleBackColor = false;
            listenButton.Click += listenButton_Click;
            // 
            // serverLogGroup
            // 
            serverLogGroup.Controls.Add(serverLog);
            serverLogGroup.Controls.Add(listenButton);
            serverLogGroup.Location = new Point(12, 12);
            serverLogGroup.Name = "serverLogGroup";
            serverLogGroup.Size = new Size(789, 456);
            serverLogGroup.TabIndex = 3;
            serverLogGroup.TabStop = false;
            serverLogGroup.Text = "Server Log";
            // 
            // dataGroup
            // 
            dataGroup.Controls.Add(recordGridView);
            dataGroup.Location = new Point(831, 12);
            dataGroup.Name = "dataGroup";
            dataGroup.Size = new Size(789, 456);
            dataGroup.TabIndex = 4;
            dataGroup.TabStop = false;
            dataGroup.Text = "Data";
            // 
            // recordGridView
            // 
            recordGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            recordGridView.Columns.AddRange(new DataGridViewColumn[] { domain, ttl, ttd, ipAddress, type, value });
            recordGridView.Location = new Point(6, 26);
            recordGridView.Name = "recordGridView";
            recordGridView.RowHeadersWidth = 51;
            recordGridView.RowTemplate.Height = 29;
            recordGridView.Size = new Size(777, 344);
            recordGridView.TabIndex = 0;
            // 
            // domain
            // 
            domain.HeaderText = "Domain";
            domain.MinimumWidth = 6;
            domain.Name = "domain";
            domain.Width = 125;
            // 
            // ttl
            // 
            ttl.HeaderText = "TTL";
            ttl.MinimumWidth = 6;
            ttl.Name = "ttl";
            ttl.Width = 125;
            // 
            // ttd
            // 
            ttd.HeaderText = "TTD";
            ttd.MinimumWidth = 6;
            ttd.Name = "ttd";
            ttd.Width = 125;
            // 
            // ipAddress
            // 
            ipAddress.HeaderText = "IP Address";
            ipAddress.MinimumWidth = 6;
            ipAddress.Name = "ipAddress";
            ipAddress.Width = 125;
            // 
            // type
            // 
            type.HeaderText = "Type";
            type.MinimumWidth = 6;
            type.Name = "type";
            type.Width = 125;
            // 
            // value
            // 
            value.HeaderText = "Value";
            value.MinimumWidth = 6;
            value.Name = "value";
            value.Width = 125;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1631, 480);
            Controls.Add(dataGroup);
            Controls.Add(serverLogGroup);
            Name = "Server";
            Text = "Server";
            serverLogGroup.ResumeLayout(false);
            dataGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)recordGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ListBox serverLog;
        private Button listenButton;
        private GroupBox serverLogGroup;
        private GroupBox dataGroup;
        private DataGridView recordGridView;
        private DataGridViewTextBoxColumn domain;
        private DataGridViewTextBoxColumn ttl;
        private DataGridViewTextBoxColumn ttd;
        private DataGridViewTextBoxColumn ipAddress;
        private DataGridViewTextBoxColumn type;
        private DataGridViewTextBoxColumn value;
    }
}