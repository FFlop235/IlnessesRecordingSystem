# Реализация данного задания: 

### План-задание для студентов 2 курса  
**«Система учёта заболеваемости сотрудников предприятия»**  
*Разработка Avalonia MVVM-приложения с подключением к MySQL*  

---

#### 🔧 Требуемые NuGet-пакеты  
```bash
dotnet add package MySqlConnector          # Работа с MySQL
dotnet add package Avalonia.Controls.DataGrid  # Таблица для отображения данных
dotnet add package MessageBox.Avalonia     # Диалоговые окна (подтверждение, ошибки)
```

Для работы потребуется бд и субд
- Ссылки: 
- mariadb `https://mariadb.com/downloads/community/community-server/`
- dbeaver `https://github.com/dbeaver/dbeaver/releases`
---

### 🗂️ Структура базы данных 

#### Таблица 1: Отделы предприятия (`Departments`)
| Поле       | Тип         | Описание               |
|------------|-------------|------------------------|
| Id         | INT PK AI   | Уникальный номер       |
| Name       | VARCHAR(50) | Название отдела        |
| Floor      | INT         | Этаж размещения        |

#### Таблица 2: Сотрудники (`Employees`)
| Поле          | Тип         | Описание                     |
|---------------|-------------|------------------------------|
| Id            | INT PK AI   | Уникальный номер             |
| FullName      | VARCHAR(100)| ФИО сотрудника               |
| DepartmentId  | INT FK      | Ссылка на отдел              |
| Position      | VARCHAR(50) | Должность                    |
| HireDate      | DATE        | Дата приема на работу        |

#### Таблица 3: Типы заболеваний (`IllnessTypes`)
| Поле         | Тип         | Описание                          |
|--------------|-------------|-----------------------------------|
| Id           | INT PK AI   | Уникальный номер                  |
| Name         | VARCHAR(50) | Название (грипп, ОРВИ, корь...)  |
| IsInfectious | BOOLEAN     | Признак инфекционности (1/0)     |
| QuarantineDays| INT        | Рекомендуемый карантин (дней)    |

#### Таблица 4: Факты заболеваний (`IllnessRecords`)
| Поле          | Тип         | Описание                              |
|---------------|-------------|---------------------------------------|
| Id            | INT PK AI   | Уникальный номер                      |
| EmployeeId    | INT FK      | Ссылка на сотрудника                  |
| IllnessTypeId | INT FK      | Ссылка на тип заболевания             |
| StartDate     | DATE        | Дата начала болезни                   |
| EndDate       | DATE        | Дата окончания болезни                |
| DiagnosisNote | TEXT        | Примечание врача (опционально)        |

> — ФИО сотрудника хранится один раз в `Employees`, а не дублируется в каждой записи болезни.  
> — Название заболевания хранится в справочнике `IllnessTypes`, а не повторяется в каждой строке.  
> — При изменении названия отдела достаточно обновить одну запись в `Departments`.

---

### 📋 Функциональные требования к приложению

#### Функция 1: Просмотр списка заболеваний (пагинация)
- Отображать данные в `DataGrid` со столбцами:  
  `ФИО сотрудника | Отдел | Тип заболевания | Дата начала | Дата окончания | Длительность (дней)`
- На одной странице — **ровно 10 записей**.
- Элементы управления:  
  `[<<] [<] [Страница 2 из 7] [>] [>>]`
  `[<<] - первая страница`
  `[>>] - последняя страница`
  `[<] - предыдущая страница`
  `[>] - следующая страница`
- При переключении страницы — данные обновляются без перезагрузки окна.

#### Функция 2: Добавление новой записи
1. Нажатие кнопки «Добавить» → открывается модальное окно.
2. Поля ввода:  
   - Выбор сотрудника из выпадающего списка (`ComboBox` с ФИО)  
   - Выбор типа заболевания из списка (`ComboBox` с названиями)  
   - Поля дат: `Дата начала`, `Дата окончания` (`DatePicker`)  
   - Текстовое поле «Примечание врача» (необязательно)
3. Кнопки: «Сохранить» / «Отмена».
4. **Валидация перед сохранением**:  
   - Все обязательные поля заполнены  
   - `Дата окончания` ≥ `Дата начала`  
   - При ошибке — показать `MessageBox` с текстом ошибки.

#### Функция 3: Редактирование записи
1. Двойной клик по строке в `DataGrid` → открытие окна редактирования с заполненными полями.
2. Все те же поля, что и при добавлении.
3. Кнопка «Удалить запись» внутри окна редактирования (с подтверждением через `MessageBox`).

#### Функция 4: Удаление записи
1. Выбор строки в таблице + нажатие кнопки «Удалить».
2. Появляется диалог:  
   > «Вы уверены, что хотите удалить запись о заболевании сотрудника Иванов А.А. (грипп, 10.01–17.01)?»  
   > Кнопки: **Да** / **Нет**
3. При подтверждении — запись удаляется из БД и таблица обновляется.

#### Функция 5: Статистика (вкладка или панель справа)
Отображать 4 блока информации:

| Блок статистики               | Пример вывода                                
|-------------------------------|----------------------------------------------
| Топ-3 заболеваний             | 1. Грипп — 12 случаев 2. ОРВИ — 8 случаев 
| Средняя длительность          | Грипп: 6.2 дня, ОРВИ: 4.1 дня                
| Самые «болеющие» отделы       | 1. Бухгалтерия — 24 дня болезни              
| Доля инфекционных заболеваний | Инфекционные: 68% (21 из 31)               
| Самый стойкий сотрудник       | Иванов А.А. Случаев болезни - 1                     

> Статистика обновляется автоматически при загрузке данных и после сохранения/удаления записи.

---

### 💡 Примеры работы с компонентами 

#### Пример 1: Простое подтверждение удаления через MessageBox
```csharp
// В обработчике кнопки "Удалить"
var messageBox = MessageBoxManager
    .GetMessageBoxStandard("Подтверждение", 
        $"Удалить запись о заболевании сотрудника {выбраннаяЗапись.EmployeeName}?",
        ButtonEnum.YesNo);
    
var result = await messageBox.ShowWindowDialog(this); // this = текущее окно

if (result == ButtonResult.Yes)
{
    // Синхронное удаление из БД
    _databaseService.DeleteIllnessRecord(выбраннаяЗапись.Id);
    LoadData(); // Перезагрузка таблицы
}
```

#### Пример 2: Отображение ошибки валидации
```csharp
// При нажатии "Сохранить" в окне редактирования
if (string.IsNullOrWhiteSpace(EmployeeName))
{
    var errorBox = MessageBoxManager
        .GetMessageBoxStandard("Ошибка", "Укажите ФИО сотрудника", ButtonEnum.Ok);
    await errorBox.ShowWindowDialog(this);
    return;
}

if (EndDate < StartDate)
{
    var errorBox = MessageBoxManager
        .GetMessageBoxStandard("Ошибка", "Дата окончания не может быть раньше даты начала", ButtonEnum.Ok);
    await errorBox.ShowWindowDialog(this);
    return;
}
// ... сохранение в БД
```

#### Пример 3: Постраничный запрос к БД 
```csharp
// В классе DatabaseService
public List<IllnessRecordView> GetPage(int pageNumber, int pageSize)
{
    var result = new List<IllnessRecordView>();
    
    using (var conn = new MySqlConnection(_connectionString))
    {
        conn.Open();
        
        var cmd = new MySqlCommand(@"
            SELECT 
                ir.Id,
                e.FullName AS EmployeeName,
                d.Name AS DepartmentName,
                it.Name AS IllnessType,
                ir.StartDate,
                ir.EndDate,
                DATEDIFF(ir.EndDate, ir.StartDate) AS DurationDays
            FROM IllnessRecords ir
            JOIN Employees e ON ir.EmployeeId = e.Id
            JOIN Departments d ON e.DepartmentId = d.Id
            JOIN IllnessTypes it ON ir.IllnessTypeId = it.Id
            ORDER BY ir.StartDate DESC
            LIMIT @pageSize OFFSET @offset", conn);
            
        cmd.Parameters.AddWithValue("@pageSize", pageSize);
        cmd.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);
        
        using (var reader = cmd.ExecuteReader()) 
        {
            while (reader.Read())
            {
                result.Add(new IllnessRecordView
                {
                    Id = reader.GetInt32("Id"),
                    EmployeeName = reader.GetString("EmployeeName"),
                    DepartmentName = reader.GetString("DepartmentName"),
                    IllnessType = reader.GetString("IllnessType"),
                    StartDate = reader.GetDateTime("StartDate"),
                    EndDate = reader.GetDateTime("EndDate"),
                    DurationDays = reader.GetInt32("DurationDays")
                });
            }
        }
    }
    
    return result;
}
```
#### Пример 4: топ-5 отделов по дням болезни
```sql
SELECT 
    d.Name AS DepartmentName,
    SUM(DATEDIFF(ir.EndDate, ir.StartDate)) AS TotalSickDays,
    COUNT(ir.Id) AS IllnessCount,
    ROUND(AVG(DATEDIFF(ir.EndDate, ir.StartDate)), 1) AS AvgDuration
FROM IllnessRecords ir
JOIN Employees e ON ir.EmployeeId = e.Id
JOIN Departments d ON e.DepartmentId = d.Id
GROUP BY d.Id, d.Name
ORDER BY TotalSickDays DESC
LIMIT 5;
```


---

### 📝 Этапы выполнения задания

| Шаг | Задача                                                                 |
|--------|------------------------------------------------------------------------|
| 1      | Создать БД по схеме 3НФ, заполнить тестовыми данными (минимум 3 отдела, 15 сотрудников, 30 записей заболеваний) |
| 2      | Реализовать подключение к БД, отобразить все записи в `DataGrid` (без пагинации) |
| 3      | Добавить пагинацию (10 записей на страницу) + элементы управления      |
| 4      | Реализовать добавление и удаление записей с подтверждением через `MessageBox` |
| 5      | Добавить редактирование записей + валидацию полей                      |
| 6      | Реализовать 4 блока статистики с использованием SQL-агрегатов          |
| 7      | Финальная доработка UI: группировка элементов, цветовое оформление, тестирование |

--
