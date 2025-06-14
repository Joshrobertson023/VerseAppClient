﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccessLibrary.Models
{
    public class UserVerse
    {
        public int VerseId { get; set; }
        public int UserId { get; set; }
        public int CollectionId { get; set; }
        public string Reference { get; set; }
        public DateTime LastPracticed { get; set; }
        public DateTime DateMemorized { get; set; }
        public DateTime DateAdded { get; set; }
        public float ProgressPercent { get; set; }
        public int TimesReviewed { get; set; }
        public int TimesMemorized { get; set; }
        public int Visibility { get; set; }
        public List<Verse> Verses { get; set; } = new List<Verse>();
        public bool Expanded { get; set; } = true;
    }
}
