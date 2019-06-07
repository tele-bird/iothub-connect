using System;
using System.Collections.Generic;
using System.Text;

namespace IotPoc.Common
{
    public class MobileState
    {
        public string Id { get; set; }

        public override string ToString()
        {
            return $"{Id}";
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is MobileState)
            {
                MobileState other = (MobileState)obj;
                result = Id.Equals(other.Id);
            }
            return result;
        }
    }
}
