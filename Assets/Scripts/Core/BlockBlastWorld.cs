using LSCore;

namespace Core
{
    public class BlockBlastWorld : ServiceManager<BlockBlastWorld>
    {
        protected override void Awake()
        {
            base.Awake();
            Init();
        }
        
        private void Init()
        {
            BlockBlastWindow.AsHome();
            BlockBlastWindow.Show();
        }
    }
}