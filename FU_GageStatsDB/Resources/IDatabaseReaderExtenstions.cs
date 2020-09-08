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
                        value = Convert.ToString(r[name].ToString().Trim());
                        break;
                    case TypeCode.Int32:
                        value = Convert.ToInt32(r[name].ToString().Trim());
                        break;
                    case TypeCode.Double:
                        try
                        {
                            value = Convert.ToDouble(r[name].ToString().Replace(",", "")
                                                                   .Replace("(33)", "")
                                                                   .Replace("(30)", "")
                                                                   .Replace("(121)", "")
                                                                   .Replace("(123)", "")
                                                                   .Replace("(55)", "")
                                                                   .Replace("(54)", "").Trim());
                        }
                        catch (Exception)
                        {
                            value = null;
                        }
                        
                        break;
                    case TypeCode.DateTime:
                        value = Convert.ToDateTime(r[name].ToString().Trim());
                        break;
                    case TypeCode.Boolean:
                        switch (r[name].ToString().Trim())
                        {
                            case "False":
                            case "false":
                            case "0":
                            case "off":
                            case "N":
                            case "":
                            case "No":
                            case "no":
                            case "Undefined":
                                value = false;
                                break;
                            case "True":
                            case "true":
                            case "1":
                            case "Y":
                            case "on":
                            case "Yes":
                                value = true;
                                break;
                            default:
                                value = false;
                                break;
                        }
                        break;
                    default:
                        value = (T)Convert.ChangeType(r[name].ToString().Trim(), t);
                        break;
                }

                return (T)value;
            }
        }
    }
}
