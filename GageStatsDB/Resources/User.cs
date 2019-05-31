using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WIM.Resources;

namespace GageStatsDB.Resources
{
    public partial class User:IUser
    {
        [Required][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string Username { get; set; }
        [Required][EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public String Role { get; set; }
        [Phone]
        public string PrimaryPhone { get; set; }
        [Required]
        public string Password { get; set; }
        public bool ShouldSerializePassword()
        { return false; }
        [Required]
        public string Salt { get; set; }
        public bool ShouldSerializeSalt()
        { return false; }
    }

    public class Role
    {
        public const string Admin = "Administrator";
        public const string Manager = "Manager";
        public const string Anonymous = "Anonymous";
    }
}
