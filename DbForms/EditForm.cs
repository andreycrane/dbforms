/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 26.08.2014
 * Time: 16:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace DbForms
{
	/// <summary>
	/// Базовый класс форм, предназначенных для редактирования полей
	/// </summary>
	public abstract class EditForm : Form
	{
		public class ColumnProperties 
		{
			public bool? ReadOnly { get; set; }
			public bool? Enabled { get; set; }
			public int? DecimalPlaces { get; set; }
			public decimal? Minimum { get; set; }
			public decimal? Maximum { get; set; }
			public decimal? Increment { get; set; }
			public string Mask { get; set; }
		}
		
		private ComponentResourceManager res = new ComponentResourceManager(typeof(EditForm));
		
		private List<Label> formLabels;
		private Dictionary<string, Control> formControls;
		private Dictionary<string, Type> valueTypes;
		private Dictionary<string, object> hiddenValues;
		
		private TableLayoutPanel panel;
		private Button btnOk;
		private Button btnCancel;
		
		public EditForm()
		{
			this.formLabels = new List<Label>();
			this.formControls = new Dictionary<string, Control>();
			this.valueTypes = new Dictionary<string, Type>();
			this.hiddenValues = new Dictionary<string, object>();
			
			this.InitializeComponents();
		}
		
		private void InitializeComponents()
		{
			this.SuspendLayout();
			
			Size s =  new Size(800, 100);
			this.Size = s;
			
			this.btnOk = new Button()
			{
				Text = "Ok",
				Height = 25,
				Image = ((Icon)this.res.GetObject("dialog-ok")).ToBitmap(),
				ImageAlign = ContentAlignment.MiddleLeft,
				Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
				DialogResult = DialogResult.OK,
				TabIndex = 1
			};
			
			this.btnCancel = new Button()
			{
				Text = "Отмена",
				Height = 25,
				Width = 85,
				Image = ((Icon)this.res.GetObject("dialog-cancel")).ToBitmap(),
				ImageAlign = ContentAlignment.MiddleLeft,
				Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
				DialogResult = DialogResult.Cancel,
				TabIndex = 2
			};
			
			this.btnOk.Location = new Point(
				this.ClientSize.Width - this.btnCancel.Width - this.btnOk.Width - 3,
				this.ClientSize.Height - this.btnOk.Height);
			
			this.btnCancel.Location = new Point(
				this.ClientSize.Width - this.btnCancel.Width,
				this.ClientSize.Height - this.btnCancel.Height);
			
			this.btnOk.Click += this.btnOk_Click;
			this.btnCancel.Click += this.btnCancel_Click;
			
			this.panel = new TableLayoutPanel()
			{
				ColumnCount = 2,
				Anchor = AnchorStyles.Top | 
					AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left
			};
			
			this.panel.Location = new Point(0, 0);
			this.panel.Size = new Size(this.ClientSize.Width, 
			                           this.ClientSize.Height - 
			                           (this.ClientSize.Height - this.btnOk.Location.Y));
			
			this.Controls.Add(this.panel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			
			this.CancelButton = this.btnCancel;
			
			this.Icon = (Icon)this.res.GetObject("form");
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		
		public string Title {
			set { this.Text = value; }
			get { return this.Text; }
		}
		
		protected void AddHiddenColumn(string columnName, object val)
		{
			if(this.hiddenValues.ContainsKey(columnName))
				throw new DuplicateNameException();
			
			this.hiddenValues.Add(columnName, val);
		}
		
		protected void AddColumn(string columnName, 
		                         string columnHeader, 
		                         string val, 
		                         EventHandler clickHandler = null, 
		                         ColumnProperties properties = null)
		{
			this.AddColumn(columnName, columnHeader, val, typeof(string), clickHandler, properties);
		}
		
		protected void AddColumn(string columnName, 
		                         string columnHeader, 
		                         int val, 
		                         EventHandler clickHandler = null,
		                         ColumnProperties properties = null)
		{
			this.AddColumn(columnName, columnHeader, val, typeof(string), clickHandler, properties);
		}
		
		protected void AddColumn(string columnName, 
		                         string columnHeader, 
		                         bool val, 
		                         EventHandler clickHandler = null,
		                         ColumnProperties properties = null)
		{
			this.AddColumn(columnName, columnHeader, val, typeof(bool), clickHandler, properties);
		}
		
		protected void AddColumn(string columnName, 
		                         string columnHeader, 
		                         float val, 
		                         EventHandler clickHandler = null,
		                         ColumnProperties properties = null)
		{
			this.AddColumn(columnName, columnHeader, val, typeof(float), clickHandler, properties);
		}
		
		private void AddColumn(string columnName, 
		                         string columnHeader, 
		                         object val, 
		                         Type valueType,
		                         EventHandler clickHandler = null, 
		                         ColumnProperties properties = null)
		{
			if (this.hiddenValues.ContainsKey(columnName) || 
			    this.valueTypes.ContainsKey(columnName))
				throw new DuplicateNameException();
			
			Type valType;
			
			if(val != null) { 
				valType = val.GetType();
			} else if(valueType != null) {
				valType = valueType;
			} 
			else throw new NullReferenceException();
			
			if (clickHandler == null) {
				this.AddLabel(columnHeader);
			} else {
				this.AddLinkLabel(columnHeader, clickHandler);
			}
			
			switch (valType.Name) {
				case "Single":
				case "Int32":
					this.AddNumericUpDown(columnName, Convert.ToDecimal(val), properties);
					break;
				case "String":
					this.AddTextBox(columnName, Convert.ToString(val), properties);
					break;
				case "Boolean":
					this.AddCheckBox(columnName, Convert.ToBoolean(val), properties);
					break;
				default:
					throw new Exception("Тип " + valType.Name + " не поддерживается библиотекой!");
			}
			
			this.valueTypes.Add(columnName, valType);
		}
		
		private void AddTextBox(string columnName, string val, ColumnProperties properties = null)
		{
			MaskedTextBox txt = new MaskedTextBox()
			{
				Text = val,
				BeepOnError = true,
				PromptChar = ' ',
				Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
			};
			
			this.panel.Controls.Add(txt, 1, this.formControls.Count);
			this.formControls.Add(columnName, txt);
			
			if(properties == null) return;
			
			if (properties.ReadOnly.HasValue) txt.ReadOnly = properties.ReadOnly.Value;
			if (properties.Enabled.HasValue) txt.Enabled = properties.Enabled.Value;
			if (properties.Mask != null) txt.Mask = properties.Mask;
		}
		
		private void AddNumericUpDown(string columnName, decimal val, ColumnProperties properties = null)
		{
			NumericUpDown numeric = new NumericUpDown
			{	
				Maximum = decimal.MaxValue,
				Minimum = decimal.MinValue,
				Value = val,
				Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
			};
			
			this.panel.Controls.Add(numeric, 1, this.formControls.Count);
			this.formControls.Add(columnName, numeric);
			
			if (properties == null) return;
			if (properties.DecimalPlaces.HasValue) numeric.DecimalPlaces = properties.DecimalPlaces.Value;
			if (properties.Minimum.HasValue) numeric.Minimum = properties.Minimum.Value;
			if (properties.Maximum.HasValue) numeric.Maximum = properties.Maximum.Value;
			if (properties.Increment.HasValue) numeric.Increment = properties.Increment.Value;
		}
		
		private void AddCheckBox(string columnName, bool val, ColumnProperties properties = null)
		{
			CheckBox check = new CheckBox()
			{
				Checked = val,
				Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom
			};
			
			this.panel.Controls.Add(check, 1, this.formControls.Count);
			this.formControls.Add(columnName, check);
			
			if (properties == null) return;
			
			if (properties.Enabled.HasValue) check.Enabled = properties.Enabled.Value;
		}
		
		private void AddLabel(string columnHeader)
		{
			Label lblField = new Label()
			{
				Text = columnHeader,
				TextAlign = ContentAlignment.MiddleLeft,
				Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
				AutoSize = true
			};
			
			this.panel.Controls.Add(lblField, 0, this.formLabels.Count);
			this.formLabels.Add(lblField);
		}
		
		private void AddLinkLabel(string columnHeader, EventHandler clickHandler) 
		{
			LinkLabel lblField = new LinkLabel()
			{
				Text = columnHeader,
				TextAlign = ContentAlignment.MiddleLeft,
				Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
				AutoSize = true
			};
			
			lblField.Click += clickHandler;
			lblField.KeyUp += delegate(object sender, KeyEventArgs e) {
				if (e.KeyCode == Keys.Space)
					clickHandler.Invoke(this, e);
			};
			
			this.panel.Controls.Add(lblField, 0, this.formLabels.Count);
			this.formLabels.Add(lblField);
		}
		
		protected void SetValue(string columnName, object val)
		{
			if (this.hiddenValues.ContainsKey(columnName)) {
				this.hiddenValues[columnName] = val;
				return;
			}
			
			Control ctrl = this.formControls[columnName];
			Type t = ctrl.GetType();
			
			switch (t.Name) {
				case "NumericUpDown":
					NumericUpDown numeric = (NumericUpDown)ctrl;
					numeric.Value = Convert.ToDecimal(val);
					break;
					
				case "MaskedTextBox":
				case "TextBox":
					TextBoxBase txt = (TextBoxBase)ctrl;
					txt.Text = Convert.ToString(val);
					break;
				
				case "CheckBox":
					CheckBox chk = (CheckBox)ctrl;
					chk.Checked = Convert.ToBoolean(val);
					break;
					
				default:
					throw new KeyNotFoundException();
			}
		}
		
		protected object GetValue(string columnName)
		{
			if (this.hiddenValues.ContainsKey(columnName))
				return this.hiddenValues[columnName];
			
			Control ctrl = this.formControls[columnName];
			
			Type t = ctrl.GetType();
			
			switch (t.Name) {
				case "MaskedTextBox":
				case "TextBox":
					TextBoxBase txt = (TextBoxBase)ctrl;
					return this.FromStringToStartValue(columnName, txt.Text);
				
				case "CheckBox":
					CheckBox chk = (CheckBox)ctrl;
					return chk.Checked;
					
				case "NumericUpDown":
					NumericUpDown numeric = (NumericUpDown)ctrl;
					return this.FromStringToStartValue(columnName, Convert.ToString(numeric.Value));
					
				default:
					throw new KeyNotFoundException();
			}

		}
		
		private object FromStringToStartValue(string columnName, string val)
		{
			Type startType = this.valueTypes[columnName]; 
			
			if (startType == typeof(string)) {
				return val;
			} else if(startType == typeof(int)) {
				return Convert.ToInt32(val);
			} else if(startType == typeof(float)) {
				return Convert.ToSingle(val);
			}
			
			return new object();
		}
		
		public new DialogResult ShowDialog()
		{
			this.ClientSize = new Size(this.ClientSize.Width,
			                           this.panel.GetRowHeights().Sum() + 
			                           this.btnOk.Height + 5);
			
			this.MinimumSize = this.Size;
			this.MaximumSize = this.Size;
			
			return base.ShowDialog();
		}
		
		protected virtual void btnOk_Click(object sender, EventArgs e) { }
		protected virtual void btnCancel_Click(object sender, EventArgs e) { }
	}
}
