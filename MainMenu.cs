using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Gemstones.Scene
{
    public class MainMenu : IScene
    {
        private GameMain game;
        private readonly string name = "Main Menu";
        public Bitmap background { get; private set; }
        public SelectorText startGame { get; private set; }

        public MainMenu(GameMain game) {
            this.game = game;
            background = Helpers.LoadImage("mainbackground.png");
            startGame = new SelectorText("Start", 740, 600, 210, 42);
        }

        public void Draw(Graphics g) {
            g.DrawImage(background, 0, 0);
            g.FillRectangle(Brushes.Red, startGame.boundsRect);
            startGame.Draw(g, new Font("Starcraft", 36), Brushes.Gold);
        }

        public string GetName() {
            return name;
        }
    }
}
