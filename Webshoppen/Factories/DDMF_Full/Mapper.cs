using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace Webshoppen
{
    /// <summary>
    /// Data mapperen her har til formål at mappe tabeller og modeller sammen ud fra properties i modellen. 
    /// </summary>
    /// <typeparam name="T">Modellen der skal mappes ud fra</typeparam>
   public class Mapper<T> where T : new()
    {
        private Dictionary<string, string> _mappings { get; set; }

        public Mapper()
        {
            _mappings = CreateMap();
        }

        /// <summary>
        /// Mapper en enkelt række i en model
        /// </summary>
        /// <param name="record">Data fra den række der skal mappes, typisk fra CMD.ExecuteReader()</param>
        /// <returns>Retunere en model med data</returns>
        public T Map(IDataRecord record)
        {
            var item = Activator.CreateInstance<T>();
            var itemType = item.GetType();


            
            foreach (var map in _mappings)
            {
                var prop = itemType.GetProperty(map.Key);

                if (record[map.Value] != DBNull.Value)
                {
                    prop.SetValue(item, record[map.Value], null);
                }
                
            }

            return item;
        }

        /// <summary>
        /// Mapper flere rækker ud i en liste af en bestemt model
        /// </summary>
        /// <param name="reader">Data fra de række der skal mappes, typisk fra CMD.ExecuteReader()</param>
        /// <returns></returns>
        public List<T> MapList(IDataReader reader)
        {
            var list = new List<T>();

            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            reader.Close();
            return list;
        }

        /// <summary>
        /// Metoden her bliver brugt til at lave et map ud fra en model. Mappet bliver brugt til at bestemme hvilke kolonner og properties der skal mappes sammen
        /// </summary>
        /// <returns>Retunere et map med kolonner og properties</returns>
        public Dictionary<string, string> CreateMap()
        {
            var mappings = new Dictionary<string, string>();
            var props = typeof(T).GetProperties().Where(p => p.CanWrite);


            foreach (var prop in props)
            {
                mappings.Add(prop.Name, prop.Name);
            }

            return mappings;
        }

    }
}


