namespace SocketServer
{
    partial class FormRadar
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
            this.pictureBox_radar = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_radar)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_radar
            // 
            this.pictureBox_radar.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_radar.Location = new System.Drawing.Point(13, 69);
            this.pictureBox_radar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox_radar.Name = "pictureBox_radar";
            this.pictureBox_radar.Size = new System.Drawing.Size(324, 488);
            this.pictureBox_radar.TabIndex = 1;
            this.pictureBox_radar.TabStop = false;
            this.pictureBox_radar.Click += new System.EventHandler(this.pictureBox_radar_Click);
            this.pictureBox_radar.MouseHover += new System.EventHandler(this.pictureBox_radar_MouseHover);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.label1.Location = new System.Drawing.Point(327, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 38);
            this.label1.TabIndex = 2;
            this.label1.Text = "A型采样";
            // 
            // FormRadar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(806, 736);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox_radar);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormRadar";
            this.ShowInTaskbar = false;
            this.Text = "FormRadar";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Black;
            this.Shown += new System.EventHandler(this.FormRadar_Shown);
            this.Click += new System.EventHandler(this.FormRadar_Click);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form3_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_radar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_radar;
        private System.Windows.Forms.Label label1;
    }
}