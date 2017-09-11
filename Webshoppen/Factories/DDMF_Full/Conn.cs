using System;
using System.Data.SqlClient;

namespace Webshoppen
{
    public static class Conn
    {
        /// <summary>
        /// Metoden indeholder formindelsesindstillinger til databaen
        /// </summary>
        /// <returns>Retunere en MS SQL connection string</returns>
        public static SqlConnection GetCon()
        {
            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-PJV0RDF\SQLEXPRESS;Initial Catalog=Webshoppen;Integrated Security=True");
            //SqlConnection con = new SqlConnection(@"Data Source=sql.itcn.dk\VID;Initial Catalog=ddje.VID;Persist Security Info=True;User ID=ddje.VID;Password=8V3Y58Muex");
            return con;
        }

        /// <summary>
        /// Metoden retunere en åben forbindelse til databasen der er defineret i GetCon()
        /// </summary>
        /// <returns>Retunere en åben forbindelse til databasen</returns>
        public static SqlConnection CreateConnection()
        {
            var cn = GetCon();
            cn.Open();
            return cn;
        }
		/// <summary>
        /// Bruges til at tjekke om der er forbindelse til databasen
        /// C#: var t = RepoAM.Conn.Check();
        /// Razor: @RepoAM.Conn.Check()
        /// </summary>
        /// <returns>Retunere true eller false</returns>
        public static bool Check()
        {
            bool t = true;
            var cn = GetCon();

            try
            {
                cn.Open();
            }
            catch (Exception)
            {
                t = false;
            }
           
            return t;
        }
    }
}
