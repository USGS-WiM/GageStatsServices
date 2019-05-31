//------------------------------------------------------------------------------
//----- Equality ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2018 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Overrides Equatable
//
//discussion:   https://blogs.msdn.microsoft.com/ericlippert/2011/02/28/guidelines-and-rules-for-gethashcode/    
//              http://www.aaronstannard.com/overriding-equality-in-dotnet/
//
//              var hashCode = 13;
//              hashCode = (hashCode * 397) ^ MyNum;
//              var myStrHashCode = !string.IsNullOrEmpty(MyStr) ?
//                                      MyStr.GetHashCode() : 0;
//              hashCode = (hashCode * 397) ^ MyStr;
//              hashCode = (hashCode * 397) ^ Time.GetHashCode();
// 
using System;

namespace GageStatsDB.Resources
{
    public partial class Agency : IEquatable<Agency>
    {
        public bool Equals(Agency other)
        {
            return String.Equals(this.Name.ToLower(), other.Name.ToLower()) &&
                String.Equals(this.Code.ToLower(), other.Code.ToLower());

        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Agency);
        }
        public override int GetHashCode()
        {
            return (this.Name + this.Code).GetHashCode();
        }
    }
    public partial class Characteristic : IEquatable<Characteristic>
    {
        public bool Equals(Characteristic other)
        {
            return this.StationID == other.StationID &&
                this.VariableTypeID == other.VariableTypeID &&
                this.UnitTypeID == other.UnitTypeID;

        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Characteristic);
        }
        public override int GetHashCode()
        {
            return (this.StationID + this.VariableTypeID + this.UnitTypeID).GetHashCode();
        }
    }
    public partial class Citation : IEquatable<Citation>
    {
        public bool Equals(Citation other)
        {
            return String.Equals(this.Title.ToLower(), other.Title.ToLower()) &&
                String.Equals(this.Author.ToLower(), other.Author.ToLower());

        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Citation);
        }
        public override int GetHashCode()
        {
            return (this.Title + this.Author).GetHashCode();
        }
    }
    public partial class PredictionInterval : IEquatable<PredictionInterval>
    {
        public bool Equals(PredictionInterval other)
        {
            return base.Equals(other);

        }
    }
    public partial class Station : IEquatable<Station>
    {
        public bool Equals(Station other)
        {
            return String.Equals(this.Code.ToLower(), other.Code.ToLower()) &&
                this.AgencyID==other.AgencyID &&
                this.StationTypeID == other.StationTypeID;

        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Station);
        }
        public override int GetHashCode()
        {
            return (this.Code + this.AgencyID + this.StationTypeID).GetHashCode();
        }
    }
    public partial class StationType : IEquatable<StationType>
    {
        public bool Equals(StationType other)
        {
            return String.Equals(this.Code.ToLower(), other.Code.ToLower());

        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as StationType);
        }
        public override int GetHashCode()
        {
            return (this.Code).GetHashCode();
        }
    }
    public partial class Statistic : IEquatable<Statistic>
    {
        public bool Equals(Statistic other)
        {
            return this.StatisticGroupTypeID == other.StatisticGroupTypeID &&
                this.RegressionTypeID == other.RegressionTypeID &&
                this.StationID == other.StationID &&
                this.UnitTypeID == other.UnitTypeID;


        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Statistic);
        }
        public override int GetHashCode()
        {
            return (this.StatisticGroupTypeID + this.RegressionTypeID + this.StationID + this.UnitTypeID).GetHashCode();
        }
    }
    public partial class StatisticError : IEquatable<StatisticError>
    {
        public bool Equals(StatisticError other)
        {
            return this.StatisticID == other.StatisticID &&
                this.ErrorTypeID == other.ErrorTypeID;

        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as StatisticError);
        }
        public override int GetHashCode()
        {
            return (this.StatisticID + this.ErrorTypeID).GetHashCode();
        }
    }
    public partial class User : IEquatable<User>
    {
        public bool Equals(User other)
        {
            return String.Equals(this.Username.ToLower(), other.Username.ToLower()) &&
                String.Equals(this.Email.ToLower(), other.Email.ToLower()) &&
                String.Equals(this.FirstName.ToLower(), other.FirstName.ToLower()) &&
                string.Equals(this.LastName.ToLower(), other.LastName.ToLower());

        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as User);
        }
        public override int GetHashCode()
        {
            return (this.Username +this.Email + this.FirstName + this.LastName).GetHashCode();
        }
    }
}
