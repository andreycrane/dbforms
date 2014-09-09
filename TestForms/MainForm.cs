/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 24.08.2014
 * Time: 21:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TestForms
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private MySqlConnectionStringBuilder connStrBuilder;
		private MySqlConnection conn;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			connStrBuilder = new MySqlConnectionStringBuilder()
			{
				Port = 3307,
				UserID = "root",
				Password = "pronunciation5go*",
				CharacterSet = "utf8",
				Server = "127.0.0.1",
				Database = "world"
			};
			
			conn = new MySqlConnection(connStrBuilder.GetConnectionString(true));
			conn.Open();
		}
		
		void CityToolStripMenuItemClick(object sender, EventArgs e)
		{
			CityBrowse form = new CityBrowse(this.conn);
			form.ShowDialog();
		}
		
		void CountryToolStripMenuItemClick(object sender, EventArgs e)
		{
			CountryBrowse form = new CountryBrowse(this.conn);
			form.ShowDialog();
		}
		
		void LanguageToolStripMenuItemClick(object sender, EventArgs e)
		{
			LanguageBrowse form = new LanguageBrowse(this.conn);
			form.ShowDialog();
		}
		
		~MainForm()
		{
			conn.Close();
		}
		
		void BtnExitClick(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
