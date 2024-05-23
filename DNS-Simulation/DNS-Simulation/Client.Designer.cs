using System.Runtime.CompilerServices;

namespace DNS_Simulation
{
    partial class Client
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
            sendButton = new Button();
            responseBox = new ListBox();
            groupBox1 = new GroupBox();
            loadBalancerLabel = new Label();
            when = new Label();
            server = new Label();
            queryTime = new Label();
            clearButton = new Button();
            label3 = new Label();
            loadBalancerIpAddressTextBox = new TextBox();
            label4 = new Label();
            loadBalancerPortTextBox = new TextBox();
            type = new ComboBox();
            label2 = new Label();
            domainInput = new TextBox();
            label1 = new Label();
            connectButton = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // sendButton
            // 
            sendButton.BackColor = SystemColors.ActiveCaption;
            sendButton.Location = new Point(542, 50);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(94, 29);
            sendButton.TabIndex = 2;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = false;
            sendButton.Click += sendButton_Click;
            // 
            // responseBox
            // 
            responseBox.FormattingEnabled = true;
            responseBox.HorizontalScrollbar = true;
            responseBox.ItemHeight = 20;
            responseBox.Location = new Point(20, 84);
            responseBox.Name = "responseBox";
            responseBox.Size = new Size(768, 264);
            responseBox.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(loadBalancerLabel);
            groupBox1.Controls.Add(when);
            groupBox1.Controls.Add(server);
            groupBox1.Controls.Add(queryTime);
            groupBox1.Location = new Point(20, 362);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(768, 164);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Summary";
            // 
            // loadBalancerLabel
            // 
            loadBalancerLabel.AutoSize = true;
            loadBalancerLabel.Location = new Point(10, 133);
            loadBalancerLabel.Name = "loadBalancerLabel";
            loadBalancerLabel.Size = new Size(105, 20);
            loadBalancerLabel.TabIndex = 8;
            loadBalancerLabel.Text = "Connected to: ";
            // 
            // when
            // 
            when.AutoSize = true;
            when.Location = new Point(10, 99);
            when.Name = "when";
            when.Size = new Size(54, 20);
            when.TabIndex = 7;
            when.Text = "When: ";
            // 
            // server
            // 
            server.AutoSize = true;
            server.Location = new Point(10, 66);
            server.Name = "server";
            server.Size = new Size(57, 20);
            server.TabIndex = 6;
            server.Text = "Server: ";
            // 
            // queryTime
            // 
            queryTime.AutoSize = true;
            queryTime.Location = new Point(10, 33);
            queryTime.Name = "queryTime";
            queryTime.Size = new Size(89, 20);
            queryTime.TabIndex = 5;
            queryTime.Text = "Query time: ";
            // 
            // clearButton
            // 
            clearButton.BackColor = Color.RosyBrown;
            clearButton.Location = new Point(663, 50);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(94, 29);
            clearButton.TabIndex = 5;
            clearButton.Text = "Clear";
            clearButton.UseVisualStyleBackColor = false;
            clearButton.Click += clearButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(22, 15);
            label3.Name = "label3";
            label3.Size = new Size(78, 20);
            label3.TabIndex = 0;
            label3.Text = "IP Address";
            // 
            // loadBalancerIpAddressTextBox
            // 
            loadBalancerIpAddressTextBox.Location = new Point(106, 12);
            loadBalancerIpAddressTextBox.Name = "loadBalancerIpAddressTextBox";
            loadBalancerIpAddressTextBox.Size = new Size(205, 27);
            loadBalancerIpAddressTextBox.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(322, 15);
            label4.Name = "label4";
            label4.Size = new Size(35, 20);
            label4.TabIndex = 0;
            label4.Text = "Port";
            // 
            // loadBalancerPortTextBox
            // 
            loadBalancerPortTextBox.Location = new Point(363, 12);
            loadBalancerPortTextBox.Name = "loadBalancerPortTextBox";
            loadBalancerPortTextBox.Size = new Size(140, 27);
            loadBalancerPortTextBox.TabIndex = 1;
            loadBalancerPortTextBox.Text = "8080";
            // 
            // type
            // 
            type.FormattingEnabled = true;
            type.Items.AddRange(new object[] { "A", "AAAA", "CNAME", "ANY", "MX", "NS", "TXT", "PTR" });
            type.Location = new Point(363, 51);
            type.Name = "type";
            type.Size = new Size(140, 28);
            type.TabIndex = 11;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(317, 54);
            label2.Name = "label2";
            label2.Size = new Size(40, 20);
            label2.TabIndex = 10;
            label2.Text = "Type";
            // 
            // domainInput
            // 
            domainInput.Location = new Point(90, 51);
            domainInput.Name = "domainInput";
            domainInput.Size = new Size(221, 27);
            domainInput.TabIndex = 9;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 54);
            label1.Name = "label1";
            label1.Size = new Size(62, 20);
            label1.TabIndex = 8;
            label1.Text = "Domain";
            // 
            // connectButton
            // 
            connectButton.BackColor = SystemColors.ActiveCaption;
            connectButton.Location = new Point(602, 11);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(94, 29);
            connectButton.TabIndex = 2;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += connectButton_Click;
            // 
            // Client
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 538);
            Controls.Add(type);
            Controls.Add(label2);
            Controls.Add(domainInput);
            Controls.Add(label1);
            Controls.Add(clearButton);
            Controls.Add(groupBox1);
            Controls.Add(responseBox);
            Controls.Add(connectButton);
            Controls.Add(sendButton);
            Controls.Add(loadBalancerPortTextBox);
            Controls.Add(label4);
            Controls.Add(loadBalancerIpAddressTextBox);
            Controls.Add(label3);
            Name = "Client";
            Text = "Client";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button sendButton;
        private ListBox responseBox;
        private GroupBox groupBox1;
        private Label when;
        private Label server;
        private Label queryTime;
        private Button clearButton;
        private Label label3;
        private TextBox loadBalancerIpAddressTextBox;
        private Label label4;
        private TextBox loadBalancerPortTextBox;
        private ComboBox type;
        private Label label2;
        private TextBox domainInput;
        private Label label1;
        private Button connectButton;
        private Label loadBalancerLabel;
    }
}