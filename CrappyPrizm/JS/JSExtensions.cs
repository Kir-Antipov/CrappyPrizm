using Jint.Native;
using Jint.Native.Array;
using JSEngine = Jint.Engine;

namespace CrappyPrizm.JS
{
    internal static class JSExtensions
    {
        public static byte[] ToByteArray(this ArrayInstance arrayInstance)
        {
            int length = (int)arrayInstance.GetLength();
            byte[] result = new byte[length];
            for (int i = 0; i < length; ++i)
                result[i] = (byte)arrayInstance.Get(i.ToString()).AsNumber();
            return result;
        }

        public static JsValue GetValueByPath(this JSEngine engine, params string[] path)
        {
            JsValue value = engine.GetValue(path[0]);
            for (int i = 1; i < path.Length; ++i)
                value = engine.GetValue(value, path[i]);
            return value;
        }

        public static JsValue CreateValue(this JSEngine engine, object value) => JsValue.FromObject(engine, value);
    }
}
