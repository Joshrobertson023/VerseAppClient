using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DBAccessLibrary.Models;

namespace DBAccessLibrary
{
    public class Data
    {
        public List<string> usernames;
        public UserModel currentUser;

        public Data()
        {
            usernames = new();
            currentUser = new();
        }
    }
}
