namespace Assets.Scripts
{
    public class RocketInput
    {
        public bool Thrust { get; set; }
        public ManeuverDirection Maneuver { get; set; }
        public enum ManeuverDirection { None, Left, Right}

        public RocketInput()
        {
            Maneuver = ManeuverDirection.None;
        }
    }
}
