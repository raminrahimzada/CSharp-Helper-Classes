using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

	/// <summary>
	/// Summary description for FrmDriver.
	/// </summary>
	public class FrmDriver : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txbInput;
		private System.Windows.Forms.TextBox txbOutput;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;

		private MD5.MD5 md =new MD5.MD5();
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FrmDriver()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			txbInput.Text=md.Value;
			txbOutput.Text=md.FingerPrint;
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txbInput = new System.Windows.Forms.TextBox();
			this.txbOutput = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txbInput
			// 
			this.txbInput.Location = new System.Drawing.Point(72, 48);
			this.txbInput.Name = "txbInput";
			this.txbInput.Size = new System.Drawing.Size(240, 20);
			this.txbInput.TabIndex = 0;
			this.txbInput.Text = "";
			this.txbInput.TextChanged += new System.EventHandler(this.txbInput_TextChanged);
			// 
			// txbOutput
			// 
			this.txbOutput.Location = new System.Drawing.Point(72, 96);
			this.txbOutput.Name = "txbOutput";
			this.txbOutput.Size = new System.Drawing.Size(240, 20);
			this.txbOutput.TabIndex = 1;
			this.txbOutput.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "Input String";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 96);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 32);
			this.label2.TabIndex = 3;
			this.label2.Text = "MD5 Signature";
			// 
			// FrmDriver
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(354, 199);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label2,
																		  this.label1,
																		  this.txbOutput,
																		  this.txbInput});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmDriver";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Test Drive";
			this.TopMost = true;
			this.Closed += new System.EventHandler(this.FrmDriver_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		private void txbInput_TextChanged(object sender, System.EventArgs e)
		{
			md.Value=txbInput.Text;
			txbOutput.Text=md.FingerPrint;
		}

		private void FrmDriver_Closed(object sender, System.EventArgs e)
		{
			MessageBox.Show("Thank You For Using this program \n Send Ur Comments on s_faraz_mahmood@hotmail.com");
		}
		
	}

