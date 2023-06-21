namespace PdxSlots.API.Common.Firebase
{
    public class FirebaseUser
    {
        public string Uid { get; set; }
        public Dictionary<string, object> CustomClaims { get; set; }
    }
}
