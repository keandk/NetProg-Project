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
            msgSize = new Label();
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
            resDisplay.SuspendLayout();
            metricsContainer.SuspendLayout();
            activityLog.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 19);
            label1.Name = "label1";
            label1.Size = new Size(62, 20);
            label1.TabIndex = 0;
            label1.Text = "Domain";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(21, 61);
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
            resDisplay.Size = new Size(347, 279);
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
            resContainer.Size = new Size(330, 244);
            resContainer.TabIndex = 0;
            // 
            // metricsContainer
            // 
            metricsContainer.Controls.Add(msgSize);
            metricsContainer.Controls.Add(atTime);
            metricsContainer.Controls.Add(serverLabel);
            metricsContainer.Controls.Add(duration);
            metricsContainer.Location = new Point(21, 399);
            metricsContainer.Name = "metricsContainer";
            metricsContainer.Size = new Size(716, 159);
            metricsContainer.TabIndex = 3;
            metricsContainer.TabStop = false;
            metricsContainer.Text = "Metrics";
            // 
            // msgSize
            // 
            msgSize.AutoSize = true;
            msgSize.Location = new Point(12, 125);
            msgSize.Name = "msgSize";
            msgSize.Size = new Size(103, 20);
            msgSize.TabIndex = 3;
            msgSize.Text = "Message size: ";
            // 
            // atTime
            // 
            atTime.AutoSize = true;
            atTime.Location = new Point(12, 91);
            atTime.Name = "atTime";
            atTime.Size = new Size(54, 20);
            atTime.TabIndex = 2;
            atTime.Text = "When: ";
            // 
            // serverLabel
            // 
            serverLabel.AutoSize = true;
            serverLabel.Location = new Point(12, 57);
            serverLabel.Name = "serverLabel";
            serverLabel.Size = new Size(57, 20);
            serverLabel.TabIndex = 1;
            serverLabel.Text = "Server: ";
            // 
            // duration
            // 
            duration.AutoSize = true;
            duration.Location = new Point(12, 23);
            duration.Name = "duration";
            duration.Size = new Size(89, 20);
            duration.TabIndex = 0;
            duration.Text = "Query time: ";
            // 
            // domainInput
            // 
            domainInput.Location = new Point(89, 16);
            domainInput.Name = "domainInput";
            domainInput.Size = new Size(279, 27);
            domainInput.TabIndex = 4;
            // 
            // rcTypeCombo
            // 
            rcTypeCombo.FormattingEnabled = true;
            rcTypeCombo.Items.AddRange(new object[] { "A", "AAAA", "CNAME", "MX", "NS", "PTR", "SOA" });
            rcTypeCombo.Location = new Point(116, 58);
            rcTypeCombo.Name = "rcTypeCombo";
            rcTypeCombo.Size = new Size(127, 28);
            rcTypeCombo.TabIndex = 5;
            // 
            // sendButton
            // 
            sendButton.Location = new Point(445, 27);
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
            activityLog.Location = new Point(390, 104);
            activityLog.Name = "activityLog";
            activityLog.Size = new Size(347, 279);
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
            activityContainer.Size = new Size(330, 244);
            activityContainer.TabIndex = 0;
            // 
            // httpRadioButton
            // 
            httpRadioButton.AutoSize = true;
            httpRadioButton.Location = new Point(265, 59);
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
            udpRadioButton.Location = new Point(336, 59);
            udpRadioButton.Name = "udpRadioButton";
            udpRadioButton.Size = new Size(59, 24);
            udpRadioButton.TabIndex = 8;
            udpRadioButton.TabStop = true;
            udpRadioButton.Text = "UDP";
            udpRadioButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 585);
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
            Name = "Form1";
            Text = "Form1";
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
        private Label msgSize;
        private Label atTime;
        private Label serverLabel;
        private Label duration;
        private GroupBox activityLog;
        private ListBox activityContainer;
        private RadioButton httpRadioButton;
        private RadioButton udpRadioButton;
    }
}
