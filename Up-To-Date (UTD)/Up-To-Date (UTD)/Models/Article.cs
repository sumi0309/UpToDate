using System.ComponentModel.DataAnnotations;

namespace Up_To_Date__UTD_.Models
{
    public class Article
    {
        public int Id { get; set; }
        
        public string Title { get; set; }

       
        public string Content { get; set; }

        public string FilePath { get; set; }

    }
}
