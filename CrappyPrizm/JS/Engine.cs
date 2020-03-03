using System;
using Jint.Native;
using JSEngine = Jint.Engine;
using System.Collections.Generic;

namespace CrappyPrizm.JS
{
    internal class Engine : IDisposable
    {
        #region Constants
        private const string Pako = nameof(Pako);
        private const string CryptoJS = nameof(CryptoJS);
        private const string WordArrayToByteArray = nameof(WordArrayToByteArray);
        private const string ByteArrayToWordArray = nameof(ByteArrayToWordArray);
        #endregion

        #region Var
        private readonly Dictionary<string, Script> Scripts = new Dictionary<string, Script>
        {
            { Pako, Pako },
            { CryptoJS, CryptoJS },
            { nameof(AesEncrypt), nameof(AesEncrypt) },
            { nameof(AesDecrypt), nameof(AesDecrypt) },
            { WordArrayToByteArray, WordArrayToByteArray },
            { ByteArrayToWordArray, ByteArrayToWordArray }
        };

        private readonly JSEngine JSEngine = new JSEngine();
        #endregion

        #region Init
        public Engine()
        {
            Initialize(Pako);
            Initialize(CryptoJS);
            Initialize(nameof(AesEncrypt));
            Initialize(nameof(AesDecrypt));
            Initialize(ByteArrayToWordArray);
            Initialize(WordArrayToByteArray);
        }
        #endregion

        #region Functions
        private bool Initialize(string name)
        {
            if (Scripts.TryGetValue(name, out Script? script))
            {
                if (script is { Initialized: false })
                {
                    JSEngine.Execute(script.GetScript() ?? string.Empty);
                    Scripts[name] = script.GetInitialized();
                }
                return true;
            }
            return false;
        }

        public byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv) => JSEngine.GetValue("aesEncrypt").Invoke(JsValue.FromObject(JSEngine, data), JsValue.FromObject(JSEngine, key), JsValue.FromObject(JSEngine, iv)).AsArray().ToByteArray();

        public byte[] AesDecrypt(byte[] data, byte[] key) => JSEngine.GetValue("aesDecrypt").Invoke(JsValue.FromObject(JSEngine, data), JsValue.FromObject(JSEngine, key)).AsArray().ToByteArray();

        public byte[] Deflate(byte[] bytes) => JSEngine.GetValueByPath("pako", "gzip").Invoke(JsValue.FromObject(JSEngine, bytes)).AsArray().ToByteArray();

        public byte[] Inflate(byte[] bytes) => JSEngine.GetValueByPath("pako", "inflate").Invoke(JsValue.FromObject(JSEngine, bytes)).AsArray().ToByteArray();

        public void Dispose() => EnginePool.Release(this);
        #endregion
    }
}
