namespace E2.Tastee.Common
{
    public interface ITrackedSimpleDto: ISimpleDto
    {
        int CreatedByUserId { get; set; }
    }
}
