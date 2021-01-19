namespace JDNianBot
{
    partial class FrmMain
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Txt_Output = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Btn_Login = new System.Windows.Forms.Button();
            this.Lbl_CurrentAccount = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Btn_Refresh = new System.Windows.Forms.Button();
            this.Lbl_CurrentRedPack = new System.Windows.Forms.Label();
            this.Lbl_NextScore = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Lbl_Score = new System.Windows.Forms.Label();
            this.Lbl_Level = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Btn_StartTask = new System.Windows.Forms.Button();
            this.Btn_StopTask = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Txt_Output
            // 
            this.Txt_Output.Location = new System.Drawing.Point(256, 12);
            this.Txt_Output.Multiline = true;
            this.Txt_Output.Name = "Txt_Output";
            this.Txt_Output.Size = new System.Drawing.Size(463, 254);
            this.Txt_Output.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Btn_Login);
            this.groupBox1.Controls.Add(this.Lbl_CurrentAccount);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 57);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "账号";
            // 
            // Btn_Login
            // 
            this.Btn_Login.Location = new System.Drawing.Point(140, 21);
            this.Btn_Login.Name = "Btn_Login";
            this.Btn_Login.Size = new System.Drawing.Size(80, 25);
            this.Btn_Login.TabIndex = 2;
            this.Btn_Login.Text = "登陆";
            this.Btn_Login.UseVisualStyleBackColor = true;
            this.Btn_Login.Click += new System.EventHandler(this.Btn_Login_Click);
            // 
            // Lbl_CurrentAccount
            // 
            this.Lbl_CurrentAccount.AutoSize = true;
            this.Lbl_CurrentAccount.Location = new System.Drawing.Point(6, 26);
            this.Lbl_CurrentAccount.Name = "Lbl_CurrentAccount";
            this.Lbl_CurrentAccount.Size = new System.Drawing.Size(76, 15);
            this.Lbl_CurrentAccount.TabIndex = 0;
            this.Lbl_CurrentAccount.Text = "未登录...";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Btn_Refresh);
            this.groupBox2.Controls.Add(this.Lbl_CurrentRedPack);
            this.groupBox2.Controls.Add(this.Lbl_NextScore);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.Lbl_Score);
            this.groupBox2.Controls.Add(this.Lbl_Level);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 77);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(227, 158);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "活动信息";
            // 
            // Btn_Refresh
            // 
            this.Btn_Refresh.Location = new System.Drawing.Point(139, 125);
            this.Btn_Refresh.Name = "Btn_Refresh";
            this.Btn_Refresh.Size = new System.Drawing.Size(82, 22);
            this.Btn_Refresh.TabIndex = 9;
            this.Btn_Refresh.Text = "刷新信息";
            this.Btn_Refresh.UseVisualStyleBackColor = true;
            this.Btn_Refresh.Click += new System.EventHandler(this.Btn_Refresh_Click);
            // 
            // Lbl_CurrentRedPack
            // 
            this.Lbl_CurrentRedPack.AutoSize = true;
            this.Lbl_CurrentRedPack.Location = new System.Drawing.Point(138, 102);
            this.Lbl_CurrentRedPack.Name = "Lbl_CurrentRedPack";
            this.Lbl_CurrentRedPack.Size = new System.Drawing.Size(15, 15);
            this.Lbl_CurrentRedPack.TabIndex = 7;
            this.Lbl_CurrentRedPack.Text = "0";
            // 
            // Lbl_NextScore
            // 
            this.Lbl_NextScore.AutoSize = true;
            this.Lbl_NextScore.Location = new System.Drawing.Point(138, 76);
            this.Lbl_NextScore.Name = "Lbl_NextScore";
            this.Lbl_NextScore.Size = new System.Drawing.Size(15, 15);
            this.Lbl_NextScore.TabIndex = 6;
            this.Lbl_NextScore.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "当前红包：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "下一等级爆竹：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(138, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "0";
            // 
            // Lbl_Score
            // 
            this.Lbl_Score.AutoSize = true;
            this.Lbl_Score.Location = new System.Drawing.Point(7, 51);
            this.Lbl_Score.Name = "Lbl_Score";
            this.Lbl_Score.Size = new System.Drawing.Size(82, 15);
            this.Lbl_Score.TabIndex = 2;
            this.Lbl_Score.Text = "当前爆竹：";
            // 
            // Lbl_Level
            // 
            this.Lbl_Level.AutoSize = true;
            this.Lbl_Level.Location = new System.Drawing.Point(138, 25);
            this.Lbl_Level.Name = "Lbl_Level";
            this.Lbl_Level.Size = new System.Drawing.Size(61, 15);
            this.Lbl_Level.TabIndex = 1;
            this.Lbl_Level.Text = "0（0%）";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "当前等级：";
            // 
            // Btn_StartTask
            // 
            this.Btn_StartTask.Location = new System.Drawing.Point(13, 241);
            this.Btn_StartTask.Name = "Btn_StartTask";
            this.Btn_StartTask.Size = new System.Drawing.Size(154, 25);
            this.Btn_StartTask.TabIndex = 10;
            this.Btn_StartTask.Text = "一键做任务";
            this.Btn_StartTask.UseVisualStyleBackColor = true;
            this.Btn_StartTask.Click += new System.EventHandler(this.Btn_StartTask_Click);
            // 
            // Btn_StopTask
            // 
            this.Btn_StopTask.Enabled = false;
            this.Btn_StopTask.Location = new System.Drawing.Point(173, 241);
            this.Btn_StopTask.Name = "Btn_StopTask";
            this.Btn_StopTask.Size = new System.Drawing.Size(64, 25);
            this.Btn_StopTask.TabIndex = 11;
            this.Btn_StopTask.Text = "停止";
            this.Btn_StopTask.UseVisualStyleBackColor = true;
            this.Btn_StopTask.Click += new System.EventHandler(this.Btn_StopTask_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 282);
            this.Controls.Add(this.Btn_StopTask);
            this.Controls.Add(this.Btn_StartTask);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Txt_Output);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.Text = "JDNianBot【v1.0】";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Txt_Output;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label Lbl_CurrentAccount;
        private System.Windows.Forms.Button Btn_Login;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Btn_Refresh;
        private System.Windows.Forms.Label Lbl_CurrentRedPack;
        private System.Windows.Forms.Label Lbl_NextScore;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label Lbl_Score;
        private System.Windows.Forms.Label Lbl_Level;
        private System.Windows.Forms.Button Btn_StartTask;
        private System.Windows.Forms.Button Btn_StopTask;
    }
}

