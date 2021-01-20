using System;

namespace QSearcher_.Data
{
    public class Category
    {
        public string Title;
        public string Slug;
        public bool Selected;

        public Category(string title, string slug)
        {
            Title = title;
            Slug = slug;
        }

        public override string ToString()
        {
            return Selected ? $"[{Title}]" : Title;
        }
    }
}
