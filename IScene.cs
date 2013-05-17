using System.Drawing;

namespace Gemstones
{
    public interface IScene
    {
        string GetName();
        void Draw(Graphics g);
    }
}
