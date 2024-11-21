using System.ComponentModel.DataAnnotations;

namespace CommandService.DTO
{
    public class CommandCreateDTO
    {
        [Required]
        public string HowTo { get; set; }
        [Required]
        public string CommandLine { get; set; }
    }
}