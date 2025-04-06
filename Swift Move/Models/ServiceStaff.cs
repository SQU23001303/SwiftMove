using System.ComponentModel.DataAnnotations.Schema;

namespace Swift_Move.Models
{
    public class ServiceStaff
    {
        public int ServiceModelId { get; set; }
        public ServiceModel ServiceModel { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; }
    }
}
