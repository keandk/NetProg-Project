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
            refreshButton = new Button();
            SuspendLayout();
            // 
            // serverLog
            // 
            serverLog.FormattingEnabled = true;
            serverLog.HorizontalScrollbar = true;
            serverLog.ItemHeight = 15;
            serverLog.Location = new Point(10, 9);
            serverLog.Margin = new Padding(3, 2, 3, 2);
            serverLog.Name = "serverLog";
            serverLog.SelectionMode = SelectionMode.MultiSimple;
            serverLog.Size = new Size(680, 259);
            serverLog.TabIndex = 0;
            // 
            // listenButton
            // 
            listenButton.BackColor = SystemColors.ActiveCaption;
            listenButton.Location = new Point(309, 290);
            listenButton.Margin = new Padding(3, 2, 3, 2);
            listenButton.Name = "listenButton";
            listenButton.Size = new Size(82, 22);
            listenButton.TabIndex = 1;
            listenButton.Text = "Listen";
            listenButton.UseVisualStyleBackColor = false;
            listenButton.Click += listenButton_Click;
            // 
            // refreshButton
            // 
            refreshButton.BackColor = SystemColors.ActiveCaption;
            refreshButton.Location = new Point(465, 290);
            refreshButton.Margin = new Padding(3, 2, 3, 2);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(82, 22);
            refreshButton.TabIndex = 2;
            refreshButton.Text = "Refresh";
            refreshButton.UseVisualStyleBackColor = false;
            refreshButton.Click += refreshButton_Click;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 338);
            Controls.Add(refreshButton);
            Controls.Add(listenButton);
            Controls.Add(serverLog);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Server";
            Text = "Server";
            ResumeLayout(false);
        }

        #endregion

        private ListBox serverLog;
        private Button listenButton;
        private Button refreshButton;
    }
}