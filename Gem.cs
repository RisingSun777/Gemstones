using System.Drawing;
using System.Threading;
using System;

namespace Gemstones.Board
{
    public enum GemName {
        Axinite, Ruby, Amethyst, Emerald, Sapphire, Diamond, Nothing
    };

    public class Gem
    {
        public static readonly Bitmap spriteImage = Helpers.LoadImage("GemSprite.png");
        public static readonly int size = GameBoard.playField.Width / GameBoard.size;
        public static readonly Rectangle selectionSpriteLocation = new Rectangle(size, size, size, size);
        public static readonly Rectangle selectedSpriteLocation = new Rectangle(0, size, size, size);

        private GameBoard board;

        public bool isSelected = false;
        public GemName name;
        public Rectangle bounds { get; private set; }
        public Point index { get; private set; }
        public PointF scaleParameter = new PointF(1.0f, 1.0f);

        public Gem(GameBoard board, GemName name, Rectangle bounds, Point index) {
            this.board = board;
            this.name = name;
            this.bounds = bounds;
            this.index = index;
        }

        public Gem Clone() {
            return new Gem(this.board, this.name, this.bounds, this.index);
        }

        public void SwitchWithAnimation(Gem dst) {
            Thread thread = new Thread(ThreadSwitchUpdate);
            thread.Name = "SwitchGem";
            thread.Start(dst);
        }

        public void SetScaleParameter(float num) {
            if (num <= 0 || scaleParameter.X <= 0 || scaleParameter.Y <= 0)
            {
                scaleParameter.X = scaleParameter.Y = 0f;
                return;
            }
            scaleParameter.X = num;
            scaleParameter.Y = num;
        }

        public void ResetToNothing() {
            name = GemName.Nothing;
        }

        public void Draw(Graphics g)
        {
            Rectangle src;
            if (name == GemName.Nothing)
                src = new Rectangle(0, 0, 0, 0);
            else
                src = new Rectangle(size * (int)name, 0, size, size);

            //If the cursor is hovering, draw the selection. If the gem is selected, draw the selection border
            Point cursor = board.GetTranslatedCursor();
            if (Helpers.IsWithin(bounds, cursor) && !name.Equals(GemName.Nothing))
            {
                g.DrawImage(spriteImage, bounds, selectionSpriteLocation, GraphicsUnit.Pixel);
                //DrawInfo(g);
            }
            if (isSelected)
                g.DrawImage(spriteImage, bounds, selectedSpriteLocation, GraphicsUnit.Pixel);

            if (scaleParameter.X > 0)
            {
                System.Drawing.Drawing2D.Matrix transform = g.Transform;
                g.ScaleTransform(scaleParameter.X, scaleParameter.Y);
                g.DrawImage(spriteImage, (bounds.X + bounds.Width * (1 - scaleParameter.X) / 2) / scaleParameter.X, (bounds.Y + bounds.Height * (1 - scaleParameter.Y) / 2) / scaleParameter.Y, src, GraphicsUnit.Pixel);
                g.Transform = transform;
            }
        }

        private void DrawInfo(Graphics g) { 
            Font font = new Font("Arial", 12, FontStyle.Regular);
            g.DrawString("IsSelected = " + isSelected.ToString(), font, Brushes.Gold, 600, 50);
            g.DrawString("Name = " + name.ToString(), font, Brushes.Gold, 600, 100);
            g.DrawString("Index = " + index.ToString(), font, Brushes.Gold, 600, 150);
            g.DrawString("Bounds = " + bounds.ToString(), font, Brushes.Gold, 600, 200);
            g.DrawString("Scale = " + scaleParameter.ToString(), font, Brushes.Gold, 600, 250);
        }

        public void SwitchPlaceWith(Gem dst) {
            if (Thread.CurrentThread.Name != "SwitchGem" || dst == null)
                return;

            int skewX = this.bounds.X - ((Gem)dst).bounds.X;
            int PixelJump = skewX >= 0 ? 5 : -5;
            if (skewX != 0)
                for (int i = 0; i < Math.Abs(skewX); i += Math.Abs(PixelJump))
                {
                    this.bounds = new Rectangle(this.bounds.X - PixelJump, this.bounds.Y, size, size);
                    dst.bounds = new Rectangle(dst.bounds.X + PixelJump, dst.bounds.Y, size, size);
                    Thread.Sleep(GameMain.frameRate);
                }

            int skewY = this.bounds.Y - ((Gem)dst).bounds.Y;
            PixelJump = skewY >= 0 ? 5 : -5;
            if (skewY != 0)
                for (int i = 0; i < Math.Abs(skewY); i += Math.Abs(PixelJump))
                {
                    this.bounds = new Rectangle(this.bounds.X, this.bounds.Y - PixelJump, size, size);
                    dst.bounds = new Rectangle(dst.bounds.X, dst.bounds.Y + PixelJump, size, size);
                    Thread.Sleep(GameMain.frameRate);
                }
            BoardDataSwap((Gem)dst);
        }

        public void BoardDataSwap(Gem dst) {
            lock(board.board) {
            Gem temp = this;
            //Swap the pointers' values, while maintaining their positions in the board
            board.board[this.index.X][this.index.Y] = board.board[dst.index.X][dst.index.Y];
            board.board[dst.index.X][dst.index.Y] = temp;

            temp = this.Clone();
            this.index = dst.index;
            dst.index = temp.index;
            }
        }

        private void ThreadSwitchUpdate(object dst)
        {
            if (!Thread.CurrentThread.Name.Equals("SwitchGem"))
                return;
            board.inGame.IsAbleToClick = false;

            lock (board.board)
            {
                int skewX = this.bounds.X - ((Gem)dst).bounds.X;
                if (skewX != 0)
                    for (int i = 0; i < size; i += 5)
                    {
                        this.bounds = new Rectangle(this.bounds.X - skewX / 14, this.bounds.Y, size, size);
                        ((Gem)dst).bounds = new Rectangle(((Gem)dst).bounds.X + skewX / 14, ((Gem)dst).bounds.Y, size, size);
                        Thread.Sleep(GameMain.frameRate);
                    }

                int skewY = this.bounds.Y - ((Gem)dst).bounds.Y;
                if (skewY != 0)
                    for (int i = 0; i < size; i += 5)
                    {
                        this.bounds = new Rectangle(this.bounds.X, this.bounds.Y - skewY / 14, size, size);
                        ((Gem)dst).bounds = new Rectangle(((Gem)dst).bounds.X, ((Gem)dst).bounds.Y + skewY / 14, size, size);
                        Thread.Sleep(GameMain.frameRate);
                    }
                BoardDataSwap((Gem)dst);

                if (!board.CheckAndDoMatching())
                {
                    skewX = this.bounds.X - ((Gem)dst).bounds.X;
                    skewY = this.bounds.Y - ((Gem)dst).bounds.Y;
                    if (skewX != 0)
                        for (int i = 0; i < size; i += 5)
                        {
                            this.bounds = new Rectangle(this.bounds.X - skewX / 14, this.bounds.Y, size, size);
                            ((Gem)dst).bounds = new Rectangle(((Gem)dst).bounds.X + skewX / 14, ((Gem)dst).bounds.Y, size, size);
                            Thread.Sleep(GameMain.frameRate);
                        }
                    if (skewY != 0)
                        for (int i = 0; i < size; i += 5)
                        {
                            this.bounds = new Rectangle(this.bounds.X, this.bounds.Y - skewY / 14, size, size);
                            ((Gem)dst).bounds = new Rectangle(((Gem)dst).bounds.X, ((Gem)dst).bounds.Y + skewY / 14, size, size);
                            Thread.Sleep(GameMain.frameRate);
                        }
                    BoardDataSwap((Gem)dst);
                }
            }

            board.inGame.IsAbleToClick = true;
        }
    }
}
