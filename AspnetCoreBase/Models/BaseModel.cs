using System;

namespace AspnetCoreBase.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}