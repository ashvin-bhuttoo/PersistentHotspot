namespace PersistentHotspot
{
    partial class frmAutoRestartHS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAutoRestartHS));
            this.nudMins = new System.Windows.Forms.NumericUpDown();
            this.lblMins = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudMins)).BeginInit();
            this.SuspendLayout();
            // 
            // nudMins
            // 
            this.nudMins.Location = new System.Drawing.Point(12, 14);
            this.nudMins.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.nudMins.Name = "nudMins";
            this.nudMins.Size = new System.Drawing.Size(93, 20);
            this.nudMins.TabIndex = 0;
            // 
            // lblMins
            // 
            this.lblMins.AutoSize = true;
            this.lblMins.Location = new System.Drawing.Point(111, 16);
            this.lblMins.Name = "lblMins";
            this.lblMins.Size = new System.Drawing.Size(44, 13);
            this.lblMins.TabIndex = 1;
            this.lblMins.Text = "Minutes";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(180, 11);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // frmAutoRestartHS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 43);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblMins);
            this.Controls.Add(this.nudMins);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(288, 82);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(288, 82);
            this.Name = "frmAutoRestartHS";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set Hotspot Auto Restart Interval";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudMins)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudMins;
        private System.Windows.Forms.Label lblMins;
        private System.Windows.Forms.Button btnOk;
    }
}