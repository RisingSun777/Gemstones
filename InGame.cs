using System.Windows.Forms;
using System.Drawing;
using Gemstones.Board;

namespace Gemstones.Scene
{
    public class InGame : IScene, IInputEvent
    {
        public GameMain main { get; private set; }
        private readonly string name = "Ingame";
        public GameBoard gameBoard { get; private set; }
        private Bitmap background;

        public InGame(GameMain main) {
            this.main = main;
            gameBoard = new GameBoard(this);
        }

        public void Draw(Graphics g) {
            gameBoard.Draw(g);
        }

        public string GetName() {
            return name;
        }

        public void MouseClicked(MouseEventArgs e) {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    Gemstones.Board.Gem gem = gameBoard.GetCurrentGem();
                    if (gem == null)
                        return;
                    if (!gem.isSelected)
                    {
                        if (gameBoard.currentlySelectedGem != null)
                        {
                            if (SwitchGem(gem, gameBoard.currentlySelectedGem))
                            {
                                gameBoard.currentlySelectedGem.isSelected = false;
                                gameBoard.currentlySelectedGem = null;
                                break;
                            }
                            gameBoard.currentlySelectedGem.isSelected = false;
                        }
                        gem.isSelected = true;
                        gameBoard.currentlySelectedGem = gem;
                    }
                    else
                    {
                        gem.isSelected = false;
                        gameBoard.currentlySelectedGem = null;
                    }
                    break;
            }
        }

        private bool SwitchGem(Gem src, Gem dst) {
            if ((src.index.X == dst.index.X && (src.index.Y == dst.index.Y + 1 || src.index.Y == dst.index.Y - 1))
                || (src.index.Y == dst.index.Y && (src.index.X == dst.index.X + 1 || src.index.X == dst.index.X - 1)))
            {
                src.SwitchWithAnimation(dst);
                return true;
            }
            return false;
        }
    }
}
