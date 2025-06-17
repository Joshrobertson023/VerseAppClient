﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccessLibrary.Models
{
    public class UserModel
    {
        public List<Collection> Collections { get; set; } = new List<Collection>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public List<Notification> NotificationsUnseen { get; set; } = new List<Notification>();
        public string CollectionsSort { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PasswordHash { get; set; }

        public string AuthToken { get; set; }
        public string Status { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime LastSeen { get; set; }
        public string? Description { get; set; }
        public int? CurrentReadingPlan { get; set; }
        public int IsDeleted { get; set; }
        public string? ReasonDeleted { get; set; }
        public int Flagged { get; set; }
        public int FollowVerseOfTheDay { get; set; }
        public int Visibility { get; set; }

        public UserModel(string username, string firstName, string lastName, string? email, string hashedPassword, string token, string status = "DEFAULT")
        {
            Username = username.Trim();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            if (!string.IsNullOrEmpty(email))
            {
                Email = email.Trim();
            }
            PasswordHash = hashedPassword;
            AuthToken = token;
            Status = status;
            CollectionsSort = "none";
        }

        public UserModel() { }

        public string FullName
        {
            get 
            {
                return (FirstName.Substring(0, 1).ToUpper() + FirstName.Substring(1) + " " 
                        + LastName.Substring(0, 1).ToUpper() + LastName.Substring(1));
            }
        }
    }
}
