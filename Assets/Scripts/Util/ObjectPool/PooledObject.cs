using System;

namespace ggj2018.Util.ObjectPool
{
    public sealed class PooledObject : MonoBehavior
    {
#region Events
        public event EventHandler<EventArgs> RecycleEvent;
#endregion

        public string Tag { get; set; }

        public void Recycle()
        {
            ObjectPoolManager.Instance.Recycle(this);

            RecycleEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
