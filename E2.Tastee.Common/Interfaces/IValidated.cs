using System.Collections.Generic;

namespace E2.Tastee.Common
{

    public interface IValidated<T>
    {
        public T Result { get; set; }
        IList<string> WarningMessages { get; set; }
        IList<string> ErrorMessages { get; set; }
        public bool HasErrorMessages { get; }
        public bool HasWarningMessages { get; }
    }

}