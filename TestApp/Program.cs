using DatabaseObjectSerializer;

namespace TestApp
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            DatabaseLink database = new DatabaseLink("./Dbase.accdb");

            User U = new User();
            U.password = "fsfgsd";
            U.username = "dfsfds";
            U.Insert(database, "Users");

            //User[] T = database.ExecuteRead<User>("select * from Users", typeof(User));
            
            //T[0].password = "AChmbfsdsffded";
            //T[1].password = "Chummadfsfdsfr";

            //T[0].Update(database, "Users");
            //T[1].Update(database, "Users");

            //database.ExecuteCommand<User>("UPDATE Users SET passwrd = 'Moffo'", T[0]);
        }

        #endregion Methods
    }

    public class User : DatabaseObject
    {
        #region Fields

        public string username, password;

        #endregion Fields
    }
}