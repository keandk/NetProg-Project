namespace DNS_Simulation
{
    partial class LoadBalancer
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
            loadBalanceLog = new ListBox();
            clearButton = new Button();
            SuspendLayout();
            // 
            // loadBalanceLog
            // 
            loadBalanceLog.FormattingEnabled = true;
            loadBalanceLog.ItemHeight = 20;
            loadBalanceLog.Location = new Point(12, 54);
            loadBalanceLog.Name = "loadBalanceLog";
            loadBalanceLog.Size = new Size(776, 384);
            loadBalanceLog.TabIndex = 0;
            // 
            // clearButton
            // 
            clearButton.BackColor = Color.RosyBrown;
            clearButton.Location = new Point(694, 12);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(94, 29);
            clearButton.TabIndex = 1;
            clearButton.Text = "Clear";
            clearButton.UseVisualStyleBackColor = false;
            clearButton.Click += clearButton_Click;
            // 
            // LoadBalancer
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(clearButton);
            Controls.Add(loadBalanceLog);
            Name = "LoadBalancer";
            Text = "Load Balancer";
            ResumeLayout(false);
        }

        #endregion

        private ListBox loadBalanceLog;
        private Button clearButton;
    }
}