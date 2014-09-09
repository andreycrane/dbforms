Database Forms 
=================================

Набор базовых классов для .NET упрощающих разработку оконных приложений 
для работы с базами данных. Классы представленные в библиотеке упрощают
программирование Windows-форм для выполнения типичных CRUD-операций.
Данная библиотека не предоставляет средств для взаимеодействия с какой-либо
конкретной СУБД, т.к. предназначенна для использования с любым провайдером
данных: SQLite,  MySQL и т.д.

В библиотеке реализованы следующие базовые абстрактные классы:

* BrowseForm - форма просмотре и редактирования табличной информации
* EditForm - форма редактирования элемента таблицы
* SelectForm - форма выбора элемента табличной информации (справочник)
* Helpers - статический класс с несколькими расширениями

Ключевые идеи
---------------------------------

В большинстве случаев при создании Windows-приложений работающих с базами данных
приходится выполнять одну и ту же рутинную работу создания форм для отображения
табличных данных. Каждый раз в дизайнере или тем более вручную в 
программном коде мы добавляем элементы управления сетки, кнопок "добавить", "изменить", 
"удалить", текстового поля поиска по таблице и т.д. Кроме того нам приходится 
создавать еще и новую форму для редактирования-обновления этих данных. Каждая новая 
таблица (совокупность таблиц) в базе данных - это новый набор форм в приложении.

Этот небольшой проект, пусть даже и один из многих, призван решить эту проблему.
Каким именно образом, показано в примерах приведенных ниже, а также в проекте
включенном в данный репозиторий, под названием TestForms - представляющий
небольшое Windows-приложение работающее с тестовой базой данных World входящей
в поставку "MySQL 5.6".

Примеры
---------------------------------

Задача: необходима форма для отображения табличных данных взятых из 
БД.

```csharp
	using System;
	using System.Data;
	using System.Windows.Forms;
	using MySql.Data.MySqlClient;
	using DbForms;
	
	// Наследуемся от базового класса табличной формы
	class CityBrowse : BrowseForm
	{
		// Объект подключения к СУБД и загружаемой таблицы
		private MySqlConnection conn;
		private DataTable tbl;
		
		// Определяем дочерний конструктор в котором настраиваем форму
		public CityBrowse(MySqlConnection conn)
		{
			this.conn = conn;
			this.tbl = new DataTable("city");
			
			// заголовок формы
			this.Title = "Города";
			// Список отображаемых столбцов
			// название столбца в объекте источника данных, заголовок, ширина (по умолчанию 100px)
			this.AddColumn("Name", "Название", 200);
			this.AddColumn("CountryCode", "Код страны");
			this.AddColumn("District", "Столица");
			this.AddColumn("Population", "Численность населения");
			// определяем кнопки дополнительных действий
			this.AddAction("Фильтр по стране", this.filterByCountry);
		}
		// переопределяем метод загрузки формы
		// реализуем в ней загрузку данных
		protected override void formLoad(object sender, EventArgs e) 
		{	
			// загружаем данные
			string strSql = @"select 
									id,
									Name,
									CountryCode,
									District,
									Population
								from city";			
			MySqlCommand cmd = new MySqlCommand(strSql, this.conn);
			MySqlDataReader dataReader = cmd.ExecuteReader();
			this.tbl.Load(dataReader);
			// определям источник данных отображаемых на форме
			this.DataSource = this.tbl;
		}
		// определяем действия выполняемы по нажатию на кнопки "Создать", "Редактировать", "Удалить"
		protected sealed override void btnCreate_Click(object sender, EventArgs e) {}
		protected sealed override void btnEdit_Click(object sender, EventArgs e) {}
		protected sealed override void btnDelete_Click(object sender, EventArgs e) {}
		// обработчик дополнительного действия фильтра по стране
		private void filterByCountry(object sender, EventArgs e) { }
	}
	
	CityBrowse form = new CityBrowse(conn);
	form.ShowDialog();
```

В результате получаем слeдующее:

![Изображение результрующей формы](http://firepic.org/images/2014-09/09/emscb2s8a8x3.png "Результирующая форма")