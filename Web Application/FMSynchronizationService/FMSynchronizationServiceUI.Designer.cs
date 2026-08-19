namespace FMSynchronizationService
{
    partial class FMSynchronizationServiceUI
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
            this.components = new System.ComponentModel.Container();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblRunning = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.checkServiceTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(125, 41);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 0;
            this.btnStop.Text = "S&top";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblRunning
            // 
            this.lblRunning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRunning.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblRunning.Location = new System.Drawing.Point(12, 9);
            this.lblRunning.Name = "lblRunning";
            this.lblRunning.Size = new System.Drawing.Size(273, 23);
            this.lblRunning.TabIndex = 1;
            this.lblRunning.Text = "FMSynchronizationService running...";
            this.lblRunning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(206, 41);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "&Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // checkServiceTimer
            // 
            this.checkServiceTimer.Enabled = true;
            this.checkServiceTimer.Interval = 6000;
            this.checkServiceTimer.Tick += new System.EventHandler(this.checkServiceTimer_Tick);
            // 
            // FMSynchronizationServiceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 76);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblRunning);
            this.Controls.Add(this.btnStop);
            this.Name = "FMSynchronizationServiceUI";
            this.Text = "FM Sync Service";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblRunning;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Timer checkServiceTimer;
    }
}