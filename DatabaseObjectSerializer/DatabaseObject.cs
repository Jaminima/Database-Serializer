using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Reflection;

namespace DatabaseObjectSerializer
{
    public class DatabaseObject
    {
        public int Id;

        public void Update(DatabaseLink databaseLink, string DBTable)
        {
            Type type = this.GetType();

            string Set = "";
            Dictionary<string, object> paramaters = new Dictionary<string, object>();

            foreach (FieldInfo field in type.GetFields())
            {
                if (field.Name != "DBTable" && field.Name!="Id")
                {
                    paramaters.Add(field.Name, field.GetValue(this));
                    Set +="["+ field.Name + "] = @" + field.Name + ", ";
                }
            }
            Set = Set.Substring(0, Set.Length - 2);

            paramaters.Add("Id", Id);

            databaseLink.ExecuteCommand("UPDATE " + DBTable + " SET " + Set + " WHERE Id = @Id", paramaters);
        }
    }
}
