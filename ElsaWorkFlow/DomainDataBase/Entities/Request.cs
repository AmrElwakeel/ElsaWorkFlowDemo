using System.ComponentModel.DataAnnotations;

namespace ElsaWorkFlow.DomainDataBase.Entities
{
    public class Request
    {
        [Key]
        public string Id { get; set; }
        public int Status { get; set; }
        public string CurrentUserRole { get; set; }
        public string Comment { get; set; }
    }
}
