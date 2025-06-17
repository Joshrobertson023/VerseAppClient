using System;
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

        public static void SortCollections(List<Collection> collections, int sortBy, string customSort)
        {
            if (sortBy == 0)
            {
                collections.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase));
            }
            else if (sortBy == 1)
            {
                collections.Sort((x, y) => DateTime.Compare(x.DateCreated, y.DateCreated));
            }
            else if (sortBy == 2)
            {
                collections.Sort((x, y) => DateTime.Compare(x.LastPracticed, y.LastPracticed));
            }
            else if (sortBy == 3)
            {
                collections.Sort((x, y) => y.ProgressPercent.CompareTo(x.ProgressPercent));
            }
            else if (sortBy == 4)
            {
                List<Collection> sortedCollections = new List<Collection>();

                foreach (var sort in customSort.Split(','))
                {
                    sortedCollections.Add(collections.FirstOrDefault(c => c.Id == Convert.ToInt32(sort.Trim())));
                }
                collections = sortedCollections;
            }
        }
    }
}
