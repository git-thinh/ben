using CefSharp;
using Gma.System.MouseKeyHook;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms; 
using CefSharp.WinForms; 

namespace browser
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (!CEF.Initialize(new Settings())) return;
            Application.Run(new fMain());
            CEF.Shutdown();
        }
    }

    class fMain : Form
    {
        readonly WebView ui_browser;
        const bool m_hook_MouseMove = true;
        bool m_resizing = false;

        /*////////////////////////////////////////////////////////////////////////*/

        #region [ MOUSE MOVE: IN FORM, OUT FORM ]

        private void f_mouse_move_intoForm(int x, int y)
        {
            f_form_Resize(x, y, MOUSE_XY.INT);
        }

        private void f_mouse_move_outForm(int x, int y)
        {
            f_form_Resize(x, y, MOUSE_XY.OUT);
        }

        #endregion

        #region [ FORM MOVE, RESIZE ]

        enum MOUSE_XY { OUT, INT };

        void f_form_Resize(int x, int y, MOUSE_XY type)
        {
            if (m_resizing)
            {
                int max_x = this.Location.X + this.Width;
                int max_y = this.Location.Y + this.Height;
                this.Width = x - this.Location.X;
                this.Height = y - this.Location.Y;
            }
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void f_form_move_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion

        #region [ HOOK MOUSE: MOVE, WHEEL ... ]

        void f_hook_mouse_move_CallBack(MouseEventArgs e)
        {
            int max_x = this.Width + this.Location.X,
                max_y = e.Location.Y + this.Height;
            //Debug.WriteLine(this.Location.X + " " +e.X  + " " + max_x + " | " + this.Location.Y + " " +e.Y  + " " + max_y);

            if (e.X > this.Location.X && e.X < max_x
                && e.Y > this.Location.Y && e.Y < max_y)
            {
                //Debug.WriteLine("IN FORM: "+ this.Location.X + " " + e.X + " " + max_x + " | " + this.Location.Y + " " + e.Y + " " + max_y);
                f_mouse_move_intoForm(e.X, e.Y);
            }
            else
            {
                //Debug.WriteLine("OUT FORM: " + this.Location.X + " " + e.X + " " + max_x + " | " + this.Location.Y + " " + e.Y + " " + max_y);
                f_mouse_move_outForm(e.X, e.Y);
            }
        }

        void f_hook_mouse_Open()
        {
            if (m_hook_MouseMove)
                f_hook_mouse_SubscribeGlobal();
        }

        void f_hook_mouse_Close()
        {
            if (m_hook_MouseMove)
                f_hook_mouse_Unsubscribe();
        }

        /*////////////////////////////////////////////////////////////////////////*/

        private IKeyboardMouseEvents hook_events;

        private void f_hook_mouse_SubscribeApplication()
        {
            f_hook_mouse_Unsubscribe();
            f_hook_mouse_Subscribe(Hook.AppEvents());
        }

        private void f_hook_mouse_SubscribeGlobal()
        {
            f_hook_mouse_Unsubscribe();
            f_hook_mouse_Subscribe(Hook.GlobalEvents());
        }

        private void f_hook_mouse_Subscribe(IKeyboardMouseEvents events)
        {
            hook_events = events;
            //m_Events.KeyDown += OnKeyDown;
            //m_Events.KeyUp += OnKeyUp;
            //m_Events.KeyPress += HookManager_KeyPress;

            //m_Events.MouseUp += OnMouseUp;
            //m_Events.MouseClick += OnMouseClick;
            //m_Events.MouseDoubleClick += OnMouseDoubleClick;

            hook_events.MouseMove += f_hook_mouse_HookManager_MouseMove;

            //m_Events.MouseDragStarted += OnMouseDragStarted;
            //m_Events.MouseDragFinished += OnMouseDragFinished;

            //if (checkBoxSupressMouseWheel.Checked)
            //m_Events.MouseWheelExt += f_hook_mouse_HookManager_MouseWheelExt;
            //else
            ////hook_events.MouseWheel += f_hook_mouse_HookManager_MouseWheel;

            //if (checkBoxSuppressMouse.Checked)
            //m_Events.MouseDownExt += HookManager_Supress;
            //else
            //m_Events.MouseDown += OnMouseDown;
        }


        private void f_hook_mouse_Unsubscribe()
        {
            if (hook_events == null) return;
            //m_Events.KeyDown -= OnKeyDown;
            //m_Events.KeyUp -= OnKeyUp;
            //m_Events.KeyPress -= HookManager_KeyPress;

            //m_Events.MouseUp -= OnMouseUp;
            //m_Events.MouseClick -= OnMouseClick;
            //m_Events.MouseDoubleClick -= OnMouseDoubleClick;

            hook_events.MouseMove -= f_hook_mouse_HookManager_MouseMove;

            //m_Events.MouseDragStarted -= OnMouseDragStarted;
            //m_Events.MouseDragFinished -= OnMouseDragFinished;

            //if (checkBoxSupressMouseWheel.Checked)
            //m_Events.MouseWheelExt -= f_hook_mouse_HookManager_MouseWheelExt;
            //else
            //hook_events.MouseWheel -= f_hook_mouse_HookManager_MouseWheel;

            //if (checkBoxSuppressMouse.Checked)
            //m_Events.MouseDownExt -= HookManager_Supress;
            //else
            //m_Events.MouseDown -= OnMouseDown;

            hook_events.Dispose();
            hook_events = null;
        }

        private void f_hook_mouse_HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            f_hook_mouse_move_CallBack(e);
        }

        ////private void f_hook_mouse_HookManager_MouseWheel(object sender, MouseEventArgs e)
        ////{
        ////    //Debug.WriteLine(string.Format("Wheel={0:000}", e.Delta));
        ////    //f_hook_mouse_wheel_CallBack(e);
        ////}

        ////private void f_hook_mouse_HookManager_MouseWheelExt(object sender, MouseEventExtArgs e)
        ////{
        ////    //Debug.WriteLine(string.Format("Wheel={0:000}", e.Delta)); 
        ////    //Debug.WriteLine("Mouse Wheel Move Suppressed.\n");
        ////    e.Handled = true;
        ////    //e.Handled = true; // true: break event at here, stop mouse wheel at here
        ////}

        /////////////////////////////////////////////////////////////


        #endregion

        /*////////////////////////////////////////////////////////////////////////*/


        public fMain()
        { 
            this.Icon = browser.Properties.Resources.icon;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            var ui_move = new ControlTransparent()
            {
                Location = new Point(0, 0),
                BackColor = Color.Transparent,
                //BackColor = Color.Orange,
                Height = 45,
                Width = 99
            };
            this.Controls.Add(ui_move);
            ui_move.MouseMove += f_form_move_MouseDown;
            
            var ui_resize = new Label()
            {
                Text = string.Empty,
                Width = 15,
                Height = 15,
                BackColor = Color.Transparent,
            };
            this.Controls.Add(ui_resize);

            ui_resize.MouseDown += (se, ev) => { f_hook_mouse_Open(); m_resizing = true; };
            ui_resize.MouseUp += (se, ev) =>
            {
                m_resizing = false;
                f_hook_mouse_Close();
            };

            var ui_close = new Label() {
                Width = 17, Height = 17,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = string.Empty,
                BackColor = Color.Orange,
            };
            this.Controls.Add(ui_close);
            ui_close.MouseDoubleClick += (se, ev) => this.Close();
            ui_close.MouseMove += f_form_move_MouseDown;

            ui_browser = new WebView("about:blank", new BrowserSettings() {
                PageCacheDisabled = true,
                WebSecurityDisabled = true,
                ApplicationCacheDisabled = true
            });
            ui_browser.Dock = DockStyle.Fill;
            this.Controls.Add(ui_browser);
            ui_browser.MenuHandler = new MenuHandler();


            
            ContextMenuStrip myMenu = new ContextMenuStrip();
            this.ContextMenuStrip = myMenu; 
            myMenu.Items.Add("Reload Page");
            myMenu.Items.Add("Show DevTools");
            myMenu.Items.Add("Go Url");

            myMenu.Items[0].Click += new EventHandler(f_menuContext_Click);
            myMenu.Items[1].Click += new EventHandler(f_menuContext_Click);
            myMenu.Items[2].Click += new EventHandler(f_menuContext_Click);

            //myMenu.Items.Add("Form Info2"); // add some items to the menu
            //// create item with submenus Item1 and Item2
            //ToolStripMenuItem mySubMenu = new ToolStripMenuItem();
            //mySubMenu.Text = "InfoMenu1";
            //ToolStripMenuItem myItem = new ToolStripMenuItem();
            //myItem.Text = "Item1";
            ////myItem.Click += new EventHandler(Item1_Menu_Click);
            //mySubMenu.DropDownItems.Add(myItem);
            //myItem = new ToolStripMenuItem(); //reuse of name
            //myItem.Text = "Item2";
            //mySubMenu.DropDownItems.Add(myItem);
            //myMenu.Items.Add(mySubMenu);
            //// further menu items can be added etc.
            //myMenu.Items.Add("Form close");
            ////fourth item added so index here is 3
            ////myMenu.Items[3].Click += new EventHandler(FormCloseMenu_Click);



            this.Shown += (se, ev) => {
                this.Width = 999;
                this.Height = Screen.PrimaryScreen.WorkingArea.Height - 100;
                this.Top = 45;
                this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;

                ui_close.Location = new Point(this.Width - 17, 0);
                ui_close.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                ui_move.Width = this.Width - (123 + 320);
                ui_move.Height = 48;
                ui_move.Location = new Point(123, 0);
                ui_move.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                            
                ui_resize.Location = new Point(this.Width - 15, this.Height - 15);
                ui_resize.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                ui_resize.BringToFront();
                ui_move.BringToFront();
                ui_close.BringToFront();
            };
        }

        private void f_menuContext_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem m = (ToolStripMenuItem)sender;
            //MessageBox.Show(m.Text);
            switch (m.Text)
            {
                case "Reload Page":
                    this.ui_browser.Stop();
                    this.ui_browser.Reload();
                    break;
                case "Show DevTools":
                    this.ui_browser.ShowDevTools();
                    break;
                case "Go Url":
                    string url = Microsoft.VisualBasic.Interaction.InputBox("Input URL:", "Go page", "http://localhost:56789/");
                    Uri u;
                    if (Uri.TryCreate(url, UriKind.Absolute, out u))
                    {
                        this.ui_browser.Stop();
                        this.ui_browser.Load(url);
                    }
                    break;
            }
        }
    }

    class MenuHandler : IMenuHandler
    {
        public bool OnBeforeMenu(IWebBrowser browser)
        {
            return true;
        }
    }


}
