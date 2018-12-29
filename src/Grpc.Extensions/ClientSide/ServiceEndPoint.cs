namespace Grpc.Extensions.ClientSide
{
    public class ServiceEndPoint
    {
        public string Address { get; set; }

        public int Port { get; set; }

        public override string ToString()
        {
            if (Port > 0)
            {
                return $"{Address}:{Port}";
            }
            else
            {
                return Address;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as ServiceEndPoint;
            if (other == null)
                return false;

            return Equals(other);
        }

        public bool Equals(ServiceEndPoint other)
        {
            return Port == other.Port && Address == other.Address;
        }

        public override int GetHashCode()
        {
            var hash = Address.GetHashCode();
            if (Port != 0)
                hash ^= Port.GetHashCode();

            return hash;
        }
    }
}