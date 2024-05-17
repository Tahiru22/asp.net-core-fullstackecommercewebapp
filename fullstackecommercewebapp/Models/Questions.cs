namespace fullstackecommercewebapp.Models
{

    public partial class Questions : BaseEntity
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int UserId { get; set; }
        public int? ManagerId { get; set; }

        public virtual User Manager { get; set; }
        public virtual User User { get; set; }
    }
}
