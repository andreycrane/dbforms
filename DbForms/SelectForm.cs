/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 29.08.2014
 * Time: 19:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DbForms
{	
	/// <summary>
	/// Базовый класс форм, предназначенных для выбора некоторого значения из табличных данных
	/// </summary>
	public abstract class SelectForm : Form
	{
		private class SelectDataGridView : DataGridView {
			public event KeyEventHandler Key;
			
			protected override bool ProcessDataGridViewKey(KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape) {
					this.Key.Invoke(this, e);
					return false;	
				}
				
				return base.ProcessDataGridViewKey(e);
			}
		}		
		
		private ComponentResourceManager res = new ComponentResourceManager(typeof(SelectForm));
		
		private SelectDataGridView grid;
		private ToolStrip tool;
		private DataTable tbl;
		private DataView view;
		
		private ToolStripTextBox txtSearch;
		
		public SelectForm()
		{
			this.InitializeComponents();
			
			this.Load += this.formLoad;
		}
		
		/// <summary>
		/// Определяет заголов формы
		/// </summary>
		protected string Title {
			get { return this.Text; }
			set { this.Text = value; }
		}
		
		/// <summary>
		/// Источник данных для отображения на форме
		/// </summary>
		protected DataTable DataSource {
			set {
				this.tbl = value;
				this.view = value.AsDataView();
				this.grid.DataSource = this.view;
			}
		}
		
		private void InitializeComponents()
		{
			this.SuspendLayout();
			
			this.txtSearch = new ToolStripTextBox()
			{
				Alignment = ToolStripItemAlignment.Right,
				AutoSize = false,
				Width = 200,
				Padding = new Padding(0, 0, 10, 0)
			};
			
			this.txtSearch.KeyUp += this.txtSearch_KeyUp; 
						
			ToolStripButton btnCancel = new ToolStripButton("Закрыть")
			{
				Image = ((Icon)this.res.GetObject("exit")).ToBitmap(),
				ImageAlign = ContentAlignment.MiddleLeft
			};
			
			btnCancel.Click += delegate {
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			};
			
			ToolStripButton btnSearch = new ToolStripButton("Поиск")
			{
				Alignment = ToolStripItemAlignment.Right,
				Padding = new Padding(0, 0, 5, 0)
			};
			
			btnSearch.Click += (sender, e)=> this.view.FilterText(this.txtSearch.Text);
			
			this.grid = new SelectDataGridView()
			{
				Dock = DockStyle.Fill,
				ReadOnly = true,
				RowHeadersVisible = false,
				AutoGenerateColumns = false,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect
			};
			
			this.grid.Key += grid_Key;
			
			this.tool = new ToolStrip();
			
			this.tool.Items.Add(btnCancel);
			this.tool.Items.Add(this.txtSearch);
			this.tool.Items.Add(btnSearch);
			
			this.Controls.Add(this.grid);
			this.Controls.Add(this.tool);
			
			this.Icon = (Icon)this.res.GetObject("book-open");
			this.StartPosition = FormStartPosition.Manual;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			
			this.ResumeLayout();
			this.PerformLayout();
		}
		
		/// <summary>
		/// Добавляет столбец отображаемый на форме
		/// </summary>
		/// <param name="columnName">Наименование столбца в источнике данных</param>
		/// <param name="columnHeader">Заголовок столбца оторбражаемый наформе</param>
		/// <param name="width">Ширина столбца отображаемого на форме. По умолчанию: 100px</param>
		protected void AddColumn(string columnName, string columnHeader, int width = 100)
		{
			this.grid.Columns.Add(columnName, columnHeader);
			this.grid.Columns[columnName].DataPropertyName = columnName;
			this.grid.Columns[columnName].Width = width;
		}
		
		private void grid_Key(object sender, KeyEventArgs e) 
		{
			if(e.KeyCode == Keys.Enter) {
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			
			if(e.KeyCode == Keys.Escape) {
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			}
		}
		
		private void txtSearch_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) {
				e.SuppressKeyPress = true;
				
				this.view.FilterText(this.txtSearch.Text);
				this.grid.Focus();
			}
		}
		
		public new DataRow ShowDialog()
		{
			int ColumnsWidth = 0;
			foreach (DataGridViewColumn column in this.grid.Columns) {
				ColumnsWidth += column.Width;
			}
			
			this.ClientSize = new Size(
				(ColumnsWidth < 200) ? 200 : ColumnsWidth + 50,
			    this.ClientSize.Height);
			
			this.Height = Screen.PrimaryScreen.WorkingArea.Height;
			this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, 0);
			
			if(base.ShowDialog() == DialogResult.OK)
				return this.tbl.Rows[this.grid.CurrentRow.Index];
			
			return null;
		}
		
		protected abstract void formLoad(object sender, EventArgs e);
	}
}
