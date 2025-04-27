
namespace ExPresSXR.Minigames.Archery.ObjectPool
{
    /// <summary>
    /// Represents an object for which an object pool can execute functionality on return and retrieval.
    /// </summary>
    public interface IPoolObject
    {
        /// <summary>
        /// Gets automatically executed when the object is retrieved from a pool.
        /// </summary>
        public void HandlePoolRetrieved();

        /// <summary>
        /// Gets automatically executed when the object is returned to a pool.
        /// </summary>
        public void HandlePoolReturned();
    }
}