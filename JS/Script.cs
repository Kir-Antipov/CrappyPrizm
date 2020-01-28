namespace CrappyPrizm.JS
{
    public class Script
    {
        #region Var
        public string Name { get; }
        public string Path { get; }
        public bool Initialized { get; }
        #endregion

        #region Init
        public Script(string name, string path, bool initialized)
        {
            Name = name;
            Path = path;
            Initialized = initialized;
        }

        public Script(string name, string path) : this(name, path, false) { }

        public Script(string name) : this(name, GetPathFromName(name), false) { }

        public Script(string name, bool initialized) : this(name, GetPathFromName(name), initialized) { }
        #endregion

        #region Functions
        public static implicit operator Script(string name) => new Script(name);

        public Script GetInitialized() => new Script(Name, Path, true);

        public override string ToString() => Name;

        private static string GetPathFromName(string name) => $"scripts/{char.ToLower(name[0])}{name.Substring(1)}.min.js";
        #endregion
    }
}
