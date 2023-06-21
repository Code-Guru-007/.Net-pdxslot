using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class IgcuserGaf
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int OperatorId { get; set; }
        public string Gaf { get; set; }
        public DateTime Created { get; set; }

        public virtual Operator Operator { get; set; }
    }
}
