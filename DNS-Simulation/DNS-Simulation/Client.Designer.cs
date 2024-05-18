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
            label1 = new Label();
            domainInput = new TextBox();
            sendButton = new Button();
            responseBox = new ListBox();
            groupBox1 = new GroupBox();
            messageSize = new Label();
            when = new Label();
            server = new Label();
            queryTime = new Label();
            clearButton = new Button();
            label2 = new Label();
            type = new ComboBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 16);
            label1.Name = "label1";
            label1.Size = new Size(62, 20);
            label1.TabIndex = 0;
            label1.Text = "Domain";
            // 
            // domainInput
            // 
            domainInput.Location = new Point(88, 13);
            domainInput.Name = "domainInput";
            domainInput.Size = new Size(194, 27);
            domainInput.TabIndex = 1;
            // 
            // sendButton
            // 
            sendButton.BackColor = SystemColors.ActiveCaption;
            sendButton.Location = new Point(573, 12);
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
            responseBox.Location = new Point(20, 64);
            responseBox.Name = "responseBox";
            responseBox.Size = new Size(768, 224);
            responseBox.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(messageSize);
            groupBox1.Controls.Add(when);
            groupBox1.Controls.Add(server);
            groupBox1.Controls.Add(queryTime);
            groupBox1.Location = new Point(20, 304);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(768, 164);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Summary";
            // 
            // messageSize
            // 
            messageSize.AutoSize = true;
            messageSize.Location = new Point(12, 132);
            messageSize.Name = "messageSize";
            messageSize.Size = new Size(103, 20);
            messageSize.TabIndex = 8;
            messageSize.Text = "Message size: ";
            // 
            // when
            // 
            when.AutoSize = true;
            when.Location = new Point(12, 98);
            when.Name = "when";
            when.Size = new Size(54, 20);
            when.TabIndex = 7;
            when.Text = "When: ";
            // 
            // server
            // 
            server.AutoSize = true;
            server.Location = new Point(12, 65);
            server.Name = "server";
            server.Size = new Size(57, 20);
            server.TabIndex = 6;
            server.Text = "Server: ";
            // 
            // queryTime
            // 
            queryTime.AutoSize = true;
            queryTime.Location = new Point(12, 32);
            queryTime.Name = "queryTime";
            queryTime.Size = new Size(89, 20);
            queryTime.TabIndex = 5;
            queryTime.Text = "Query time: ";
            // 
            // clearButton
            // 
            clearButton.BackColor = Color.RosyBrown;
            clearButton.Location = new Point(694, 12);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(94, 29);
            clearButton.TabIndex = 5;
            clearButton.Text = "Clear";
            clearButton.UseVisualStyleBackColor = false;
            clearButton.Click += clearButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(304, 16);
            label2.Name = "label2";
            label2.Size = new Size(40, 20);
            label2.TabIndex = 6;
            label2.Text = "Type";
            // 
            // type
            // 
            type.FormattingEnabled = true;
            type.Items.AddRange(new object[] { "PTR", "A", "AAAA", "CNAME", "ANY", "MX", "NS", "TXT" });
            type.Location = new Point(350, 13);
            type.Name = "type";
            type.Size = new Size(151, 28);
            type.TabIndex = 7;
            // 
            // Client
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 480);
            Controls.Add(type);
            Controls.Add(label2);
            Controls.Add(clearButton);
            Controls.Add(groupBox1);
            Controls.Add(responseBox);
            Controls.Add(sendButton);
            Controls.Add(domainInput);
            Controls.Add(label1);
            Name = "Client";
            Text = "Client";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox domainInput;
        private Button sendButton;
        private ListBox responseBox;
        private GroupBox groupBox1;
        private Label when;
        private Label server;
        private Label queryTime;
        private Label messageSize;
        private Button clearButton;
        private Label label2;
        private ComboBox type;
    }
}