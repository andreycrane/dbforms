/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 24.08.2014
 * Time: 21:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace DbForms
{
	/// <remarks>
	/// 	<para>
	/// 		Класс BrowseForm - базовый класс форм, предназначенных для отображения табличных 
	/// 		данных и манипулирования этими данными (вставка, изменение, удаление).
	/// 	</para>
	/// 		Класс BrowseForm абстрактный, и не предназначенный для непосредственного
	/// 		использования.
	/// 	<para>
	/// 	<example>
	/// 		<code>
	/// 			class SimplTable : BrowseFor {
	/// 				private MySqlConnection conn;
	/// 				private DataTabe tbl;
	/// 
	/// 				public SimplTable(MySqlConnection conn) {
	/// 					this.conn = conn;
	/// 					this.tbl = new DataTable("simple"); 
	/// 
	/// 					this.Title = "Пример простой формы для отображения таблицы из двух столбцов";
	///						
    /// 					/// Добавляем столбцы отображаемых данных
	/// 					this.AddColumn("id", "ИД");
	/// 					this.AddColumn("Name", "Имя");
	/// 				}
	/// 
	/// 				protected sealed override void formLoad(object sender, EventArgs e)
	///					{
	/// 					// Код извлечения отображаемых данных
	///						string sql = @"SELECT id, Name FROM simple";
	///						
	///						MySqlCommand cmd = new MySqlCommand(sql, this.conn);
	///						MySqlDataReader reader = cmd.ExecuteReader();
	///						
	///						this.tbl.Load(reader);
	///						this.DataSource = this.tbl;
	///					}
	/// 
	/// 				protected sealed override void btnCreate_Click(object sender, EventArgs e) { }
	/// 				protected sealed override void btnEdit_Click(object sender, EventArgs e) { }
	/// 				protected sealed override void btnDelete_Click(object sender, EventArgs e) { }
	/// 			}
	/// 		</code>
	/// 	</example>
	/// </para>
	/// </remarks>
	public abstract class BrowseForm : Form
	{		
		protected class BrowseDataGridView : DataGridView {
			public event KeyEventHandler Key;
			
			protected override bool ProcessDataGridViewKey(KeyEventArgs e)
			{
				Keys[] keys = { Keys.Escape, Keys.Insert, Keys.F2, Keys.Delete };
				
				if (keys.Contains<Keys>(e.KeyCode)) {
					this.Key.Invoke(this, e);
					return false;	
				}
				
				return base.ProcessDataGridViewKey(e);
			}
		}
		
		protected BrowseDataGridView grid;
		private ToolStripButton btnCreate;
		private ToolStripButton btnEdit;
		private ToolStripButton btnDelete;
		private ToolStripTextBox txtSearch;
		private ToolStrip tools;
		private bool actionsSeparator;
		private DataView view;
		private ComponentResourceManager res = new ComponentResourceManager(typeof(BrowseForm)); 
		
		public BrowseForm()
		{
			
			this.InitializeComponents();
			
			this.MinimizeBox = false;
			this.MaximizeBox = false;

			this.Load += formLoad;
		}
		
		/// <summary>
		/// Определяет заголовок формы
		/// </summary>
		protected string Title {
			get {
				return this.Text;
			}
			
			set {
				this.Text = value;
			}
		}
		
		protected DataTable DataSource {			
			set {
				this.view = value.AsDataView();
				this.grid.DataSource = this.view;
			}
		}
		
		/// <summary>
		/// Определяет будет ли отображаться кнопка "Создать"
		/// </summary>
		protected bool ButtonCreate 
		{
			set { this.btnCreate.Visible = value; }
			get { return this.btnCreate.Visible; }
		}
		
		/// <summary>
		/// Определяет будет ли отображаться кнопка "Редактировать"
		/// </summary>
		protected bool ButtonEdit
		{
			set { this.btnEdit.Visible = value; }
			get { return this.btnEdit.Visible; }
		}
		
		/// <summary>
		/// Определяет будет ли отображаться кнопка "Удалить"
		/// </summary>
		protected bool ButtonDelete 
		{
			set { this.btnDelete.Visible = value; }
			get { return this.btnDelete.Visible; }
		}
		
		private void InitializeComponents() 
		{
			this.SuspendLayout();
			
			ToolStripButton btnExit = new ToolStripButton("Выход")
			{
				Image = ((Icon)res.GetObject("exit")).ToBitmap(),
				ImageAlign = ContentAlignment.MiddleLeft,
				ToolTipText = "Esc - Выход"
			};
			
			btnExit.Click += (sender, e) => this.Close();
			
			this.btnCreate = new ToolStripButton("Создать")
			{
				Image = ((Icon)res.GetObject("add")).ToBitmap(),
				ImageAlign = ContentAlignment.MiddleLeft,
				ToolTipText = "Ins - Создать строку"
			};
			
			this.btnEdit = new ToolStripButton("Редактировать")
			{
				Image = ((Icon)res.GetObject("edit")).ToBitmap(),
				ImageAlign = ContentAlignment.MiddleLeft,
				ToolTipText = "F2 - Редактировать строку"
			};
			
			this.btnDelete = new ToolStripButton("Удалить")
			{
				Image = ((Icon)res.GetObject("delete")).ToBitmap(),
				ImageAlign = ContentAlignment.MiddleLeft,
				ToolTipText = "Delete - Удалить строку"
			};
			
			btnCreate.Click += btnCreate_Click;
			btnEdit.Click += btnEdit_Click;			
			btnDelete.Click += btnDelete_Click;
			
			this.txtSearch = new ToolStripTextBox()
			{
				Alignment = ToolStripItemAlignment.Right,
				Padding = new Padding(0, 0, 5, 0),
				AutoSize = false,
				Width = 200,
				BorderStyle = BorderStyle.FixedSingle
			};
			
			this.txtSearch.KeyUp += this.txtSearch_KeyUp;
			
			ToolStripButton btnSearch = new ToolStripButton("Поиск")
			{
				Alignment = ToolStripItemAlignment.Right,
				Padding = new Padding(0, 0, 5, 0)
			};
			
			btnSearch.Click += (sender, e)=> this.view.FilterText(this.txtSearch.Text);
			
			this.tools = new ToolStrip();
			
			this.tools.Items.AddRange(new ToolStripItem[] {
										btnExit,			                     	
			                     		this.btnCreate, 
			                     		this.btnEdit, 
			                     		this.btnDelete,
			                          	this.txtSearch,
			                          	btnSearch
			                          });
			
			this.grid = new BrowseDataGridView()
			{
				Dock = DockStyle.Fill,
				ReadOnly = true,
				RowHeadersVisible = false,
				AutoGenerateColumns = false,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
			};
			
			this.grid.Key += delegate(object sender, KeyEventArgs e) 
			{
				switch (e.KeyCode) {
					case Keys.Escape: 
						btnExit.PerformClick();
						break;
					case Keys.F2:
						this.btnEdit.PerformClick();
						break;
					case Keys.Insert:
						this.btnCreate.PerformClick();
						break;
					case Keys.Delete:
						this.btnDelete.PerformClick();
						break;
				}
			};
			
			this.Controls.Add(this.grid);
			this.Controls.Add(tools);
		
			this.Icon = (Icon)res.GetObject("book");
			
			this.ShowInTaskbar = false;
			this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width,
			                     Screen.PrimaryScreen.WorkingArea.Height);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.actionsSeparator = false;
			
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		
		private void txtSearch_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) {
				e.SuppressKeyPress = true;
				
				this.view.FilterText(this.txtSearch.Text);
				this.grid.Focus();
			}
		}
		
		/// <summary>
		/// Обработчик события нажания кнопки "Удалить". Предназначен для переопределения в дочернем классе.
		/// </summary>
		protected virtual void btnDelete_Click(object sender, EventArgs e) {}
		/// <summary>
		/// Обработчик события нажания кнопки "Изменить". Предназначен для переопределения в дочернем классе.
		/// </summary>
		protected virtual void btnEdit_Click(object sender, EventArgs e) {}
		/// <summary>
		/// Обработчик события нажания кнопки "Создать". Предназначен для переопределения в дочернем классе.
		/// </summary>
		protected virtual void btnCreate_Click(object sender, EventArgs e) {}
		/// <summary>
		/// Обработчик события Load формы. Предназначен для определения в нем 
		/// логики загрузки данных, в дочерней форме.
		/// </summary>
		protected virtual void formLoad(object sender, EventArgs e) {}
		/// <summary>
		/// Добавляет столбец отображаемых данных в источнике DataSource
		/// </summary>
		/// <param name="columnName">Наименование столбца в источнике данных</param>
		/// <param name="headerText">Заголовок столбца на форме</param>
		/// <param name="width">Ширина столбца</param>
		protected void AddColumn(string columnName, 
		                         string headerText, 
		                         int width = 100)
		{
			this.grid.Columns.Add(columnName, headerText);
			this.grid.Columns[columnName].DataPropertyName = columnName;
			this.grid.Columns[columnName].Width = width;
		}
		
		protected void AddAction(string actionTitle, EventHandler action)
		{
			if (!this.actionsSeparator) {
				this.tools.Items.Add(new ToolStripSeparator());
				this.tools.Items.Add(new ToolStripSeparator());
				this.actionsSeparator = true;
			}
			
			Bitmap go = ((Icon)this.res.GetObject("go")).ToBitmap();
			
			this.tools.Items.Add(actionTitle, go, action);
		}
		
		protected override void WndProc(ref Message m)
		{
			// Запрет перемещения формы по экрану
			const int WM_NCLBUTTONDOWN = 161;
			const int WM_SYSCOMMAND = 274;
			const int HTCAPTION = 2;
			const int SC_MOVE = 61456;
			
			if ((m.Msg == WM_SYSCOMMAND) && (m.WParam.ToInt32() == SC_MOVE))
				return;
			
			if ((m.Msg == WM_NCLBUTTONDOWN) && (m.WParam.ToInt32() == HTCAPTION))
				return;
			
			base.WndProc(ref m);
		}
	}
}
