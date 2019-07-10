using System;
using System.Collections.Generic;
using System.Globalization;
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
                var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                var col = r.GetOrdinal(name);
                if (r.IsDBNull(col) || String.IsNullOrEmpty(r[name].ToString())) return (T)defaultIfNull;
                object value = defaultIfNull;
                switch (Type.GetTypeCode(t))
                {
                    case TypeCode.String:
                        value = Convert.ToString(r[name]);
                        break;
                    case TypeCode.Int32:
                        value = Convert.ToInt32(r[name]);
                        break;
                    case TypeCode.Double:
                        value = Convert.ToDouble(r[name]);
                        break;
                    case TypeCode.DateTime:
                        value = Convert.ToDateTime(r[name]);
                        break;
                    case TypeCode.Boolean:
                        switch (r[name].ToString().Trim())
                        {
                            case "False":
                            case "false":
                            case "0":
                            case "off":
                            case "":
                                value = false;
                                break;
                            case "True":
                            case "true":
                            case "1":
                            case "on":
                                value = true;
                                break;
                            default:
                                value = false;
                                break;
                        }
                        break;
                    default:
                        value = (T)Convert.ChangeType(r[name], t);
                        break;
                }

                return (T)value;
            }
        }
    }
}
