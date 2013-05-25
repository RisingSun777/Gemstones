using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gemstones
{
    public class GameMain
    {
        private Form form;
        public static readonly string title = "Gemstones";
        public static readonly int frameRate = 1000 / 60;
        public static Size screenSize = new Size(1024, 768);
        public static Point cursorLocation { get; private set; }

        public Debugger.DebuggerTools debuggerTools { get; private set; }
        public PictureBox pb;
        public Graphics device;
        public Bitmap surface;
        private Timer timer;

        public Scene.MainMenu mainMenu { get; private set; }
        public Scene.InGame inGame { get; private set; }
        public IScene currentScene;
        public IInputEvent currentInput;

        public GameMain(Form form) {
            this.form = form;
            
            form.Location = new Point(150, 0);
            form.Text = title;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Size = screenSize;
            form.Icon = Properties.Resources.gameIcon;

            pb = new PictureBox();
            pb.Parent = form;
            pb.Dock = DockStyle.Fill;
            pb.BackColor = Color.Black;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.MouseClick += new MouseEventHandler(pb_MouseClick);

            surface = new Bitmap(screenSize.Width, screenSize.Height);
            pb.Image = surface;
            device = Graphics.FromImage(surface);

            debuggerTools = new Debugger.DebuggerTools(this);

            timer = new Timer();
            timer.Interval = GameMain.frameRate;
            timer.Tick += new EventHandler(timer_Tick);

            mainMenu = new Scene.MainMenu(this);
            inGame = new Scene.InGame(this);

            UpdateScene(mainMenu);
        }

        private void timer_Tick(object sender, EventArgs e) {
            Update();
        }

        public void Start() {
            mainMenu.Draw(device);
            timer.Start();
        }

        public void DrawCursorLocation()
        {
            device.DrawString("Mouse X: " + cursorLocation.X, new Font("Arial", 20), Brushes.Gold, 100, 100);
            device.DrawString("Mouse Y: " + cursorLocation.Y, new Font("Arial", 20), Brushes.Gold, 100, 150);
        }

        public void Update() {
            if (device == null)
                return;
            
            device.Clear(Color.Black);

            //update cursor location
            cursorLocation = GetCursorLocation();

            currentScene.Draw(device);           
            //DrawCursorLocation();

            pb.Invalidate();
        }

        private Point GetCursorLocation() {
            Point location = form.PointToClient(Cursor.Position);
            //if (form.FormBorderStyle == FormBorderStyle.None)
                Point temp = form.PointToClient(form.Location);
                location.X -= temp.X;
                location.Y -= temp.Y;
            
            return location;
        }

        public void UpdateScene(IScene scene) {
            currentScene = scene;
            if (scene is IInputEvent)
                currentInput = scene as IInputEvent;
            else
                currentInput = null;
        }

        public void pb_MouseClick(object sender, MouseEventArgs e)
        {
            if(currentInput != null)
                currentInput.MouseClicked(e);
        }
    }
}