namespace MyORM;

public class Database
{

}

// Вызвать в программ.сс(MyORM.Prog) методы Select, Insert(Вызвать 11 раз)
// Изменить Databse класс добавив 3 метода SelectById(int id), Update<T>(T t), DeleteById(int id)
// * В классе Database в методах не собирать sql запрос, а составлять с помощью linq (В первую очереь Select)
// * Сбилдить проект и подключить MyOrm.dll в MyHttpServer
// * в MyHttpServer сделать 5 методов в AccountController с работой БД (AddAccount, SelectAccounts, SelectByIdAccount, UpdateAccount, DeleteByIdAccount)
