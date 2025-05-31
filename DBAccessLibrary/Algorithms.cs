﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DBAccessLibrary.Models;
using System.Globalization;

namespace DBAccessLibrary
{
    public static class Algorithms
    {
        public static List<T> SortByName<T>(List<T> userInfo)
        {
            if (typeof(T) == typeof(List<string>))
            {
                List<T> sortedUserInfo = new List<T>();
                return sortedUserInfo;
            }
            else if (typeof(T) == typeof(List<RecoveryInfo>))
            {
                List<T> sortedUserInfo = new List<T>();
                return sortedUserInfo;
            }
            else
            {
                throw new NotImplementedException("The type you are trying to sort has not been implemented.");
            }
        }
    }
}
