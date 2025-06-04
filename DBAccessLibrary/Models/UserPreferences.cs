using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccessLibrary.Models
{
    public class UserPreferences
    {
        public string LastReadPassage { get; set; }
        public int LastPracticedVerseId { get; set; }
        public int AppTheme { get; set; }
        public int ShowVersesSaved { get; set; }
        public int ShowPopularHighlights { get; set; }
        public int AllowPushNotifications { get; set; }
    }
}
