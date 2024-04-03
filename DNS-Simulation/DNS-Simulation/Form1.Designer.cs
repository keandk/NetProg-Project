namespace DNS_Simulation
{
    partial class Form1
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
            msgSizeLabel = new Label();
            atTime = new Label();
            serverLabel = new Label();
            duration = new Label();
            domainInput = new TextBox();
            rcTypeCombo = new ComboBox();
            sendButton = new Button();
            activityLog = new GroupBox();
            activityContainer = new ListBox();
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
            label2.Location = new Point(363, 19);
            label2.Name = "label2";
            label2.Size = new Size(89, 20);
            label2.TabIndex = 1;
            label2.Text = "Record type";
            // 
            // resDisplay
            // 
            resDisplay.Controls.Add(resContainer);
            resDisplay.Location = new Point(21, 61);
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
            metricsContainer.Controls.Add(msgSizeLabel);
            metricsContainer.Controls.Add(atTime);
            metricsContainer.Controls.Add(serverLabel);
            metricsContainer.Controls.Add(duration);
            metricsContainer.Location = new Point(21, 356);
            metricsContainer.Name = "metricsContainer";
            metricsContainer.Size = new Size(716, 159);
            metricsContainer.TabIndex = 3;
            metricsContainer.TabStop = false;
            metricsContainer.Text = "Metrics";
            // 
            // msgSizeLabel
            // 
            msgSizeLabel.AutoSize = true;
            msgSizeLabel.Location = new Point(12, 125);
            msgSizeLabel.Name = "msgSizeLabel";
            msgSizeLabel.Size = new Size(103, 20);
            msgSizeLabel.TabIndex = 3;
            msgSizeLabel.Text = "Message size: ";
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
            domainInput.Size = new Size(268, 27);
            domainInput.TabIndex = 4;
            // 
            // rcTypeCombo
            // 
            rcTypeCombo.FormattingEnabled = true;
            rcTypeCombo.Items.AddRange(new object[] { "A", "AAAA", "CNAME", "MX", "NS", "PTR", "SOA" });
            rcTypeCombo.Location = new Point(458, 16);
            rcTypeCombo.Name = "rcTypeCombo";
            rcTypeCombo.Size = new Size(151, 28);
            rcTypeCombo.TabIndex = 5;
            // 
            // sendButton
            // 
            sendButton.Location = new Point(643, 16);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(94, 29);
            sendButton.TabIndex = 6;
            sendButton.Text = "SEND";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            // 
            // activityLog
            // 
            activityLog.Controls.Add(activityContainer);
            activityLog.Location = new Point(390, 61);
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 547);
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
        private Label msgSizeLabel;
        private Label atTime;
        private Label serverLabel;
        private Label duration;
        private GroupBox activityLog;
        private ListBox activityContainer;
    }
}
