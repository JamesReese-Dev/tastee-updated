using System;
using System.Collections.Generic;
using System.Text;

namespace E2.Tastee.Common
{
    public class SimpleDto: ISimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
