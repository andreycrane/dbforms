/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 24.08.2014
 * Time: 21:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace TestForms
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.cityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.countryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnExit = new System.Windows.Forms.ToolStripButton();
			this.menuStrip.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripMenuItem1});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(292, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.cityToolStripMenuItem,
									this.countryToolStripMenuItem,
									this.languageToolStripMenuItem});
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(39, 20);
			this.toolStripMenuItem1.Text = "Мир";
			// 
			// cityToolStripMenuItem
			// 
			this.cityToolStripMenuItem.Name = "cityToolStripMenuItem";
			this.cityToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.cityToolStripMenuItem.Text = "Города";
			this.cityToolStripMenuItem.Click += new System.EventHandler(this.CityToolStripMenuItemClick);
			// 
			// countryToolStripMenuItem
			// 
			this.countryToolStripMenuItem.Name = "countryToolStripMenuItem";
			this.countryToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.countryToolStripMenuItem.Text = "Страны";
			this.countryToolStripMenuItem.Click += new System.EventHandler(this.CountryToolStripMenuItemClick);
			// 
			// languageToolStripMenuItem
			// 
			this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
			this.languageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.languageToolStripMenuItem.Text = "Языки";
			this.languageToolStripMenuItem.Click += new System.EventHandler(this.LanguageToolStripMenuItemClick);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.btnExit});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(292, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// btnExit
			// 
			this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
			this.btnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(60, 22);
			this.btnExit.Text = "Выход";
			this.btnExit.Click += new System.EventHandler(this.BtnExitClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.Text = "БД \"Мир\"";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripButton btnExit;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem countryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.MenuStrip menuStrip;
	}
}
