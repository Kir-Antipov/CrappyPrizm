namespace CrappyPrizm
{
    public class Message
    {
        #region Var
        public string Text { get; }
        #endregion

        #region Init
        public Message(string text) => Text = text;
        
        public static implicit operator string(Message message) => message.Text;
        public static implicit operator Message(string text) => new Message(text);
        #endregion

        #region Functions
        public EncryptedMessage Encrypt(string publicKey, string secretPhrase)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => Text;
        public override int GetHashCode() => Text.GetHashCode();
        public override bool Equals(object? obj) => obj is Message message && message.Text == Text;
        #endregion
    }
}
