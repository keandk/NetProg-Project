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
            type = new ComboBox();
            label2 = new Label();
            domainInput = new TextBox();
            label1 = new Label();
            testToggle = new CheckBox();
            label5 = new Label();
            numOfRequests = new TextBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // sendButton
            // 
            sendButton.BackColor = SystemColors.ActiveCaption;
            sendButton.Location = new Point(672, 12);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(148, 60);
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
            responseBox.Location = new Point(20, 110);
            responseBox.Name = "responseBox";
            responseBox.Size = new Size(800, 384);
            responseBox.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(loadBalancerLabel);
            groupBox1.Controls.Add(when);
            groupBox1.Controls.Add(server);
            groupBox1.Controls.Add(queryTime);
            groupBox1.Location = new Point(20, 500);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(800, 164);
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
            clearButton.Location = new Point(672, 75);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(148, 29);
            clearButton.TabIndex = 5;
            clearButton.Text = "Clear";
            clearButton.UseVisualStyleBackColor = false;
            clearButton.Click += clearButton_Click;
            // 
            // type
            // 
            type.FormattingEnabled = true;
            type.Items.AddRange(new object[] { "A", "AAAA", "CNAME", "ANY", "MX", "NS", "TXT", "PTR" });
            type.Location = new Point(363, 9);
            type.Name = "type";
            type.Size = new Size(140, 28);
            type.TabIndex = 11;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(317, 12);
            label2.Name = "label2";
            label2.Size = new Size(40, 20);
            label2.TabIndex = 10;
            label2.Text = "Type";
            // 
            // domainInput
            // 
            domainInput.Location = new Point(106, 9);
            domainInput.Name = "domainInput";
            domainInput.Size = new Size(205, 27);
            domainInput.TabIndex = 9;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 12);
            label1.Name = "label1";
            label1.Size = new Size(62, 20);
            label1.TabIndex = 8;
            label1.Text = "Domain";
            // 
            // testToggle
            // 
            testToggle.AutoSize = true;
            testToggle.Location = new Point(509, 12);
            testToggle.Name = "testToggle";
            testToggle.Size = new Size(160, 24);
            testToggle.TabIndex = 12;
            testToggle.Text = "Test load balancing";
            testToggle.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(408, 48);
            label5.Name = "label5";
            label5.Size = new Size(95, 20);
            label5.TabIndex = 8;
            label5.Text = "# of requests";
            // 
            // numOfRequests
            // 
            numOfRequests.Location = new Point(509, 45);
            numOfRequests.Name = "numOfRequests";
            numOfRequests.Size = new Size(157, 27);
            numOfRequests.TabIndex = 9;
            // 
            // Client
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(832, 676);
            Controls.Add(testToggle);
            Controls.Add(type);
            Controls.Add(label2);
            Controls.Add(numOfRequests);
            Controls.Add(domainInput);
            Controls.Add(label5);
            Controls.Add(label1);
            Controls.Add(clearButton);
            Controls.Add(groupBox1);
            Controls.Add(responseBox);
            Controls.Add(sendButton);
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
        private ComboBox type;
        private Label label2;
        private TextBox domainInput;
        private Label label1;
        private Label loadBalancerLabel;
        private CheckBox testToggle;
        private Label label5;
        private TextBox numOfRequests;
    }
}