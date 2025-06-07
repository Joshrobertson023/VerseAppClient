﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DBAccessLibrary.Models
{
    public class RecoveryInfo
    {
        private string username;
        private string firstName;
        private string lastName;
        private string? email;
        private string passwordHash;
        public int UserId { get; set; }
        public string Token { get; set; }
        public int Id { get; set; }
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
            }
        }
        public static int UsernameMax { get { return 18; } }
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value.ToLower();
            }
        }
        public static int NameMax { get { return 15; } }
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value.ToLower();
            }
        }
        [JsonIgnore]
        public string FullName
        {
            get
            {
                return (firstName.Substring(0, 1).ToUpper() + firstName.Substring(1) + " "
                        + lastName.Substring(0, 1).ToUpper() + lastName.Substring(1));
            }
        }
        public string? Email
        {
            get
            {
                    return email;
            }
            set
            {
                email = value;
            }
        }
        public static int EmailMax { get { return 128; } }
        public string PasswordHash
        {
            get { return passwordHash; }
            set
            {
                passwordHash = value;
            }
        }
        public static int PasswordMax { get { return 128; } }
    }
}
