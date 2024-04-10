namespace DNS_Simulation
{
    partial class Client
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            resDisplay = new GroupBox();
            resContainer = new ListBox();
            metricsContainer = new GroupBox();
            atTime = new Label();
            serverLabel = new Label();
            duration = new Label();
            domainInput = new TextBox();
            rcTypeCombo = new ComboBox();
            sendButton = new Button();
            activityLog = new GroupBox();
            activityContainer = new ListBox();
            httpRadioButton = new RadioButton();
            udpRadioButton = new RadioButton();
            createNewClientButton = new Button();
            label3 = new Label();
            destinationCombo = new ComboBox();
            resDisplay.SuspendLayout();
            metricsContainer.SuspendLayout();
            activityLog.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 15);
            label1.Name = "label1";
            label1.Size = new Size(62, 20);
            label1.TabIndex = 0;
            label1.Text = "Domain";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 57);
            label2.Name = "label2";
            label2.Size = new Size(89, 20);
            label2.TabIndex = 1;
            label2.Text = "Record type";
            // 
            // resDisplay
            // 
            resDisplay.Controls.Add(resContainer);
            resDisplay.Location = new Point(21, 104);
            resDisplay.Name = "resDisplay";
            resDisplay.Size = new Size(458, 279);
            resDisplay.TabIndex = 2;
            resDisplay.TabStop = false;
            resDisplay.Text = "Response Display";
            // 
            // resContainer
            // 
            resContainer.FormattingEnabled = true;
            resContainer.HorizontalScrollbar = true;
            resContainer.ItemHeight = 20;
            resContainer.Location = new Point(6, 26);
            resContainer.Name = "resContainer";
            resContainer.Size = new Size(446, 244);
            resContainer.TabIndex = 0;
            // 
            // metricsContainer
            // 
            metricsContainer.Controls.Add(atTime);
            metricsContainer.Controls.Add(serverLabel);
            metricsContainer.Controls.Add(duration);
            metricsContainer.Location = new Point(21, 399);
            metricsContainer.Name = "metricsContainer";
            metricsContainer.Size = new Size(458, 159);
            metricsContainer.TabIndex = 3;
            metricsContainer.TabStop = false;
            metricsContainer.Text = "Metrics";
            // 
            // atTime
            // 
            atTime.AutoSize = true;
            atTime.Location = new Point(6, 106);
            atTime.Name = "atTime";
            atTime.Size = new Size(54, 20);
            atTime.TabIndex = 2;
            atTime.Text = "When: ";
            // 
            // serverLabel
            // 
            serverLabel.AutoSize = true;
            serverLabel.Location = new Point(6, 72);
            serverLabel.Name = "serverLabel";
            serverLabel.Size = new Size(57, 20);
            serverLabel.TabIndex = 1;
            serverLabel.Text = "Server: ";
            // 
            // duration
            // 
            duration.AutoSize = true;
            duration.Location = new Point(6, 38);
            duration.Name = "duration";
            duration.Size = new Size(89, 20);
            duration.TabIndex = 0;
            duration.Text = "Query time: ";
            // 
            // domainInput
            // 
            domainInput.Location = new Point(95, 12);
            domainInput.Name = "domainInput";
            domainInput.Size = new Size(294, 27);
            domainInput.TabIndex = 4;
            // 
            // rcTypeCombo
            // 
            rcTypeCombo.FormattingEnabled = true;
            rcTypeCombo.Items.AddRange(new object[] { "A", "AAAA", "CNAME", "MX", "NS", "PTR", "SOA" });
            rcTypeCombo.Location = new Point(122, 54);
            rcTypeCombo.Name = "rcTypeCombo";
            rcTypeCombo.Size = new Size(127, 28);
            rcTypeCombo.TabIndex = 5;
            // 
            // sendButton
            // 
            sendButton.Location = new Point(562, 27);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(103, 50);
            sendButton.TabIndex = 6;
            sendButton.Text = "SEND";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            // 
            // activityLog
            // 
            activityLog.Controls.Add(activityContainer);
            activityLog.Location = new Point(533, 104);
            activityLog.Name = "activityLog";
            activityLog.Size = new Size(458, 279);
            activityLog.TabIndex = 3;
            activityLog.TabStop = false;
            activityLog.Text = "Actitity Log";
            // 
            // activityContainer
            // 
            activityContainer.FormattingEnabled = true;
            activityContainer.HorizontalScrollbar = true;
            activityContainer.ItemHeight = 20;
            activityContainer.Location = new Point(6, 26);
            activityContainer.Name = "activityContainer";
            activityContainer.Size = new Size(446, 244);
            activityContainer.TabIndex = 0;
            // 
            // httpRadioButton
            // 
            httpRadioButton.AutoSize = true;
            httpRadioButton.Location = new Point(403, 15);
            httpRadioButton.Name = "httpRadioButton";
            httpRadioButton.Size = new Size(65, 24);
            httpRadioButton.TabIndex = 7;
            httpRadioButton.TabStop = true;
            httpRadioButton.Text = "HTTP";
            httpRadioButton.UseVisualStyleBackColor = true;
            // 
            // udpRadioButton
            // 
            udpRadioButton.AutoSize = true;
            udpRadioButton.Location = new Point(474, 15);
            udpRadioButton.Name = "udpRadioButton";
            udpRadioButton.Size = new Size(59, 24);
            udpRadioButton.TabIndex = 8;
            udpRadioButton.TabStop = true;
            udpRadioButton.Text = "UDP";
            udpRadioButton.UseVisualStyleBackColor = true;
            // 
            // createNewClientButton
            // 
            createNewClientButton.Location = new Point(689, 456);
            createNewClientButton.Name = "createNewClientButton";
            createNewClientButton.Size = new Size(162, 50);
            createNewClientButton.TabIndex = 9;
            createNewClientButton.Text = "NEW CLIENT";
            createNewClientButton.UseVisualStyleBackColor = true;
            createNewClientButton.Click += createNewClientButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(277, 57);
            label3.Name = "label3";
            label3.Size = new Size(69, 20);
            label3.TabIndex = 10;
            label3.Text = "To where";
            // 
            // destinationCombo
            // 
            destinationCombo.FormattingEnabled = true;
            destinationCombo.Items.AddRange(new object[] { "Local 1", "Local 2", "Cloudflare", "Google" });
            destinationCombo.Location = new Point(352, 54);
            destinationCombo.Name = "destinationCombo";
            destinationCombo.Size = new Size(127, 28);
            destinationCombo.TabIndex = 11;
            // 
            // Client
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1031, 585);
            Controls.Add(destinationCombo);
            Controls.Add(label3);
            Controls.Add(createNewClientButton);
            Controls.Add(udpRadioButton);
            Controls.Add(httpRadioButton);
            Controls.Add(activityLog);
            Controls.Add(sendButton);
            Controls.Add(rcTypeCombo);
            Controls.Add(domainInput);
            Controls.Add(metricsContainer);
            Controls.Add(resDisplay);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Client";
            Text = "Client";
            resDisplay.ResumeLayout(false);
            metricsContainer.ResumeLayout(false);
            metricsContainer.PerformLayout();
            activityLog.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private GroupBox resDisplay;
        private GroupBox metricsContainer;
        private TextBox domainInput;
        private ComboBox rcTypeCombo;
        private Button sendButton;
        private ListBox resContainer;
        private Label atTime;
        private Label serverLabel;
        private Label duration;
        private GroupBox activityLog;
        private ListBox activityContainer;
        private RadioButton httpRadioButton;
        private RadioButton udpRadioButton;
        private Button createNewClientButton;
        private Label label3;
        private ComboBox destinationCombo;
    }
}
