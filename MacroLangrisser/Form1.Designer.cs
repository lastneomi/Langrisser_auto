namespace MacroLangrisser
{
    partial class Form1
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnCapImg = new System.Windows.Forms.Button();
            this.btnCase = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSpecial = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(107, 6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(195, 229);
            this.textBox1.TabIndex = 2;
            // 
            // btnCapImg
            // 
            this.btnCapImg.Location = new System.Drawing.Point(5, 6);
            this.btnCapImg.Name = "btnCapImg";
            this.btnCapImg.Size = new System.Drawing.Size(96, 45);
            this.btnCapImg.TabIndex = 4;
            this.btnCapImg.Text = "이미지 저장";
            this.btnCapImg.UseVisualStyleBackColor = true;
            this.btnCapImg.Click += new System.EventHandler(this.btnCapImg_Click);
            // 
            // btnCase
            // 
            this.btnCase.Location = new System.Drawing.Point(6, 88);
            this.btnCase.Name = "btnCase";
            this.btnCase.Size = new System.Drawing.Size(96, 45);
            this.btnCase.TabIndex = 9;
            this.btnCase.Text = "사건 자동";
            this.btnCase.UseVisualStyleBackColor = true;
            this.btnCase.Click += new System.EventHandler(this.btnCase_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(6, 190);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(96, 45);
            this.btnStop.TabIndex = 10;
            this.btnStop.Text = "정지";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnSpecial
            // 
            this.btnSpecial.Location = new System.Drawing.Point(6, 139);
            this.btnSpecial.Name = "btnSpecial";
            this.btnSpecial.Size = new System.Drawing.Size(96, 45);
            this.btnSpecial.TabIndex = 11;
            this.btnSpecial.Text = "비경 자동";
            this.btnSpecial.UseVisualStyleBackColor = true;
            this.btnSpecial.Click += new System.EventHandler(this.btnSpecial_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 239);
            this.Controls.Add(this.btnSpecial);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnCase);
            this.Controls.Add(this.btnCapImg);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnCapImg;
        private System.Windows.Forms.Button btnCase;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnSpecial;
    }
}

