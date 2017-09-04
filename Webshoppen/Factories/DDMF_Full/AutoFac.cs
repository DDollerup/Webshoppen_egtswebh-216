using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace Webshoppen
{
    // Koden kan bruges fridt så længe denne tekst bliver i toppen af filen.
    // Copyright 2016 E-train I/S, Udviklet af Henrik Obsen

    /// <summary>
    /// AutoFac tilføjer CRUD mm til en valgt model
    /// </summary>
    /// <typeparam name="T">Modellen der skal tilknyttes</typeparam>
    public class AutoFac<T> where T : new()
    {
        private string table;
        private Mapper<T> mapper = new Mapper<T>();

        public AutoFac()
        {
            table = "[" + typeof(T).Name + "]";
        }

        /// <summary>
        /// Metoden bruges til at hente en række ud fra et ID
        /// </summary>
        /// <param name="ID">Værdien i kolonnen ID</param>
        /// <returns>Retunere en model af den valgte type</returns>
        public T Get(int ID)
        {
            using (var cmd = new SqlCommand("SELECT * FROM " + table + " WHERE ID=@ID", Conn.CreateConnection()))
            {
                cmd.Parameters.AddWithValue("@ID", ID);

                var r = cmd.ExecuteReader();
                T type = new T();

                if (r.Read())
                {
                    type = mapper.Map(r);
                }

                r.Close();
                cmd.Connection.Close();
                return type;
            }
        }
        /// <summary>
        /// Bruges til at fylde modeller der ikke har en tabel i databasen for eksempel når man bruger Inner Join i til at samle flere tabeller i en model.
        /// </summary>
        /// <typeparam name="R">R er den nye model der ikke har en tabel eller er en samling af flere.</typeparam>
        /// <param name="SQL">SQL-sætningen der bruges til at fylde modellen R</param>
        /// <returns>Retunere en Liste af typen R</returns>
       public static List<R> ExecuteSQL<R>(string SQL) where R : new()
        {
            using (var cmd = new SqlCommand(SQL, Conn.CreateConnection()))
            {
                Mapper<R> mapper = new Mapper<R>();

                List<R> list = mapper.MapList(cmd.ExecuteReader());
                cmd.Connection.Close();
                return list;
            }           
        }


        /// <summary>
        /// Metoden bruges til at hente alle rækker i en tabel
        /// </summary>
        /// <returns>Retunere en liste af den valgte model</returns>
        public List<T> GetAll()
        {
            using (var cmd = new SqlCommand("SELECT * FROM " + table, Conn.CreateConnection()))
            {
                List<T> list = mapper.MapList(cmd.ExecuteReader());
                cmd.Connection.Close();
                return list;
            }
        }

        /// <summary>
        /// Metoden bruges til at hente flere rækker i en tabe sorteret efter en kolonne
        /// </summary>
        /// <param name="column">Navnet på kolonnen der skal sorteres fra</param>
        /// <param name="direction">Retninge DESC/ASC</param>
        /// <returns>Retunere en liste af den valgte model</returns>
        public List<T> GetAll(string column, string direction = "DESC")
        {
            using (var cmd = new SqlCommand("SELECT * FROM " + table + " ORDER BY " + column + " " + direction, Conn.CreateConnection()))
            {
                List<T> list = mapper.MapList(cmd.ExecuteReader());

                cmd.Connection.Close();

                return list;
            }
        }

        /// <summary>
        /// Metoden bruges til at hente flere rækker i en tabe sorteret efter en kolonne
        /// </summary>
        /// <param name="column">Navnet på kolonnen der skal sorteres efter</param>
        /// <param name="direction">Retninge DESC/ASC</param>
        /// <param name="amount">Antal rækker der skal retuneres</param>
        /// <returns>Retunere en liste af den valgte model</returns>
        public List<T> GetAll(string column, string direction, int amount)
        {
            using (var cmd = new SqlCommand("SELECT Top(" + amount + ") * FROM " + table + " ORDER BY " + column + " " + direction, Conn.CreateConnection()))
            {
                List<T> list = mapper.MapList(cmd.ExecuteReader());
                cmd.Connection.Close();
                return list;
            }
        }

        /// <summary>
        /// Metoden kan retuner x anatal rækker ud fra en kolonne med en bestemt værdi
        /// </summary>
        /// <param name="column">Navnet på kolonnen</param>
        /// <param name="value">Værdien der skal være i kolonnen</param>
        /// <returns></returns>
        public List<T> GetBy(string column, object value)
        {
            using (var cmd = new SqlCommand("SELECT * FROM " + table + " WHERE " + column + "=@KID", Conn.CreateConnection()))
            {
                cmd.Parameters.AddWithValue("@KID", value);

                List<T> list = mapper.MapList(cmd.ExecuteReader());
                cmd.Connection.Close();
                return list;
            }
        }

        /// <summary>
        /// Metoden kan retuner x anatal rækker ud fra en kolonne med en bestemt værdi
        /// </summary>
        /// <param name="column">Navnet på kolonnen er skal vælges ud fra</param>
        /// <param name="value">Værdien der skal være i kolonnen</param>
        /// <param name="ordercolumn">Navnet på kolonnen der skal sorteres efter</param>
        /// <param name="direction">Sorteringsretning DESC/ASC</param>
        /// <returns>Retunere en liste af den valgte model</returns>
        public List<T> GetBy(string column, object value, string ordercolumn, string direction = "DESC")
        {
            using (var cmd = new SqlCommand("SELECT * FROM " + table + " WHERE " + column + "=@KID ORDER BY " + ordercolumn + " " + direction, Conn.CreateConnection()))
            {
                cmd.Parameters.AddWithValue("@KID", value);

                List<T> list = mapper.MapList(cmd.ExecuteReader());
                cmd.Connection.Close();
                return list;
            }
        }

        /// <summary>
        /// Bruges til at tælle antal rækker i en tabel, ud fra en valgt model
        /// </summary>
        /// <returns>Retunere antal række som typen int</returns>
        public int Count()
        {
            using (var cmd = new SqlCommand("SELECT COUNT(ID) as antal FROM FROM " + table, Conn.CreateConnection()))
            {
                int counter = 0;
                var r = cmd.ExecuteReader();

                if (r.Read())
                {
                    counter = int.Parse(r["antal"].ToString());
                }

                r.Close();
                cmd.Connection.Close();
                return counter;
            }
        }

        /// <summary>
        /// Bruges til at tælle antal rækker i en tabel, ud fra en valgt model. Kan se efter en værdi i en bestemt kolonne
        /// </summary>
        /// <param name="column">Navnet på kolonnen er skal vælges ud fra</param>
        /// <param name="value">Værdien der skal være i kolonnen</param>
        /// <returns>Retunere antal række som typen int</returns>
        public int Count(string column, object value)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM FROM " + table + " WHERE " + column + "=@KID", Conn.CreateConnection()))
            {
                cmd.Parameters.AddWithValue("@KID", value);
                int counter = 0;
                var r = cmd.ExecuteReader();

                if (r.Read())
                {
                    counter = int.Parse(r["antal"].ToString());
                }

                r.Close();
                cmd.Connection.Close();
                return counter;

            }
        }

        /// <summary>
        /// Sletter en række ud fra et ID i form af et tal
        /// </summary>
        /// <param name="ID">ID'et på den række der skal slettes</param>
        public void Delete(int ID)
        {
            using (var cmd = new SqlCommand("DELETE FROM " + table + " WHERE ID=@ID", Conn.CreateConnection()))
            {
                cmd.Parameters.AddWithValue("ID", ID);
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Sletter en eller flere rækkre ud fra en værdi i en bestemt kolonne
        /// </summary>
        /// <param name="column">Navnet på kolonnen er skal vælges ud fra</param>
        /// <param name="value">Værdien der skal være i kolonnen</param>
        public void DeleteBy(string column, object value)
        {
            using (var cmd = new SqlCommand("DELETE FROM " + table + " WHERE " + column + "=@value", Conn.CreateConnection()))
            {
                cmd.Parameters.AddWithValue("@value", value);
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }

        //-----------------Den original Insert metode-------------
        //public void Insert(T pro)
        //{
        //    string parms = "";
        //    string fielsds = "";

        //    var mappings = mapper.CreateMap();

        //    foreach (var map in mappings)
        //    {
        //        if (map.Key.ToLower() != "id")
        //        {
        //            fielsds += map.Value + ", ";
        //            parms += "@" + map.Key + ", ";
        //        }
        //    }

        //    fielsds = fielsds.Substring(0, fielsds.Length - 2);
        //    parms = parms.Substring(0, parms.Length - 2);

        //    using (var cmd = new SqlCommand("INSERT INTO " + table + " (" + fielsds + ") VALUES(" + parms + ")", Conn.CreateConnection()))
        //    {

        //        foreach (var prop in mappings)
        //        {
        //            if (prop.Key.ToLower() != "id")
        //            {
        //                cmd.Parameters.AddWithValue(prop.Key, pro.GetType().GetProperty(prop.Key).GetValue(pro, null));
        //            }
        //        }

        //        cmd.ExecuteNonQuery();
        //        cmd.Connection.Close();
        //    }
        //}

        /// <summary>
        /// Indsætter en ny række i en tabel ud fra en valgt model
        /// </summary>
        /// <param name="pro">Modellen med det data der skal indsættes</param>
        /// <returns>Kan retunere ID'et på den nye række</returns>
        public int Insert(T pro)
        {
            string parms = "";
            string columns = "";

            var mappings = mapper.CreateMap();

            foreach (var map in mappings)
            {
                if (map.Key.ToLower() != "id" && pro.GetType().GetProperty(map.Key).GetValue(pro, null) != null)
                {
                    columns += map.Value + ", ";
                    parms += "@" + map.Key + ", ";
                }
            }

            columns = columns.Substring(0, columns.Length - 2);
            parms = parms.Substring(0, parms.Length - 2);

            using (var cmd = new SqlCommand("INSERT INTO " + table + " (" + columns + ") VALUES(" + parms + ");SELECT SCOPE_IDENTITY() as curID;", Conn.CreateConnection()))
            {

                foreach (var prop in mappings)
                {
                    if (prop.Key.ToLower() != "id" && pro.GetType().GetProperty(prop.Key).GetValue(pro, null) != null)
                    {
                        cmd.Parameters.AddWithValue(prop.Key, pro.GetType().GetProperty(prop.Key).GetValue(pro, null));
                    }
                }

                var r = cmd.ExecuteReader();
                int curID = 0;
                if (r.Read())
                {
                    curID = int.Parse(r["curID"].ToString());
                }

                r.Close();
                cmd.Connection.Close();
                return curID;
            }
        }

        /// <summary>
        /// Bruges til at opdatere en eksisterende række. Enkelte felter kan undlades ved at sætte værdien til null i modellen
        /// </summary>
        /// <param name="pro">Modellen med det data der skal opdateres i tabellen</param>
        public void Update(T pro)
        {
            string fAndP = "";
            var mappings = mapper.CreateMap();

            foreach (var map in mappings)
            {
                if (map.Key.ToLower() != "id")
                {
                    if (pro.GetType().GetProperty(map.Key).GetValue(pro, null) != null)
                    {
                        fAndP += map.Value + "=@" + map.Key + ", ";
                    }
                }
            }

            fAndP = fAndP.Substring(0, fAndP.Length - 2);

            using (var cmd = new SqlCommand("UPDATE " + table + " SET " + fAndP + " WHERE ID=@Id", Conn.CreateConnection()))
            {

                foreach (var prop in mappings)
                {

                    if (pro.GetType().GetProperty(prop.Key).GetValue(pro, null) != null)
                    {
                        cmd.Parameters.AddWithValue(prop.Key, pro.GetType().GetProperty(prop.Key).GetValue(pro, null));
                    }
                }

                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Kan opdatere værdien i en enkelt kolonne ud fra rækkens ID
        /// </summary>
        /// <param name="column">Navnet på kolonnen er skal vælges ud fra</param>
        /// <param name="value">Værdien der skal være i kolonnen</param>
        /// <param name="ID">ID'et på rækken der skal opdateres</param>
        public void UpdateColumn(string column, object value, int ID)
        {
            using (var cmd = new SqlCommand("UPDATE " + table + " SET " + column + "=@value WHERE ID=@ID", Conn.CreateConnection()))
            {
                cmd.Parameters.AddWithValue("@ID", ID);
                cmd.Parameters.AddWithValue("@value", value);

                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }





        // -------------------- Svend Roperos Nielsen's Tilføjelser ------------------------------





        public T GetTopXDesc()
        {
            using (var cmd = new SqlCommand("SELECT TOP 1 * FROM " + table + " ORDER BY ID DESC", Conn.CreateConnection()))
            {

                var r = cmd.ExecuteReader();
                T type = new T();

                if (r.Read())
                {
                    type = mapper.Map(r);
                }

                r.Close();
                cmd.Connection.Close();
                return type;
            }
        }

        public List<T> GetTopXDesc(int x)
        {
            using (var cmd = new SqlCommand("SELECT TOP " + x + " * FROM " + table + " ORDER BY ID DESC", Conn.CreateConnection()))
            {
                List<T> list = mapper.MapList(cmd.ExecuteReader());
                cmd.Connection.Close();
                return list;
            }
        }

        public T GetXRnd()
        {
            using (var cmd = new SqlCommand("SELECT TOP 1 * FROM " + table + " ORDER BY NEWID()", Conn.CreateConnection()))
            {
         
                var r = cmd.ExecuteReader();
                T type = new T();

                if (r.Read())
                {
                    type = mapper.Map(r);
                }

                r.Close();
                cmd.Connection.Close();
                return type;
            }
        }

        public List<T> GetXRnd(int x)
        {
            using (var cmd = new SqlCommand("SELECT TOP " + x + " * FROM " + table + " ORDER BY NEWID()", Conn.CreateConnection()))
            {
                List<T> list = mapper.MapList(cmd.ExecuteReader());
                cmd.Connection.Close();
                return list;
            }
        }

        public T SqlQuery(string sqlQuery)
        {
            using (var cmd = new SqlCommand(sqlQuery, Conn.CreateConnection()))
            {
                var r = cmd.ExecuteReader();
                T type = new T();

                if (r.Read())
                {
                    type = mapper.Map(r);
                }

                r.Close();
                cmd.Connection.Close();
                return type;
            }
        }

        public List<T> SqlQueryAll(string sql)
        {
            using (var cmd = new SqlCommand(sql, Conn.CreateConnection()))
            {
                List<T> list = mapper.MapList(cmd.ExecuteReader());
                cmd.Connection.Close();
                return list;
            }
        }


    }

    // -------------------- Dynamic models ------------------------------

    public class JoinListModel<T1, T2> where T1 : new() where T2 : new()
    {
        public T1 Type1 { get; set; }
        public List<T2> Type2 { get; set; }
    }

    public class JoinModel<T1, T2> where T1 : new() where T2 : new()
    {
        public T1 Type1 { get; set; }
        public T2 Type2 { get; set; }
    }
}

