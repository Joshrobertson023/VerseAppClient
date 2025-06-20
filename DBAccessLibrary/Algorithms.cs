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

        public static List<Collection> Sort(List<Collection> _collections, int sortBy)
        {
            List<Collection> collections = new List<Collection>();

            if (sortBy == 0)
            {
                collections.Sort((x, y) => DateTime.Compare(y.DateCreated, x.DateCreated));
            }
            else if (sortBy == 1)
            {
                collections.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase));
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
                return _collections;
            }

            return collections;
        }

        public static (string, List<Collection>) SortCustom(List<Collection> collections, int sortBy)
        {
            List<Collection> sortedCollections = new List<Collection>();
            string newSort = string.Empty;
            
            if (sortBy == 4)
            {
                foreach (var collection in collections)
                {
                    if (collection.Pinned == 1)
                        sortedCollections.Add(collection);
                }

                foreach (var collection in collections)
                {
                    if (!sortedCollections.Contains(collection) && collection.Pinned == 0)
                        sortedCollections.Add(collection);
                }

                foreach (var collection in sortedCollections)
                {
                    newSort += $"{collection.Id},";
                }

                collections = sortedCollections;
            }

            //foreach (var order in newSort.Split(',').Where(o => !string.IsNullOrEmpty(o)))
            //{
            //    Collection col = collections.FirstOrDefault(c => c.Id.ToString() == order);
            //    collections.Add(col);
            //}

            return (newSort, collections);
        }
    }
}
