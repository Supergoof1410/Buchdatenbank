using System;

namespace Buchdatenbank
{
    public class Books
    {
        public string Boxnumber { get; set; }
        public string Isbn10 { get; set; }
        public string Isbn13 { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public string? Published { get; set; }
        public string? Price { get; set; }
        public bool Buchmaxe_source { get; set; }
        public bool Another_source { get; set; }
        public int? Category { get; set; }
        public string? CategoryName { get; set; }
        public int? Status { get; set; }
        public string? StatusName { get; set; }
        public int Cover { get; set; }
        public string? CoverName { get; set; }
        public string ListedAt { get; set; }

        public string User { get; set; }

        //public int Count { get; set; }
        public Books()
        {
            Isbn10 = "";
            Isbn13 = "";
            Title = "";
            Author = "unbekannt";
            Publisher = "unbekannt";
            Published = "1978";
            Cover = 0;
            Price = "1,00";
            Buchmaxe_source = false;
            Another_source = true;
            Category = 0;
            Status = 0;
            Boxnumber = "";
            CoverName = "";
            StatusName = "";
            CategoryName = "";
            ListedAt = DateTime.Now.ToString("G");
            User = Environment.UserName;
            //Count = 0;
        }
    }
}