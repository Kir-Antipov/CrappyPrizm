using System;
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

        public byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv)
        {
            JSEngine.SetValue("data", data);
            JSEngine.SetValue("key", key);
            JSEngine.SetValue("iv", iv);

            JSEngine.Execute("data = byteArrayToWordArray(data);");
            JSEngine.Execute("key = CryptoJS.SHA256(byteArrayToWordArray(key));");
            JSEngine.Execute("iv = byteArrayToWordArray(iv);");

            JSEngine.Execute("var encrypted = CryptoJS.AES.encrypt(data, key, { iv: iv });");
            JSEngine.Execute("var result = wordArrayToByteArray(encrypted.iv).concat(wordArrayToByteArray(encrypted.ciphertext))");

            return JSEngine.GetValue("result").AsArray().ToByteArray();
        }

        public byte[] AesDecrypt(byte[] data, byte[] key)
        {
            JSEngine.SetValue("data", data);
            JSEngine.SetValue("key", key);

            JSEngine.Execute("var iv = byteArrayToWordArray(data.slice(0, 16));");
            JSEngine.Execute("data = byteArrayToWordArray(data.slice(16));");
            JSEngine.Execute("key = CryptoJS.SHA256(byteArrayToWordArray(key));");
            JSEngine.Execute("var encrypted = CryptoJS.lib.CipherParams.create({ ciphertext: data, iv: iv, key: key });");
            JSEngine.Execute("var decrypted = wordArrayToByteArray(CryptoJS.AES.decrypt(encrypted, key, { iv: iv }));");

            return JSEngine.GetValue("decrypted").AsArray().ToByteArray();
        }

        public byte[] Deflate(byte[] bytes)
        {
            JSEngine.SetValue("bytes", bytes);
            return JSEngine.GetValueByPath("pako", "gzip").Invoke(JSEngine.GetValue("bytes")).AsArray().ToByteArray();
        }

        public byte[] Inflate(byte[] bytes)
        {
            JSEngine.SetValue("bytes", bytes);
            return JSEngine.GetValueByPath("pako", "inflate").Invoke(JSEngine.GetValue("bytes")).AsArray().ToByteArray();
        }

        public void Dispose() => EnginePool.Release(this);
        #endregion
    }
}
