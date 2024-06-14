namespace DNS_Simulation
{
    partial class LanIPAsk
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
            serverIPInput = new TextBox();
            submitIPButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 36);
            label1.Name = "label1";
            label1.Size = new Size(226, 20);
            label1.TabIndex = 0;
            label1.Text = "Where is your server listening at?";
            // 
            // serverIPInput
            // 
            serverIPInput.Location = new Point(27, 59);
            serverIPInput.Name = "serverIPInput";
            serverIPInput.Size = new Size(437, 27);
            serverIPInput.TabIndex = 1;
            // 
            // submitIPButton
            // 
            submitIPButton.Location = new Point(198, 109);
            submitIPButton.Name = "submitIPButton";
            submitIPButton.Size = new Size(94, 29);
            submitIPButton.TabIndex = 2;
            submitIPButton.Text = "OK";
            submitIPButton.UseVisualStyleBackColor = true;
            submitIPButton.Click += submitIPButton_Click;
            // 
            // LanIPAsk
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(491, 159);
            Controls.Add(submitIPButton);
            Controls.Add(serverIPInput);
            Controls.Add(label1);
            Name = "LanIPAsk";
            Text = "LAN IP Address";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox serverIPInput;
        private Button submitIPButton;
    }
}