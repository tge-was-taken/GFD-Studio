using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class SceneAdapter : ResourceAdapter<Scene>
    {
        public override MenuFlags ContextMenuFlags
            => MenuFlags.Delete | MenuFlags.Export | MenuFlags.Move | MenuFlags.Replace;

        public override Flags NodeFlags
            => Flags.Branch;

        public SceneAdapter(string text, Scene scene) : base(text, scene)
        {
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            base.InitializeViewCore();
        }
    }
}