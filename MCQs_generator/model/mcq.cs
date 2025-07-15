using System.ComponentModel.DataAnnotations;

namespace MCQs_generator.model
{
    public class mcq
    {
        [Key]
        public int Id { get; set; }
        public string Question { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; }

        public string topic { get; set; }
    }
}
