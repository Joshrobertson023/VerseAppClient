﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccessLibrary.Models
{
    public class Verse
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Text { get; set; }
        public int UsersSaved { get; set; }
        public int UsersMemorized { get; set; }
        public int InYourLists { get; set; }
        public int YouMemorized { get; set; }
    }
}
