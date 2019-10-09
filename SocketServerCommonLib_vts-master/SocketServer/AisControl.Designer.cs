namespace SocketServer
{
    partial class AisControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.m_wndPagesControl = new System.Windows.Forms.TabControl();
            this.m_wndPageRaw = new System.Windows.Forms.TabPage();
            this.m_wndRawDataList = new System.Windows.Forms.ListBox();
            this.m_wndPageNMEA = new System.Windows.Forms.TabPage();
            this.m_wndNMEAList = new System.Windows.Forms.ListBox();
            this.m_wndPageAIS = new System.Windows.Forms.TabPage();
            this.m_wndAISList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.m_wndPageDatabaseLog = new System.Windows.Forms.TabPage();
            this.m_wndLog = new System.Windows.Forms.TextBox();
            this.m_wndPagesControl.SuspendLayout();
            this.m_wndPageRaw.SuspendLayout();
            this.m_wndPageNMEA.SuspendLayout();
            this.m_wndPageAIS.SuspendLayout();
            this.m_wndPageDatabaseLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_wndPagesControl
            // 
            this.m_wndPagesControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.m_wndPagesControl.Controls.Add(this.m_wndPageRaw);
            this.m_wndPagesControl.Controls.Add(this.m_wndPageNMEA);
            this.m_wndPagesControl.Controls.Add(this.m_wndPageAIS);
            this.m_wndPagesControl.Controls.Add(this.m_wndPageDatabaseLog);
            this.m_wndPagesControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_wndPagesControl.Location = new System.Drawing.Point(0, 0);
            this.m_wndPagesControl.Multiline = true;
            this.m_wndPagesControl.Name = "m_wndPagesControl";
            this.m_wndPagesControl.SelectedIndex = 0;
            this.m_wndPagesControl.Size = new System.Drawing.Size(549, 312);
            this.m_wndPagesControl.TabIndex = 1;
            // 
            // m_wndPageRaw
            // 
            this.m_wndPageRaw.Controls.Add(this.m_wndRawDataList);
            this.m_wndPageRaw.Location = new System.Drawing.Point(4, 25);
            this.m_wndPageRaw.Name = "m_wndPageRaw";
            this.m_wndPageRaw.Padding = new System.Windows.Forms.Padding(3);
            this.m_wndPageRaw.Size = new System.Drawing.Size(541, 283);
            this.m_wndPageRaw.TabIndex = 0;
            this.m_wndPageRaw.Text = "原始数据";
            this.m_wndPageRaw.UseVisualStyleBackColor = true;
            // 
            // m_wndRawDataList
            // 
            this.m_wndRawDataList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_wndRawDataList.FormattingEnabled = true;
            this.m_wndRawDataList.IntegralHeight = false;
            this.m_wndRawDataList.ItemHeight = 12;
            this.m_wndRawDataList.Location = new System.Drawing.Point(3, 3);
            this.m_wndRawDataList.Name = "m_wndRawDataList";
            this.m_wndRawDataList.Size = new System.Drawing.Size(535, 277);
            this.m_wndRawDataList.TabIndex = 0;
            // 
            // m_wndPageNMEA
            // 
            this.m_wndPageNMEA.Controls.Add(this.m_wndNMEAList);
            this.m_wndPageNMEA.Location = new System.Drawing.Point(4, 25);
            this.m_wndPageNMEA.Name = "m_wndPageNMEA";
            this.m_wndPageNMEA.Padding = new System.Windows.Forms.Padding(3);
            this.m_wndPageNMEA.Size = new System.Drawing.Size(601, 335);
            this.m_wndPageNMEA.TabIndex = 1;
            this.m_wndPageNMEA.Text = "NMEA数据";
            this.m_wndPageNMEA.UseVisualStyleBackColor = true;
            // 
            // m_wndNMEAList
            // 
            this.m_wndNMEAList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_wndNMEAList.FormattingEnabled = true;
            this.m_wndNMEAList.IntegralHeight = false;
            this.m_wndNMEAList.ItemHeight = 12;
            this.m_wndNMEAList.Location = new System.Drawing.Point(3, 3);
            this.m_wndNMEAList.Name = "m_wndNMEAList";
            this.m_wndNMEAList.Size = new System.Drawing.Size(595, 329);
            this.m_wndNMEAList.TabIndex = 1;
            // 
            // m_wndPageAIS
            // 
            this.m_wndPageAIS.Controls.Add(this.m_wndAISList);
            this.m_wndPageAIS.Location = new System.Drawing.Point(4, 25);
            this.m_wndPageAIS.Name = "m_wndPageAIS";
            this.m_wndPageAIS.Size = new System.Drawing.Size(601, 335);
            this.m_wndPageAIS.TabIndex = 2;
            this.m_wndPageAIS.Text = "AIS目标";
            this.m_wndPageAIS.UseVisualStyleBackColor = true;
            // 
            // m_wndAISList
            // 
            this.m_wndAISList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.m_wndAISList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_wndAISList.FullRowSelect = true;
            this.m_wndAISList.GridLines = true;
            this.m_wndAISList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.m_wndAISList.Location = new System.Drawing.Point(0, 0);
            this.m_wndAISList.MultiSelect = false;
            this.m_wndAISList.Name = "m_wndAISList";
            this.m_wndAISList.Size = new System.Drawing.Size(601, 335);
            this.m_wndAISList.TabIndex = 0;
            this.m_wndAISList.UseCompatibleStateImageBehavior = false;
            this.m_wndAISList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "MMSI";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "纬度";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "经度";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 90;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "航向";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "航速";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "艏向";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "船名";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 150;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "呼号";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader8.Width = 80;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "目的地";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader9.Width = 160;
            // 
            // m_wndPageDatabaseLog
            // 
            this.m_wndPageDatabaseLog.Controls.Add(this.m_wndLog);
            this.m_wndPageDatabaseLog.Location = new System.Drawing.Point(4, 25);
            this.m_wndPageDatabaseLog.Name = "m_wndPageDatabaseLog";
            this.m_wndPageDatabaseLog.Size = new System.Drawing.Size(601, 335);
            this.m_wndPageDatabaseLog.TabIndex = 3;
            this.m_wndPageDatabaseLog.Text = "数据库日志";
            this.m_wndPageDatabaseLog.UseVisualStyleBackColor = true;
            // 
            // m_wndLog
            // 
            this.m_wndLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_wndLog.Location = new System.Drawing.Point(0, 0);
            this.m_wndLog.Multiline = true;
            this.m_wndLog.Name = "m_wndLog";
            this.m_wndLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_wndLog.Size = new System.Drawing.Size(601, 335);
            this.m_wndLog.TabIndex = 0;
            // 
            // AisControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_wndPagesControl);
            this.Name = "AisControl";
            this.Size = new System.Drawing.Size(549, 312);
            this.m_wndPagesControl.ResumeLayout(false);
            this.m_wndPageRaw.ResumeLayout(false);
            this.m_wndPageNMEA.ResumeLayout(false);
            this.m_wndPageAIS.ResumeLayout(false);
            this.m_wndPageDatabaseLog.ResumeLayout(false);
            this.m_wndPageDatabaseLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl m_wndPagesControl;
        private System.Windows.Forms.TabPage m_wndPageRaw;
        private System.Windows.Forms.ListBox m_wndRawDataList;
        private System.Windows.Forms.TabPage m_wndPageNMEA;
        private System.Windows.Forms.ListBox m_wndNMEAList;
        private System.Windows.Forms.TabPage m_wndPageAIS;
        private System.Windows.Forms.ListView m_wndAISList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.TabPage m_wndPageDatabaseLog;
        private System.Windows.Forms.TextBox m_wndLog;
    }
}
