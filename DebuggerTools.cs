using System.Drawing;
using System.Collections.Generic;
using Gemstones.Board;

namespace Gemstones.Debugger
{
    public class DebuggerTools
    {
        private GameMain gameMain;

        public DebuggerTools(GameMain gameMain) {
            this.gameMain = gameMain;
        }

        public void GenerateGem(GemName name) {
            Point cursor = gameMain.inGame.gameBoard.GetTranslatedCursor();
            foreach (List<Gem> list in gameMain.inGame.gameBoard.board)
                foreach(Gem gem in list) {
                    if (Helpers.IsWithin(gem.bounds, cursor)) {
                        gem.name = name;
                        return;
                    }
                }
        }
    }
}
