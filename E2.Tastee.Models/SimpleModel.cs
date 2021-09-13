using E2.Tastee.Common;

namespace E2.Tastee.Models
{
    public class SimpleModel : ModelBase, ISimpleDto
    {
        public virtual string Name { get; set; }
    }
}
