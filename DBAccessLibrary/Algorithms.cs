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

        public static List<Collection> Sort(List<Collection> _collections, int sortBy, string pinned)
        {
            Console.WriteLine("Sort ran.");
            var collections = new List<Collection>();
            var pinnedCollections = new List<Collection>();

            foreach (var pin in pinned.Split(','))
            {
                pinnedCollections.Add(collections.FirstOrDefault(c => c.Id.ToString() == pin));
            }
            foreach (var collection in collections)
            {
                if (!pinnedCollections.Contains(collection) && collection.Pinned == 0)
                    collections.Add(collection);
            }

            switch (sortBy)
            {
                case 0:
                    collections.Sort((x, y) => DateTime.Compare(y.DateCreated, x.DateCreated));
                    break;
                case 1:
                    collections.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase));
                    break;
                case 2:
                    collections.Sort((x, y) => DateTime.Compare(x.LastPracticed, y.LastPracticed));
                    break;
                case 3:
                    collections.Sort((x, y) => y.ProgressPercent.CompareTo(x.ProgressPercent));
                    break;
                default:
                    break;
            }

            pinnedCollections.AddRange(collections);
            collections = pinnedCollections;
            foreach (var col in collections)
            {
                Console.WriteLine(col.Title);
            }
            return collections;
        }


        public static (List<Collection>, string, string) SortCustom(List<Collection> collections, string order, string pinned)
        {
            Console.WriteLine("Custom sort ran.");
            List<Collection> sortedCollections = new List<Collection>();
            List<Collection> pinnedCollections = new List<Collection>();
            string newSort = string.Empty;
            string newPinned = string.Empty;

            if (order.Contains(",0,"))
            {
                order = string.Empty;
                Console.WriteLine("Order contains ,0, so resetting order to empty string.");
            }

            if (string.IsNullOrEmpty(order) || order == "NONE")
            {
                order = string.Empty;
                Console.WriteLine("Order is empty or NONE, so resetting order to empty string.");
                foreach (var collection in collections)
                {
                    if (collection.Pinned == 1)
                    {
                        pinnedCollections.Add(collection);
                        newPinned += $"{collection.Id},";
                    }
                }

                foreach (var collection in collections)
                {
                    if (!sortedCollections.Contains(collection) && collection.Pinned == 0)
                    {
                        sortedCollections.Add(collection);
                        newSort += $"{collection.Id},";
                    }
                }
                pinnedCollections.AddRange(sortedCollections);
                collections = pinnedCollections;
            }
            else
            {
                int count = 0;

                foreach (var pin in pinned.Split(',').Where(p => !string.IsNullOrEmpty(p)))
                {
                    Collection col = collections.FirstOrDefault(c => c.Id.ToString() == pin);
                    pinnedCollections.Add(col);
                    count++;
                }

                foreach (var ord in order.Split(',').Where(o => !string.IsNullOrEmpty(o)))
                {
                    Collection col = collections.FirstOrDefault(c => c.Id.ToString() == ord);
                    sortedCollections.Add(col);
                    count++;
                }
                if (count < collections.Count)
                {
                    foreach (var collection in collections)
                    {
                        if (!sortedCollections.Contains(collection) && !pinnedCollections.Contains(collection))
                        {
                            if (collection.Pinned == 1)
                            {
                                pinnedCollections.Add(collection);
                                newPinned += $"{collection.Id},";
                            }
                            else
                            {
                                sortedCollections.Add(collection);
                                newSort += $"{collection.Id},";
                            }
                        }
                    }
                }
                pinnedCollections.AddRange(sortedCollections);
                collections = pinnedCollections;
            }

            return (collections, newSort, newPinned);
        }
    }
}
