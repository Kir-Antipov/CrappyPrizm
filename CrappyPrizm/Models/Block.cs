using System.Numerics;

namespace CrappyPrizm.Models
{
    public class Block
    {
        #region Var
        public BigInteger Id { get; }
        public int Height { get; }
        public int MaxHeight { get; }
        #endregion

        #region Init
        public Block(BigInteger id, int height, int maxHeight = int.MaxValue)
        {
            Id = id;
            Height = height;
            MaxHeight = maxHeight;
        }
        #endregion

        #region Functions
        public override string ToString() => Id.ToString();
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object? obj) => obj is Block block && block.Id == Id;
        #endregion
    }
}
