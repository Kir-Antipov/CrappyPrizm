namespace CrappyPrizm
{
    public class EncryptedMessage
    {
        #region Var
        public byte[] Data { get; }
        public byte[] Salt { get; }
        #endregion

        #region Init
        public EncryptedMessage(byte[] data, byte[] salt)
        {
            Data = data;
            Salt = salt;
        }
        #endregion
    }
}
