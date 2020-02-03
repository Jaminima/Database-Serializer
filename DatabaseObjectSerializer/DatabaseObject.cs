using System;
using System.Collections.Generic;
using System.Reflection;

namespace DatabaseObjectSerializer
{
    public class DatabaseObject
    {
        #region Fields

        public int Id;

        #endregion Fields

        #region Methods

        public void Update(DatabaseLink databaseLink, string DBTable)
        {
            Type type = this.GetType();

            string Set = "";
            Dictionary<string, object> paramaters = new Dictionary<string, object>();

            foreach (FieldInfo field in type.GetFields())
            {
                if (field.Name != "Id")
                {
                    paramaters.Add(field.Name, field.GetValue(this));
                    Set += "[" + field.Name + "] = @" + field.Name + ", ";
                }
            }
            Set = Set.Substring(0, Set.Length - 2);

            paramaters.Add("Id", Id);

            databaseLink.ExecuteCommand("UPDATE " + DBTable + " SET " + Set + " WHERE Id = @Id", paramaters);
        }

        public void Insert(DatabaseLink databaseLink, string DBTable)
        {
            Type type = this.GetType();

            string Values = "", Collumns = "";
            Dictionary<string, object> paramaters = new Dictionary<string, object>();

            foreach (FieldInfo field in type.GetFields())
            {
                if (field.Name != "Id")
                {
                    paramaters.Add(field.Name, field.GetValue(this));
                    Values += "@"+field.Name + ", ";
                    Collumns += "["+field.Name + "], ";
                }
            }
            Values = Values.Substring(0, Values.Length - 2);
            Collumns = Collumns.Substring(0, Collumns.Length - 2);

            databaseLink.ExecuteCommand("INSERT INTO "+DBTable+" ( "+Collumns+" ) VALUES ( "+Values+" )", paramaters);
        }

        #endregion Methods
    }
}