namespace SrcGenProtoLib
{
    public abstract class Monitor
    {
        public string Name { get; set; }

        public string RoutingId { get; set; }

        public abstract void Speak();
    }
}