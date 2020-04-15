namespace Client
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lstChatters = new System.Windows.Forms.ListBox();
            this.txtChatBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStats = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstChatters
            // 
            this.lstChatters.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lstChatters.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstChatters.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstChatters.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lstChatters.FormattingEnabled = true;
            this.lstChatters.ItemHeight = 22;
            this.lstChatters.Location = new System.Drawing.Point(448, 48);
            this.lstChatters.Name = "lstChatters";
            this.lstChatters.Size = new System.Drawing.Size(103, 264);
            this.lstChatters.TabIndex = 8;
            // 
            // txtChatBox
            // 
            this.txtChatBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtChatBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtChatBox.Enabled = false;
            this.txtChatBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChatBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.txtChatBox.Location = new System.Drawing.Point(42, 48);
            this.txtChatBox.Multiline = true;
            this.txtChatBox.Name = "txtChatBox";
            this.txtChatBox.ReadOnly = true;
            this.txtChatBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChatBox.Size = new System.Drawing.Size(363, 266);
            this.txtChatBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 335);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "label1";
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.BackColor = System.Drawing.Color.Transparent;
            this.lblStats.Font = new System.Drawing.Font("Microsoft YaHei UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStats.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblStats.Location = new System.Drawing.Point(40, 11);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(373, 36);
            this.lblStats.TabIndex = 11;
            this.lblStats.Text = "MESSAGES FROM OTHERS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(443, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 26);
            this.label2.TabIndex = 12;
            this.label2.Text = "CLIENT LIST";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(583, 365);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblStats);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstChatters);
            this.Controls.Add(this.txtChatBox);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstChatters;
        private System.Windows.Forms.TextBox txtChatBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.Label label2;
    }
}