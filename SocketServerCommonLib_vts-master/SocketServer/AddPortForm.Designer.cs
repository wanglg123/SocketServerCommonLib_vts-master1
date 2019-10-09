namespace SocketServer
{
    partial class AddPortForm
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
            this.m_wndOK = new System.Windows.Forms.Button();
            this.m_wndComboxParity = new System.Windows.Forms.ComboBox();
            this.m_wndComboxStopbits = new System.Windows.Forms.ComboBox();
            this.m_wndComboxDatasize = new System.Windows.Forms.ComboBox();
            this.m_wndComboxRaud = new System.Windows.Forms.ComboBox();
            this.m_wndComboxName = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_wndOK
            // 
            this.m_wndOK.Location = new System.Drawing.Point(168, 197);
            this.m_wndOK.Name = "m_wndOK";
            this.m_wndOK.Size = new System.Drawing.Size(75, 23);
            this.m_wndOK.TabIndex = 21;
            this.m_wndOK.Text = "OK";
            this.m_wndOK.UseVisualStyleBackColor = true;
            // 
            // m_wndComboxParity
            // 
            this.m_wndComboxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_wndComboxParity.FormattingEnabled = true;
            this.m_wndComboxParity.Location = new System.Drawing.Point(106, 156);
            this.m_wndComboxParity.Name = "m_wndComboxParity";
            this.m_wndComboxParity.Size = new System.Drawing.Size(121, 20);
            this.m_wndComboxParity.TabIndex = 20;
            // 
            // m_wndComboxStopbits
            // 
            this.m_wndComboxStopbits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_wndComboxStopbits.FormattingEnabled = true;
            this.m_wndComboxStopbits.Location = new System.Drawing.Point(106, 127);
            this.m_wndComboxStopbits.Name = "m_wndComboxStopbits";
            this.m_wndComboxStopbits.Size = new System.Drawing.Size(121, 20);
            this.m_wndComboxStopbits.TabIndex = 19;
            // 
            // m_wndComboxDatasize
            // 
            this.m_wndComboxDatasize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_wndComboxDatasize.FormattingEnabled = true;
            this.m_wndComboxDatasize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8"});
            this.m_wndComboxDatasize.Location = new System.Drawing.Point(106, 98);
            this.m_wndComboxDatasize.Name = "m_wndComboxDatasize";
            this.m_wndComboxDatasize.Size = new System.Drawing.Size(121, 20);
            this.m_wndComboxDatasize.TabIndex = 18;
            // 
            // m_wndComboxRaud
            // 
            this.m_wndComboxRaud.FormattingEnabled = true;
            this.m_wndComboxRaud.Items.AddRange(new object[] {
            "2400",
            "4800",
            "9600",
            "11520",
            "38400"});
            this.m_wndComboxRaud.Location = new System.Drawing.Point(106, 69);
            this.m_wndComboxRaud.Name = "m_wndComboxRaud";
            this.m_wndComboxRaud.Size = new System.Drawing.Size(121, 20);
            this.m_wndComboxRaud.TabIndex = 17;
            // 
            // m_wndComboxName
            // 
            this.m_wndComboxName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_wndComboxName.FormattingEnabled = true;
            this.m_wndComboxName.Location = new System.Drawing.Point(106, 40);
            this.m_wndComboxName.Name = "m_wndComboxName";
            this.m_wndComboxName.Size = new System.Drawing.Size(121, 20);
            this.m_wndComboxName.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 15;
            this.label5.Text = "Parity:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "StopBits:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "DataSize:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "Raud:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "COM:";
            // 
            // AddPortForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.m_wndOK);
            this.Controls.Add(this.m_wndComboxParity);
            this.Controls.Add(this.m_wndComboxStopbits);
            this.Controls.Add(this.m_wndComboxDatasize);
            this.Controls.Add(this.m_wndComboxRaud);
            this.Controls.Add(this.m_wndComboxName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddPortForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加串口";
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AddPortForm_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_wndOK;
        private System.Windows.Forms.ComboBox m_wndComboxParity;
        private System.Windows.Forms.ComboBox m_wndComboxStopbits;
        private System.Windows.Forms.ComboBox m_wndComboxDatasize;
        private System.Windows.Forms.ComboBox m_wndComboxRaud;
        private System.Windows.Forms.ComboBox m_wndComboxName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}