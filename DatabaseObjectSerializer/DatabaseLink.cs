using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Reflection;
using System.Linq;

namespace DatabaseObjectSerializer
{
    public class DatabaseLink
    {
        #region Fields

        private OleDbConnection connection;

        #endregion Fields

        #region Methods

        private OleDbCommand PrepareCommand(string StrCommand, Dictionary<string, object> parameters = null)
        {
            OleDbCommand command = new OleDbCommand(StrCommand, connection);

            if (parameters != null) {
                foreach (string segment in StrCommand.Split(' ').Where(x => x.StartsWith("@")))
                {
                    string S = segment.Replace("@", "").Replace(";","").Replace(",","");
                    if (parameters.ContainsKey(S))
                    { command.Parameters.AddWithValue(S, parameters[S]); }
                }
            }
            return command;
        }

        private OleDbCommand PrepareCommand<T>(string StrCommand, T obj, Dictionary<string, object> parameters = null)
        {
            if (parameters == null) { parameters = new Dictionary<string, object>(); }
            Type t = obj.GetType();
            foreach (FieldInfo field in t.GetFields().Where(x => StrCommand.Contains("@" + x.Name)))
            {
                parameters.Add(field.Name, field.GetValue(obj));
            }
            return PrepareCommand(StrCommand, parameters);
        }

        #endregion Methods

        #region Constructors

        public DatabaseLink(string DBFile)
        {
            connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+DBFile+";");
            connection.Open();
        }

        #endregion Constructors

        public void ExecuteCommand(string StrCommand, Dictionary<string, object> parameters = null)
        {
            OleDbCommand command = PrepareCommand(StrCommand, parameters);
            command.ExecuteNonQuery();
        }

        public void ExecuteCommand<T>(string StrCommand, T obj, Dictionary<string, object> parameters = null)
        {
            OleDbCommand command = PrepareCommand<T>(StrCommand, obj, parameters);
            command.ExecuteNonQuery();
        }

        public void ExecuteCommand<T>(string StrCommand, T[] objs, Dictionary<string, object> parameters = null)
        {
            foreach (T obj in objs)
            {
                ExecuteCommand<T>(StrCommand, obj, parameters);
            }
        }

        public List<object[]> ExecuteRead(string StrCommand, Dictionary<string, object> parameters = null)
        {
            OleDbCommand command = PrepareCommand(StrCommand, parameters);
            OleDbDataReader reader = command.ExecuteReader();

            List<object[]> objs = new List<object[]>();
            while (reader.Read())
            {
                object[] obj = new object[reader.FieldCount];
                for (int i = 0; i < obj.Length; i++)
                {
                    obj[i] = reader.GetValue(i);
                }
                objs.Add(obj);
            }
            return objs;
        }

        public T[] ExecuteRead<T>(string StrCommand, Type t, Dictionary<string, object> parameters = null)
        {
            OleDbCommand command = PrepareCommand(StrCommand, parameters);
            OleDbDataReader reader = command.ExecuteReader();

            List<T> Objs = new List<T>();
            while (reader.Read())
            {
                T obj = (T)Activator.CreateInstance(t);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    FieldInfo field = t.GetField(reader.GetName(i));
                    field.SetValue(obj, reader.GetValue(i));
                }
                Objs.Add(obj);
            }
            return Objs.ToArray();
        }
    }
}