using System.ComponentModel.DataAnnotations.Schema;

namespace Swift_Move.Models
{
    public class ServiceStaff
    {
        //To assign staff to a job
        public int ServiceModelId { get; set; }
        public ServiceModel ServiceModel { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; }
    }
}
