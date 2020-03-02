namespace CrappyPrizm.JS
{
    internal class Script
    {
        #region Var
        public string Name { get; }
        public bool Initialized { get; }
        #endregion

        #region Init
        public Script(string name, bool initialized)
        {
            Name = name;
            Initialized = initialized;
        }

        public Script(string name) : this(name, false) { }
        #endregion

        #region Functions
        public static implicit operator Script(string name) => new Script(name);

        public Script GetInitialized() => new Script(Name, true);

        public string? GetScript() => Resources.Scripts.ResourceManager.GetString(Name);

        public override string ToString() => Name;
        #endregion
    }
}
