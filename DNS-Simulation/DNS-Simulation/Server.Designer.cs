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
            SuspendLayout();
            // 
            // serverLog
            // 
            serverLog.FormattingEnabled = true;
            serverLog.HorizontalScrollbar = true;
            serverLog.ItemHeight = 20;
            serverLog.Location = new Point(12, 12);
            serverLog.Name = "serverLog";
            serverLog.SelectionMode = SelectionMode.MultiSimple;
            serverLog.Size = new Size(776, 344);
            serverLog.TabIndex = 0;
            // 
            // listenButton
            // 
            listenButton.BackColor = SystemColors.ActiveCaption;
            listenButton.Location = new Point(353, 386);
            listenButton.Name = "listenButton";
            listenButton.Size = new Size(94, 29);
            listenButton.TabIndex = 1;
            listenButton.Text = "Listen";
            listenButton.UseVisualStyleBackColor = false;
            listenButton.Click += listenButton_Click;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listenButton);
            Controls.Add(serverLog);
            Name = "Server";
            Text = "Server";
            ResumeLayout(false);
        }

        #endregion

        private ListBox serverLog;
        private Button listenButton;
    }
}