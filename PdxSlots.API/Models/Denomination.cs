using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class Denomination
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int GameMathId { get; set; }
    }
}
