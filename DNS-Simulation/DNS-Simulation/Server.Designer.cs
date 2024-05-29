﻿namespace DNS_Simulation
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
            clearLogButton = new Button();
            testCheckBox = new CheckBox();
            ipAddressLabel = new Label();
            lanRadioButton = new RadioButton();
            localRadioButton = new RadioButton();
            newClient = new Button();
            dataGroup = new GroupBox();
            recordGridView = new DataGridView();
            domain = new DataGridViewTextBoxColumn();
            ttl = new DataGridViewTextBoxColumn();
            ttd = new DataGridViewTextBoxColumn();
            ipAddress = new DataGridViewTextBoxColumn();
            type = new DataGridViewTextBoxColumn();
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
            serverLog.Size = new Size(904, 344);
            serverLog.TabIndex = 0;
            // 
            // listenButton
            // 
            listenButton.BackColor = SystemColors.ActiveCaption;
            listenButton.Location = new Point(324, 435);
            listenButton.Name = "listenButton";
            listenButton.Size = new Size(112, 38);
            listenButton.TabIndex = 1;
            listenButton.Text = "Listen";
            listenButton.UseVisualStyleBackColor = false;
            listenButton.Click += listenButton_Click;
            // 
            // serverLogGroup
            // 
            serverLogGroup.Controls.Add(clearLogButton);
            serverLogGroup.Controls.Add(testCheckBox);
            serverLogGroup.Controls.Add(ipAddressLabel);
            serverLogGroup.Controls.Add(lanRadioButton);
            serverLogGroup.Controls.Add(localRadioButton);
            serverLogGroup.Controls.Add(serverLog);
            serverLogGroup.Controls.Add(newClient);
            serverLogGroup.Controls.Add(listenButton);
            serverLogGroup.Location = new Point(12, 12);
            serverLogGroup.Name = "serverLogGroup";
            serverLogGroup.Size = new Size(916, 496);
            serverLogGroup.TabIndex = 3;
            serverLogGroup.TabStop = false;
            serverLogGroup.Text = "Server Log";
            // 
            // clearLogButton
            // 
            clearLogButton.BackColor = Color.RosyBrown;
            clearLogButton.Location = new Point(798, 376);
            clearLogButton.Name = "clearLogButton";
            clearLogButton.Size = new Size(112, 38);
            clearLogButton.TabIndex = 5;
            clearLogButton.Text = "Clear";
            clearLogButton.UseVisualStyleBackColor = false;
            clearLogButton.Click += clearButton_Click;
            // 
            // testCheckBox
            // 
            testCheckBox.AutoSize = true;
            testCheckBox.Location = new Point(382, 405);
            testCheckBox.Name = "testCheckBox";
            testCheckBox.Size = new Size(152, 24);
            testCheckBox.TabIndex = 4;
            testCheckBox.Text = "Test load balancer";
            testCheckBox.UseVisualStyleBackColor = true;
            // 
            // ipAddressLabel
            // 
            ipAddressLabel.AutoSize = true;
            ipAddressLabel.Location = new Point(6, 470);
            ipAddressLabel.Name = "ipAddressLabel";
            ipAddressLabel.Size = new Size(85, 20);
            ipAddressLabel.TabIndex = 3;
            ipAddressLabel.Text = "IP Address: ";
            // 
            // lanRadioButton
            // 
            lanRadioButton.AutoSize = true;
            lanRadioButton.Location = new Point(491, 376);
            lanRadioButton.Name = "lanRadioButton";
            lanRadioButton.Size = new Size(58, 24);
            lanRadioButton.TabIndex = 2;
            lanRadioButton.TabStop = true;
            lanRadioButton.Text = "LAN";
            lanRadioButton.UseVisualStyleBackColor = true;
            // 
            // localRadioButton
            // 
            localRadioButton.AutoSize = true;
            localRadioButton.Location = new Point(367, 376);
            localRadioButton.Name = "localRadioButton";
            localRadioButton.Size = new Size(65, 24);
            localRadioButton.TabIndex = 2;
            localRadioButton.TabStop = true;
            localRadioButton.Text = "Local";
            localRadioButton.UseVisualStyleBackColor = true;
            // 
            // newClient
            // 
            newClient.BackColor = SystemColors.ActiveCaption;
            newClient.Location = new Point(480, 435);
            newClient.Name = "newClient";
            newClient.Size = new Size(112, 38);
            newClient.TabIndex = 1;
            newClient.Text = "New Client";
            newClient.UseVisualStyleBackColor = false;
            newClient.Click += newClient_Click;
            // 
            // dataGroup
            // 
            dataGroup.Controls.Add(recordGridView);
            dataGroup.Location = new Point(934, 12);
            dataGroup.Name = "dataGroup";
            dataGroup.Size = new Size(685, 496);
            dataGroup.TabIndex = 4;
            dataGroup.TabStop = false;
            dataGroup.Text = "Data";
            // 
            // recordGridView
            // 
            recordGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            recordGridView.Columns.AddRange(new DataGridViewColumn[] { domain, ttl, ttd, ipAddress, type });
            recordGridView.Dock = DockStyle.Fill;
            recordGridView.Location = new Point(3, 23);
            recordGridView.Name = "recordGridView";
            recordGridView.RowHeadersWidth = 51;
            recordGridView.RowTemplate.Height = 29;
            recordGridView.Size = new Size(679, 470);
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
            ipAddress.HeaderText = "Value";
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
            // Server
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1631, 520);
            Controls.Add(dataGroup);
            Controls.Add(serverLogGroup);
            Name = "Server";
            Text = "Server";
            serverLogGroup.ResumeLayout(false);
            serverLogGroup.PerformLayout();
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
        private RadioButton localRadioButton;
        private RadioButton lanRadioButton;
        private Label ipAddressLabel;
        private Button newClient;
        private CheckBox testCheckBox;
        private Button clearLogButton;
        private DataGridViewTextBoxColumn domain;
        private DataGridViewTextBoxColumn ttl;
        private DataGridViewTextBoxColumn ttd;
        private DataGridViewTextBoxColumn ipAddress;
        private DataGridViewTextBoxColumn type;
    }
}