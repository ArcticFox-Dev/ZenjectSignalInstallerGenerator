namespace ArcticFox.Zenject.SignalsGenerator
{
    public class Signal
    {
        public string FullQualifiedName { get; private set; }
        public string ShortName { get; private set; }

        public Signal(string fullQualifiedName, string shortName)
        {
            FullQualifiedName = fullQualifiedName;
            ShortName = shortName;
        }
    }
}