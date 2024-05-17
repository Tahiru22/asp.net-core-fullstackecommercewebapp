using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.ViewModels
{
    public class TrackViewModel
    {
        [Required]
        public int Number { get; set; }
        public string Status { get; set; }
        public TrackViewModel()
        {
            Status = "";
        }

    }
}
