namespace DNS_Simulation
{
    partial class ServerControl
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
            localRadioButton = new RadioButton();
            newClientButton = new Button();
            lanRadioButton = new RadioButton();
            listenButton = new Button();
            serverNumDropDown = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            clientNumDropDown = new NumericUpDown();
            newServerButton = new Button();
            testBalancerCheckBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)serverNumDropDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)clientNumDropDown).BeginInit();
            SuspendLayout();
            // 
            // localRadioButton
            // 
            localRadioButton.AutoSize = true;
            localRadioButton.Location = new Point(157, 178);
            localRadioButton.Name = "localRadioButton";
            localRadioButton.Size = new Size(65, 24);
            localRadioButton.TabIndex = 2;
            localRadioButton.TabStop = true;
            localRadioButton.Text = "Local";
            localRadioButton.UseVisualStyleBackColor = true;
            // 
            // newClientButton
            // 
            newClientButton.BackColor = SystemColors.ActiveCaption;
            newClientButton.Location = new Point(192, 208);
            newClientButton.Name = "newClientButton";
            newClientButton.Size = new Size(112, 38);
            newClientButton.TabIndex = 1;
            newClientButton.Text = "New Client";
            newClientButton.UseVisualStyleBackColor = false;
            newClientButton.Click += newClientButton_Click;
            // 
            // lanRadioButton
            // 
            lanRadioButton.AutoSize = true;
            lanRadioButton.Location = new Point(281, 178);
            lanRadioButton.Name = "lanRadioButton";
            lanRadioButton.Size = new Size(58, 24);
            lanRadioButton.TabIndex = 2;
            lanRadioButton.TabStop = true;
            lanRadioButton.Text = "LAN";
            lanRadioButton.UseVisualStyleBackColor = true;
            // 
            // listenButton
            // 
            listenButton.BackColor = SystemColors.ActiveCaption;
            listenButton.Location = new Point(40, 208);
            listenButton.Name = "listenButton";
            listenButton.Size = new Size(112, 38);
            listenButton.TabIndex = 1;
            listenButton.Text = "Listen";
            listenButton.UseVisualStyleBackColor = false;
            listenButton.Click += listenButton_Click;
            // 
            // serverNumDropDown
            // 
            serverNumDropDown.Location = new Point(252, 66);
            serverNumDropDown.Name = "serverNumDropDown";
            serverNumDropDown.Size = new Size(150, 27);
            serverNumDropDown.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(96, 68);
            label1.Name = "label1";
            label1.Size = new Size(130, 20);
            label1.TabIndex = 6;
            label1.Text = "Number of servers";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(95, 113);
            label2.Name = "label2";
            label2.Size = new Size(127, 20);
            label2.TabIndex = 6;
            label2.Text = "Number of clients";
            // 
            // clientNumDropDown
            // 
            clientNumDropDown.Location = new Point(252, 111);
            clientNumDropDown.Name = "clientNumDropDown";
            clientNumDropDown.Size = new Size(150, 27);
            clientNumDropDown.TabIndex = 5;
            // 
            // newServerButton
            // 
            newServerButton.BackColor = SystemColors.ActiveCaption;
            newServerButton.Location = new Point(344, 208);
            newServerButton.Name = "newServerButton";
            newServerButton.Size = new Size(112, 38);
            newServerButton.TabIndex = 1;
            newServerButton.Text = "New Server";
            newServerButton.UseVisualStyleBackColor = false;
            newServerButton.Click += newServerButton_Click;
            // 
            // testBalancerCheckBox
            // 
            testBalancerCheckBox.AutoSize = true;
            testBalancerCheckBox.Location = new Point(172, 148);
            testBalancerCheckBox.Name = "testBalancerCheckBox";
            testBalancerCheckBox.Size = new Size(152, 24);
            testBalancerCheckBox.TabIndex = 7;
            testBalancerCheckBox.Text = "Test load balancer";
            testBalancerCheckBox.UseVisualStyleBackColor = true;
            // 
            // ServerControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(496, 325);
            Controls.Add(testBalancerCheckBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(clientNumDropDown);
            Controls.Add(serverNumDropDown);
            Controls.Add(listenButton);
            Controls.Add(localRadioButton);
            Controls.Add(lanRadioButton);
            Controls.Add(newServerButton);
            Controls.Add(newClientButton);
            Name = "ServerControl";
            Text = "Server Control";
            ((System.ComponentModel.ISupportInitialize)serverNumDropDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)clientNumDropDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RadioButton localRadioButton;
        private Button newClientButton;
        private RadioButton lanRadioButton;
        private Button listenButton;
        private NumericUpDown serverNumDropDown;
        private Label label1;
        private Label label2;
        private NumericUpDown clientNumDropDown;
        private Button newServerButton;
        private CheckBox testBalancerCheckBox;
    }
}