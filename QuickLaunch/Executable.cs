namespace QuickLaunch
{
    public class Executable
    {
        public string Title { get; private set; }
        public string Path { get; private set; }

        public Executable(string title, string path)
        {
            this.Title = title;
            this.Path = path;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Executable)) return false;
            Executable other = (Executable)obj;
            return this.Path == other.Path;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public string SaveString()
        {
            return Title + ";" + Path;
        }

        public static Executable FromSaveString(string saveString)
        {
            string title = saveString.Split(';')[0];
            string path = saveString.Split(';')[1];
            return new Executable(title, path);
        }
    }
}
