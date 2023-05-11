using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace LicensePlateDataModels
{
    public class LicensePlate
    {
        [Required, Range(0, int.MaxValue)]
        public int Id { get; set; }
        public bool IsProcessed { get; set; } = false;
        [Required, StringLength(255)]
        public string FileName { get; set; }
        [Required, StringLength(20)]
        public string LicensePlateText { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
