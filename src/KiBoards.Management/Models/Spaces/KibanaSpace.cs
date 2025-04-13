namespace KiBoards.Management.Models.Spaces;

public class KibanaSpace
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public List<string> DisabledFeatures { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
}
