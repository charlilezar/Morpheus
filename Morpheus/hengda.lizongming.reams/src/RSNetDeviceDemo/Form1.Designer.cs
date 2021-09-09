namespace RSDeviceDemo
{
    partial class Form1
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
            this.btnCallStore = new System.Windows.Forms.Button();
            this.btnTimming = new System.Windows.Forms.Button();
            this.txtDeviceID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnTrans = new System.Windows.Forms.Button();
            this.txtTransData = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtParamVal = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtParamId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtParamIds = new System.Windows.Forms.TextBox();
            this.btnWriteParameters = new System.Windows.Forms.Button();
            this.btnReadParameters = new System.Windows.Forms.Button();
            this.btnReadParametersList = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.rhtResult = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCallStore
            // 
            this.btnCallStore.Location = new System.Drawing.Point(644, 49);
            this.btnCallStore.Name = "btnCallStore";
            this.btnCallStore.Size = new System.Drawing.Size(75, 23);
            this.btnCallStore.TabIndex = 18;
            this.btnCallStore.Text = "召唤数据";
            this.btnCallStore.UseVisualStyleBackColor = true;
            this.btnCallStore.Click += new System.EventHandler(this.btnCallStore_Click);
            // 
            // btnTimming
            // 
            this.btnTimming.Location = new System.Drawing.Point(552, 49);
            this.btnTimming.Name = "btnTimming";
            this.btnTimming.Size = new System.Drawing.Size(75, 23);
            this.btnTimming.TabIndex = 17;
            this.btnTimming.Text = "校时";
            this.btnTimming.UseVisualStyleBackColor = true;
            this.btnTimming.Click += new System.EventHandler(this.btnTimming_Click);
            // 
            // txtDeviceID
            // 
            this.txtDeviceID.Location = new System.Drawing.Point(125, 50);
            this.txtDeviceID.Name = "txtDeviceID";
            this.txtDeviceID.Size = new System.Drawing.Size(100, 21);
            this.txtDeviceID.TabIndex = 15;
            this.txtDeviceID.Text = "10000000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "设备地址：";
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(644, 15);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(552, 15);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 13;
            this.btnStart.Text = "启动";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtPort
            // 
            this.txtPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPort.Location = new System.Drawing.Point(415, 16);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 21);
            this.txtPort.TabIndex = 10;
            this.txtPort.Text = "2404";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(362, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "端口：";
            // 
            // txtHost
            // 
            this.txtHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHost.Location = new System.Drawing.Point(125, 16);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(214, 21);
            this.txtHost.TabIndex = 8;
            this.txtHost.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "本地服务器地址：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.btnCallStore);
            this.panel1.Controls.Add(this.btnTimming);
            this.panel1.Controls.Add(this.txtDeviceID);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Controls.Add(this.txtPort);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtHost);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(741, 380);
            this.panel1.TabIndex = 10;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnTrans);
            this.groupBox2.Controls.Add(this.txtTransData);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(16, 270);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(699, 100);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据透传";
            // 
            // btnTrans
            // 
            this.btnTrans.Location = new System.Drawing.Point(546, 33);
            this.btnTrans.Name = "btnTrans";
            this.btnTrans.Size = new System.Drawing.Size(147, 61);
            this.btnTrans.TabIndex = 1;
            this.btnTrans.Text = "数据透传";
            this.btnTrans.UseVisualStyleBackColor = true;
            this.btnTrans.Click += new System.EventHandler(this.btnTrans_Click);
            // 
            // txtTransData
            // 
            this.txtTransData.Location = new System.Drawing.Point(6, 33);
            this.txtTransData.Multiline = true;
            this.txtTransData.Name = "txtTransData";
            this.txtTransData.Size = new System.Drawing.Size(534, 61);
            this.txtTransData.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(137, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "透传数据，16进制字符串";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtParamVal);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.txtParamId);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.txtParamIds);
            this.groupBox4.Controls.Add(this.btnWriteParameters);
            this.groupBox4.Controls.Add(this.btnReadParameters);
            this.groupBox4.Controls.Add(this.btnReadParametersList);
            this.groupBox4.Location = new System.Drawing.Point(16, 128);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(699, 135);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "设备参数";
            // 
            // txtParamVal
            // 
            this.txtParamVal.Location = new System.Drawing.Point(227, 93);
            this.txtParamVal.Name = "txtParamVal";
            this.txtParamVal.Size = new System.Drawing.Size(100, 21);
            this.txtParamVal.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(184, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "参数值";
            // 
            // txtParamId
            // 
            this.txtParamId.Location = new System.Drawing.Point(77, 93);
            this.txtParamId.Name = "txtParamId";
            this.txtParamId.Size = new System.Drawing.Size(60, 21);
            this.txtParamId.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "参数编号";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(299, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "参数编号，用于读取设备参数（多个编号用英文,分隔）";
            // 
            // txtParamIds
            // 
            this.txtParamIds.Location = new System.Drawing.Point(15, 51);
            this.txtParamIds.Name = "txtParamIds";
            this.txtParamIds.Size = new System.Drawing.Size(525, 21);
            this.txtParamIds.TabIndex = 4;
            this.txtParamIds.Text = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20";
            // 
            // btnWriteParameters
            // 
            this.btnWriteParameters.Location = new System.Drawing.Point(546, 93);
            this.btnWriteParameters.Name = "btnWriteParameters";
            this.btnWriteParameters.Size = new System.Drawing.Size(127, 23);
            this.btnWriteParameters.TabIndex = 1;
            this.btnWriteParameters.Text = "下载设备参数";
            this.btnWriteParameters.UseVisualStyleBackColor = true;
            this.btnWriteParameters.Click += new System.EventHandler(this.btnWriteParameters_Click);
            // 
            // btnReadParameters
            // 
            this.btnReadParameters.Location = new System.Drawing.Point(546, 50);
            this.btnReadParameters.Name = "btnReadParameters";
            this.btnReadParameters.Size = new System.Drawing.Size(127, 23);
            this.btnReadParameters.TabIndex = 2;
            this.btnReadParameters.Text = "读取设备参数";
            this.btnReadParameters.UseVisualStyleBackColor = true;
            this.btnReadParameters.Click += new System.EventHandler(this.btnReadParameters_Click);
            // 
            // btnReadParametersList
            // 
            this.btnReadParametersList.Location = new System.Drawing.Point(546, 20);
            this.btnReadParametersList.Name = "btnReadParametersList";
            this.btnReadParametersList.Size = new System.Drawing.Size(127, 23);
            this.btnReadParametersList.TabIndex = 3;
            this.btnReadParametersList.Text = "读取设备参数列表";
            this.btnReadParametersList.UseVisualStyleBackColor = true;
            this.btnReadParametersList.Click += new System.EventHandler(this.btnReadParametersList_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox8);
            this.groupBox3.Controls.Add(this.checkBox7);
            this.groupBox3.Controls.Add(this.checkBox6);
            this.groupBox3.Controls.Add(this.checkBox5);
            this.groupBox3.Controls.Add(this.checkBox4);
            this.groupBox3.Controls.Add(this.checkBox3);
            this.groupBox3.Controls.Add(this.checkBox2);
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Location = new System.Drawing.Point(16, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(699, 48);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "继电器操作";
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Location = new System.Drawing.Point(561, 20);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(66, 16);
            this.checkBox8.TabIndex = 17;
            this.checkBox8.Tag = "7";
            this.checkBox8.Text = "继电器7";
            this.checkBox8.UseVisualStyleBackColor = true;
            this.checkBox8.CheckedChanged += new System.EventHandler(this.chk_Click);
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(486, 20);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(66, 16);
            this.checkBox7.TabIndex = 18;
            this.checkBox7.Tag = "6";
            this.checkBox7.Text = "继电器6";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox7.CheckedChanged += new System.EventHandler(this.chk_Click);
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(411, 20);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(66, 16);
            this.checkBox6.TabIndex = 19;
            this.checkBox6.Tag = "5";
            this.checkBox6.Text = "继电器5";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.chk_Click);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(336, 20);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(66, 16);
            this.checkBox5.TabIndex = 20;
            this.checkBox5.Tag = "4";
            this.checkBox5.Text = "继电器4";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.chk_Click);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(261, 20);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(66, 16);
            this.checkBox4.TabIndex = 21;
            this.checkBox4.Tag = "3";
            this.checkBox4.Text = "继电器3";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.chk_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(186, 20);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(66, 16);
            this.checkBox3.TabIndex = 22;
            this.checkBox3.Tag = "2";
            this.checkBox3.Text = "继电器2";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.chk_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(111, 20);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(66, 16);
            this.checkBox2.TabIndex = 23;
            this.checkBox2.Tag = "1";
            this.checkBox2.Text = "继电器1";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.chk_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(36, 20);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(66, 16);
            this.checkBox1.TabIndex = 24;
            this.checkBox1.Tag = "0";
            this.checkBox1.Text = "继电器0";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.chk_Click);
            // 
            // rhtResult
            // 
            this.rhtResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rhtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rhtResult.Location = new System.Drawing.Point(3, 17);
            this.rhtResult.Name = "rhtResult";
            this.rhtResult.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rhtResult.Size = new System.Drawing.Size(735, 130);
            this.rhtResult.TabIndex = 0;
            this.rhtResult.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rhtResult);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 380);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(741, 150);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "接收数据：";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 530);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCallStore;
        private System.Windows.Forms.Button btnTimming;
        private System.Windows.Forms.TextBox txtDeviceID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rhtResult;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnWriteParameters;
        private System.Windows.Forms.Button btnReadParameters;
        private System.Windows.Forms.Button btnReadParametersList;
        private System.Windows.Forms.TextBox txtParamIds;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtParamVal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtParamId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnTrans;
        private System.Windows.Forms.TextBox txtTransData;
        private System.Windows.Forms.Label label7;
    }
}

