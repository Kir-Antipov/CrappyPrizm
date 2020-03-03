using System;
using Jint.Parser;
using System.Linq;
using JSEngine = Jint.Engine;
using JSProgram = Jint.Parser.Ast.Program;

namespace CrappyPrizm.JS
{
    internal class Engine : IDisposable
    {
        #region Var
        private readonly JSEngine JSEngine;

        private static readonly JSProgram JSProgram;
        #endregion

        #region Init
        static Engine() => JSProgram = new JavaScriptParser().Parse(string.Join('\n', Script.All.Select(x => x.GetScript()).Where(x => x is { })));

        public Engine() => JSEngine = new JSEngine().Execute(JSProgram);
        #endregion

        #region Functions
        public byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv) => JSEngine.GetValue("aesEncrypt").Invoke(JSEngine.CreateValue(data), JSEngine.CreateValue(key), JSEngine.CreateValue(iv)).AsArray().ToByteArray();

        public byte[] AesDecrypt(byte[] data, byte[] key) => JSEngine.GetValue("aesDecrypt").Invoke(JSEngine.CreateValue(data), JSEngine.CreateValue(key)).AsArray().ToByteArray();

        public byte[] Deflate(byte[] bytes) => JSEngine.GetValueByPath("pako", "gzip").Invoke(JSEngine.CreateValue(bytes)).AsArray().ToByteArray();

        public byte[] Inflate(byte[] bytes) => JSEngine.GetValueByPath("pako", "inflate").Invoke(JSEngine.CreateValue(bytes)).AsArray().ToByteArray();

        public void Dispose() => EnginePool.Release(this);
        #endregion
    }
}
