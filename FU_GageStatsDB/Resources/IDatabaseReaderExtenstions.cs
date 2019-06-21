using System;
using System.Collections.Generic;
using System.Text;

namespace FU_GageStatsDB.Resources
{
    public static class dbExtenstions
    {
        public static T GetDataType<T>(this System.Data.IDataReader r, string name, object defaultIfNull = null)
        {
            try
            {                
                var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                var col = r.GetOrdinal(name);
                return r.IsDBNull(col)||String.IsNullOrEmpty(r[name].ToString()) ? (T)defaultIfNull : (T)Convert.ChangeType(r[name], t);
            }
            catch (Exception ex)
            {
                return (T)defaultIfNull;
            }
        }
    }
}
