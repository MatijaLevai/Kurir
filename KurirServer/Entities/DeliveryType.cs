using System.ComponentModel.DataAnnotations;

namespace KurirServer.Entities
{
    public class DeliveryType
    {
        [Required,Key]
        public int DeliveryTypeID { get; set; }
        [Required]
        public string DeliveryTypeName { get; set; }
    }
}