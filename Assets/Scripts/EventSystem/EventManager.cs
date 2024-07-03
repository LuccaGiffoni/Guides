namespace EventSystem
{
    public static class EventManager
    {
        public static readonly AnchorEvents AnchorEvents = new();
        public static readonly DatabaseEvents DatabaseEvents = new();
        public static readonly StepEvents StepEvents = new();
    }
}