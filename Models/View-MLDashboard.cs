namespace EngageTrack.Models
{
    public class View_MLDashboard
    {
        public int TotalGroups { get; set; }
        public int GroupsIsActive { get; set; }
        public int GroupsIsInactive { get; set; }
        public int GroupsWithCRFs { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();
    }
}
