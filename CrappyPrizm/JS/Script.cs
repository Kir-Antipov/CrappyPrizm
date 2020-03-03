using System.Linq;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

namespace CrappyPrizm.JS
{
    internal class Script
    {
        #region Var
        public string Name { get; }

        public static IEnumerable<Script> All
        {
            get
            {
                ResourceManager resources = new ResourceManager(typeof(Resources.Scripts));
                ResourceSet resourceSet = resources.GetResourceSet(CultureInfo.InvariantCulture, true, true);
                return resourceSet.Cast<DictionaryEntry>().Select(x => new Script(x.Key.ToString()));
            }
        }
        #endregion

        #region Init
        public Script(string name) => Name = name;
        #endregion

        #region Functions
        public static implicit operator Script(string name) => new Script(name);

        public string? GetScript() => Resources.Scripts.ResourceManager.GetString(Name);

        public override string ToString() => Name;
        #endregion
    }
}
