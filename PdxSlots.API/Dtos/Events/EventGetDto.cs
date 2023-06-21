using PdxSlots.API.Dtos.Devices;
using PdxSlots.API.Dtos.GameMaths;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.Operators;
using PdxSlots.API.Dtos.Rounds;
using PdxSlots.API.Dtos.Users;

namespace PdxSlots.API.Dtos.Events
{
    public class EventGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }

        public DeviceGetDto Device { get; set; }
        public GameGetDto Game { get; set; }
        public GameMathsGetDto GameMath { get; set; }
        public OperatorGetDto Operator { get; set; }
        public RoundGetDto Round { get; set; }
        public UserGetDto User { get; set; }
    }
}
