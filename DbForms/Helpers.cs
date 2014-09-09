/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 04.09.2014
 * Time: 21:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Data;

namespace DbForms  
{	
	/// <summary>
	///  Утилиты упрощающие программирование форм работы с данными
	/// </summary>
	public static class Helpers
	{
		/// <summary>
		/// Расширение типа char методом Replicate, который формирует строку
		/// из указанного символа повторенного указанное количество раз.
		/// <param name="howMany">Количество раз, которое должен быть повторен символ</param>
		/// <returns>Строка содержащая символ, повторенный указанное количество раз</returns>
		/// </summary>
		public static string Replicate(this char ob, int howMany)
		{
			return new StringBuilder(howMany).Append(ob, howMany).ToString();
		}
		
		public static void FilterText(this DataView view, string text)
		{
			if (text.Length == 0) {
				view.RowFilter = String.Empty;
				return;
			}
			
			StringBuilder filterExpression = new StringBuilder();
			string pattern = String.Empty;
			
			foreach (DataColumn column in view.Table.Columns) 
				if(column.DataType == typeof(string)) {
					pattern = (filterExpression.Length > 0) ? "OR {0} LIKE '*{1}*'" : "{0} LIKE '*{1}*'"; 
					filterExpression.AppendFormat(pattern, column.ColumnName, text);
				}
			
			view.RowFilter = filterExpression.ToString();
		}
	}
}