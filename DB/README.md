<strong>MSSQL Connection Class for C# </strong><br/>
 Setup singleton class just like that<br/>
```csharp
//setup connection string 
MSSql.Instance.Setup("<your connection string here>")
```
the just call according Execute method you want
```csharp

DataTable dt=MSSql.Instance.Execute("select * from blabla;");

//call with arguments
DataTable dt=MSSql.Instance.Execute("select * from blabla where id={0};",456789);

//calling execute nonquery
MSSql.Instance.ExecuteNonQuery("update blabla set  name=N'zrt prt' where id={0};",456789);

//call multiple query
DataTable[] dt=MSSql.Instance.Execute("select * from blabla1;select * from blabla2;");

```
