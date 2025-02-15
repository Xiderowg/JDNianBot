﻿namespace JDNianBot
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.Txt_Output = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Btn_Login = new System.Windows.Forms.Button();
            this.Lbl_CurrentAccount = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Txt_Lat = new System.Windows.Forms.TextBox();
            this.Txt_Lon = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Lbl_Coin = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Btn_Refresh = new System.Windows.Forms.Button();
            this.Lbl_CurrentRedPack = new System.Windows.Forms.Label();
            this.Lbl_NextScore = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Lbl_Score = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Lbl_Level = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Btn_StartTask = new System.Windows.Forms.Button();
            this.Btn_StopTask = new System.Windows.Forms.Button();
            this.TM_Collect = new System.Windows.Forms.Timer(this.components);
            this.TM_DoTasks = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Chk_AutoRedPack = new System.Windows.Forms.CheckBox();
            this.Chk_AutoDoTasks = new System.Windows.Forms.CheckBox();
            this.Chk_AutoCollect = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Cbo_HelpType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Btn_HelpAuthor = new System.Windows.Forms.Button();
            this.Btn_CopyMine = new System.Windows.Forms.Button();
            this.Btn_DoHelp = new System.Windows.Forms.Button();
            this.Txt_InviteCode = new System.Windows.Forms.TextBox();
            this.TM_RedPacket = new System.Windows.Forms.Timer(this.components);
            this.TM_PreventSleep = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.Chk_AutoAnswer = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // Txt_Output
            // 
            this.Txt_Output.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Txt_Output.Location = new System.Drawing.Point(239, 12);
            this.Txt_Output.Multiline = true;
            this.Txt_Output.Name = "Txt_Output";
            this.Txt_Output.ReadOnly = true;
            this.Txt_Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_Output.Size = new System.Drawing.Size(463, 260);
            this.Txt_Output.TabIndex = 1;
            this.Txt_Output.Text = "本程序完全免费，仅供测试与交流使用，请勿用于商业用途及非法用途！By:EdLinus";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Btn_Login);
            this.groupBox1.Controls.Add(this.Lbl_CurrentAccount);
            this.groupBox1.Location = new System.Drawing.Point(7, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 57);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "账号";
            // 
            // Btn_Login
            // 
            this.Btn_Login.Location = new System.Drawing.Point(140, 19);
            this.Btn_Login.Name = "Btn_Login";
            this.Btn_Login.Size = new System.Drawing.Size(80, 28);
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
            this.groupBox2.Controls.Add(this.Txt_Lat);
            this.groupBox2.Controls.Add(this.Txt_Lon);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.Lbl_Coin);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.Btn_Refresh);
            this.groupBox2.Controls.Add(this.Lbl_CurrentRedPack);
            this.groupBox2.Controls.Add(this.Lbl_NextScore);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.Lbl_Score);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.Lbl_Level);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(5, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(227, 169);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "活动信息";
            // 
            // Txt_Lat
            // 
            this.Txt_Lat.Font = new System.Drawing.Font("宋体", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Txt_Lat.Location = new System.Drawing.Point(114, 135);
            this.Txt_Lat.Name = "Txt_Lat";
            this.Txt_Lat.Size = new System.Drawing.Size(37, 20);
            this.Txt_Lat.TabIndex = 15;
            this.Txt_Lat.Text = "39.938";
            // 
            // Txt_Lon
            // 
            this.Txt_Lon.Font = new System.Drawing.Font("宋体", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Txt_Lon.Location = new System.Drawing.Point(40, 136);
            this.Txt_Lon.Name = "Txt_Lon";
            this.Txt_Lon.Size = new System.Drawing.Size(37, 20);
            this.Txt_Lon.TabIndex = 14;
            this.Txt_Lon.Text = "116.347";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(83, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 15);
            this.label8.TabIndex = 13;
            this.label8.Text = "纬：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 15);
            this.label7.TabIndex = 12;
            this.label7.Text = "经：";
            // 
            // Lbl_Coin
            // 
            this.Lbl_Coin.AutoSize = true;
            this.Lbl_Coin.Location = new System.Drawing.Point(123, 114);
            this.Lbl_Coin.Name = "Lbl_Coin";
            this.Lbl_Coin.Size = new System.Drawing.Size(15, 15);
            this.Lbl_Coin.TabIndex = 11;
            this.Lbl_Coin.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "神仙书院金币：";
            // 
            // Btn_Refresh
            // 
            this.Btn_Refresh.Location = new System.Drawing.Point(175, 134);
            this.Btn_Refresh.Name = "Btn_Refresh";
            this.Btn_Refresh.Size = new System.Drawing.Size(47, 27);
            this.Btn_Refresh.TabIndex = 9;
            this.Btn_Refresh.Text = "刷新";
            this.Btn_Refresh.UseVisualStyleBackColor = true;
            this.Btn_Refresh.Click += new System.EventHandler(this.Btn_Refresh_Click);
            // 
            // Lbl_CurrentRedPack
            // 
            this.Lbl_CurrentRedPack.AutoSize = true;
            this.Lbl_CurrentRedPack.Location = new System.Drawing.Point(123, 89);
            this.Lbl_CurrentRedPack.Name = "Lbl_CurrentRedPack";
            this.Lbl_CurrentRedPack.Size = new System.Drawing.Size(15, 15);
            this.Lbl_CurrentRedPack.TabIndex = 7;
            this.Lbl_CurrentRedPack.Text = "0";
            // 
            // Lbl_NextScore
            // 
            this.Lbl_NextScore.AutoSize = true;
            this.Lbl_NextScore.Location = new System.Drawing.Point(123, 69);
            this.Lbl_NextScore.Name = "Lbl_NextScore";
            this.Lbl_NextScore.Size = new System.Drawing.Size(15, 15);
            this.Lbl_NextScore.TabIndex = 6;
            this.Lbl_NextScore.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "当前红包：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "下一等级爆竹：";
            // 
            // Lbl_Score
            // 
            this.Lbl_Score.AutoSize = true;
            this.Lbl_Score.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Lbl_Score.Location = new System.Drawing.Point(123, 47);
            this.Lbl_Score.Name = "Lbl_Score";
            this.Lbl_Score.Size = new System.Drawing.Size(15, 15);
            this.Lbl_Score.TabIndex = 3;
            this.Lbl_Score.Text = "0";
            this.Lbl_Score.Click += new System.EventHandler(this.Lbl_Score_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "当前爆竹：";
            // 
            // Lbl_Level
            // 
            this.Lbl_Level.AutoSize = true;
            this.Lbl_Level.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Lbl_Level.Location = new System.Drawing.Point(123, 25);
            this.Lbl_Level.Name = "Lbl_Level";
            this.Lbl_Level.Size = new System.Drawing.Size(61, 15);
            this.Lbl_Level.TabIndex = 1;
            this.Lbl_Level.Text = "0（0%）";
            this.Lbl_Level.Click += new System.EventHandler(this.Lbl_Level_Click);
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
            this.Btn_StartTask.Location = new System.Drawing.Point(7, 239);
            this.Btn_StartTask.Name = "Btn_StartTask";
            this.Btn_StartTask.Size = new System.Drawing.Size(154, 34);
            this.Btn_StartTask.TabIndex = 10;
            this.Btn_StartTask.Text = "一键做任务";
            this.Btn_StartTask.UseVisualStyleBackColor = true;
            this.Btn_StartTask.Click += new System.EventHandler(this.Btn_StartTask_Click);
            // 
            // Btn_StopTask
            // 
            this.Btn_StopTask.Enabled = false;
            this.Btn_StopTask.Location = new System.Drawing.Point(168, 239);
            this.Btn_StopTask.Name = "Btn_StopTask";
            this.Btn_StopTask.Size = new System.Drawing.Size(64, 34);
            this.Btn_StopTask.TabIndex = 11;
            this.Btn_StopTask.Text = "停止";
            this.Btn_StopTask.UseVisualStyleBackColor = true;
            this.Btn_StopTask.Click += new System.EventHandler(this.Btn_StopTask_Click);
            // 
            // TM_Collect
            // 
            this.TM_Collect.Interval = 1000;
            this.TM_Collect.Tick += new System.EventHandler(this.TM_Collect_Tick);
            // 
            // TM_DoTasks
            // 
            this.TM_DoTasks.Interval = 1000;
            this.TM_DoTasks.Tick += new System.EventHandler(this.TM_DoTasks_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Chk_AutoAnswer);
            this.groupBox3.Controls.Add(this.Chk_AutoRedPack);
            this.groupBox3.Controls.Add(this.Chk_AutoDoTasks);
            this.groupBox3.Controls.Add(this.Chk_AutoCollect);
            this.groupBox3.Location = new System.Drawing.Point(12, 276);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 83);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "定时任务";
            // 
            // Chk_AutoRedPack
            // 
            this.Chk_AutoRedPack.AutoSize = true;
            this.Chk_AutoRedPack.Location = new System.Drawing.Point(103, 23);
            this.Chk_AutoRedPack.Name = "Chk_AutoRedPack";
            this.Chk_AutoRedPack.Size = new System.Drawing.Size(74, 19);
            this.Chk_AutoRedPack.TabIndex = 2;
            this.Chk_AutoRedPack.Text = "抢红包";
            this.Chk_AutoRedPack.UseVisualStyleBackColor = true;
            this.Chk_AutoRedPack.Click += new System.EventHandler(this.Chk_AutoRedPack_Click);
            // 
            // Chk_AutoDoTasks
            // 
            this.Chk_AutoDoTasks.AutoSize = true;
            this.Chk_AutoDoTasks.Location = new System.Drawing.Point(10, 53);
            this.Chk_AutoDoTasks.Name = "Chk_AutoDoTasks";
            this.Chk_AutoDoTasks.Size = new System.Drawing.Size(74, 19);
            this.Chk_AutoDoTasks.TabIndex = 1;
            this.Chk_AutoDoTasks.Text = "做任务";
            this.Chk_AutoDoTasks.UseVisualStyleBackColor = true;
            this.Chk_AutoDoTasks.Click += new System.EventHandler(this.Chk_AutoDoTasks_Click);
            // 
            // Chk_AutoCollect
            // 
            this.Chk_AutoCollect.AutoSize = true;
            this.Chk_AutoCollect.Location = new System.Drawing.Point(10, 25);
            this.Chk_AutoCollect.Name = "Chk_AutoCollect";
            this.Chk_AutoCollect.Size = new System.Drawing.Size(74, 19);
            this.Chk_AutoCollect.TabIndex = 0;
            this.Chk_AutoCollect.Text = "收爆竹";
            this.Chk_AutoCollect.UseVisualStyleBackColor = true;
            this.Chk_AutoCollect.Click += new System.EventHandler(this.Chk_AutoCollect_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Cbo_HelpType);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.Btn_HelpAuthor);
            this.groupBox4.Controls.Add(this.Btn_CopyMine);
            this.groupBox4.Controls.Add(this.Btn_DoHelp);
            this.groupBox4.Controls.Add(this.Txt_InviteCode);
            this.groupBox4.Location = new System.Drawing.Point(218, 276);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(484, 83);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "好友互助";
            // 
            // Cbo_HelpType
            // 
            this.Cbo_HelpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cbo_HelpType.ItemHeight = 15;
            this.Cbo_HelpType.Items.AddRange(new object[] {
            "PK",
            "年兽",
            "书院",
            "特殊活动"});
            this.Cbo_HelpType.Location = new System.Drawing.Point(338, 19);
            this.Cbo_HelpType.Name = "Cbo_HelpType";
            this.Cbo_HelpType.Size = new System.Drawing.Size(141, 23);
            this.Cbo_HelpType.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "助力代码：";
            // 
            // Btn_HelpAuthor
            // 
            this.Btn_HelpAuthor.Location = new System.Drawing.Point(171, 50);
            this.Btn_HelpAuthor.Name = "Btn_HelpAuthor";
            this.Btn_HelpAuthor.Size = new System.Drawing.Size(160, 27);
            this.Btn_HelpAuthor.TabIndex = 3;
            this.Btn_HelpAuthor.Text = "帮作者助力";
            this.Btn_HelpAuthor.UseVisualStyleBackColor = true;
            this.Btn_HelpAuthor.Click += new System.EventHandler(this.Btn_HelpAuthor_Click);
            // 
            // Btn_CopyMine
            // 
            this.Btn_CopyMine.Location = new System.Drawing.Point(335, 50);
            this.Btn_CopyMine.Name = "Btn_CopyMine";
            this.Btn_CopyMine.Size = new System.Drawing.Size(143, 27);
            this.Btn_CopyMine.TabIndex = 2;
            this.Btn_CopyMine.Text = "复制我的助力代码";
            this.Btn_CopyMine.UseVisualStyleBackColor = true;
            this.Btn_CopyMine.Click += new System.EventHandler(this.Btn_CopyMine_Click);
            // 
            // Btn_DoHelp
            // 
            this.Btn_DoHelp.Location = new System.Drawing.Point(7, 50);
            this.Btn_DoHelp.Name = "Btn_DoHelp";
            this.Btn_DoHelp.Size = new System.Drawing.Size(160, 27);
            this.Btn_DoHelp.TabIndex = 1;
            this.Btn_DoHelp.Text = "帮他助力";
            this.Btn_DoHelp.UseVisualStyleBackColor = true;
            this.Btn_DoHelp.Click += new System.EventHandler(this.Btn_DoHelp_Click);
            // 
            // Txt_InviteCode
            // 
            this.Txt_InviteCode.Location = new System.Drawing.Point(94, 19);
            this.Txt_InviteCode.Name = "Txt_InviteCode";
            this.Txt_InviteCode.Size = new System.Drawing.Size(237, 25);
            this.Txt_InviteCode.TabIndex = 0;
            // 
            // TM_RedPacket
            // 
            this.TM_RedPacket.Interval = 1000;
            this.TM_RedPacket.Tick += new System.EventHandler(this.TM_RedPacket_Tick);
            // 
            // TM_PreventSleep
            // 
            this.TM_PreventSleep.Enabled = true;
            this.TM_PreventSleep.Interval = 15000;
            this.TM_PreventSleep.Tick += new System.EventHandler(this.TM_PreventSleep_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "JDNianBot";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // Chk_AutoAnswer
            // 
            this.Chk_AutoAnswer.AutoSize = true;
            this.Chk_AutoAnswer.Location = new System.Drawing.Point(103, 50);
            this.Chk_AutoAnswer.Name = "Chk_AutoAnswer";
            this.Chk_AutoAnswer.Size = new System.Drawing.Size(74, 19);
            this.Chk_AutoAnswer.TabIndex = 3;
            this.Chk_AutoAnswer.Text = "做题目";
            this.Chk_AutoAnswer.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 365);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Btn_StopTask);
            this.Controls.Add(this.Btn_StartTask);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Txt_Output);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.Text = "JDNianBot v1.4";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.Label Lbl_Score;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label Lbl_Level;
        private System.Windows.Forms.Button Btn_StartTask;
        private System.Windows.Forms.Button Btn_StopTask;
        private System.Windows.Forms.Timer TM_Collect;
        private System.Windows.Forms.Timer TM_DoTasks;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox Chk_AutoDoTasks;
        private System.Windows.Forms.CheckBox Chk_AutoCollect;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button Btn_DoHelp;
        private System.Windows.Forms.TextBox Txt_InviteCode;
        private System.Windows.Forms.Button Btn_HelpAuthor;
        private System.Windows.Forms.Button Btn_CopyMine;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label Lbl_Coin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Txt_Lat;
        private System.Windows.Forms.TextBox Txt_Lon;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox Chk_AutoRedPack;
        private System.Windows.Forms.ComboBox Cbo_HelpType;
        private System.Windows.Forms.Timer TM_RedPacket;
        private System.Windows.Forms.Timer TM_PreventSleep;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.CheckBox Chk_AutoAnswer;
    }
}

