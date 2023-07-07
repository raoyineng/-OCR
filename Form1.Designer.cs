namespace 易用OCR
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.PaddleOCRSharp = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(26, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(1091, 404);
            this.textBox1.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // PaddleOCRSharp
            // 
            this.PaddleOCRSharp.Location = new System.Drawing.Point(26, 452);
            this.PaddleOCRSharp.Name = "PaddleOCRSharp";
            this.PaddleOCRSharp.Size = new System.Drawing.Size(265, 77);
            this.PaddleOCRSharp.TabIndex = 2;
            this.PaddleOCRSharp.Text = "打开图片识别(&O)";
            this.PaddleOCRSharp.UseVisualStyleBackColor = true;
            this.PaddleOCRSharp.Click += new System.EventHandler(this.PaddleOCRSharp_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(432, 452);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(265, 77);
            this.button1.TabIndex = 2;
            this.button1.Text = "设置屏幕坐标截图并识别(&S)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(852, 452);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(265, 77);
            this.button2.TabIndex = 2;
            this.button2.Text = "鼠标截图识别(&R)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 568);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PaddleOCRSharp);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "好用OCR";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button PaddleOCRSharp;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

