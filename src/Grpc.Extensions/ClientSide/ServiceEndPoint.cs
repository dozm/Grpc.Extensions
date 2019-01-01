namespace Grpc.Extensions.ClientSide
{
    public class ServiceEndPoint
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public override string ToString()
        {
            if (Port > 0)
            {
                return $"{Host}:{Port}";
            }
            else
            {
                return Host;
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
            return Port == other.Port && Host == other.Host;
        }

        public override int GetHashCode()
        {
            var hash = Host.GetHashCode();
            if (Port != 0)
                hash ^= Port.GetHashCode();

            return hash;
        }
    }
}