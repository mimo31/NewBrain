using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.IO;
using System.Security.Cryptography;
using NewBrain.NativeFunctions;

namespace NewBrain
{
    public enum PasswordBackground
    {
        Plain, Blocky, Ellipse, EqualSign, Hourglass
    }

    public partial class MainForm : Form
    {
        public TextBox InputBox { get; set; }
        public Label MainOutputLabel { get; set; }
        public TextBox PasswordBox { get; set; }
        public Label PasswordLabel { get; set; }
        public SpeechSynthesizer Speech { get; set; }
        public List<Function> Functions { get; set; } = new List<Function>();
        public List<string> LastCommands { get; set; } = new List<string>();
        public bool IsBrowsingCommands { get; set; }
        public int CommandIndex { get; set; }
        public string RootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Brain";
        public PasswordBackground PasswordBackground { get; set; }
        public Random R { get; set; }
        public Action OnSpeechCompleted;
        private bool locked;
        public bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
                if (value)
                {
                    this.PasswordBox.Show();
                    this.PasswordLabel.Show();
                    this.InputBox.Hide();
                    this.MainOutputLabel.Hide();
                    this.PasswordBackground = (PasswordBackground)this.R.Next(Enum.GetNames(typeof(PasswordBackground)).Length);
                }
                else
                {
                    this.PasswordBox.Hide();
                    this.PasswordLabel.Hide();
                    this.InputBox.Show();
                    this.MainOutputLabel.Show();
                }
                this.UpdateComponentSizes();
                this.Refresh();
            }
        }
        public bool SettingPassword { get; set; }
        public bool ConfirmingPassword { get; set; }
        private byte[] PasswordBytes;

        public MainForm()
        {
            InitializeComponent();
            this.InputBox = new TextBox();
            this.MainOutputLabel = new Label();
            this.PasswordBox = new TextBox();
            this.PasswordLabel = new Label();
            this.R = new Random();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.InitializeNativeFunctions();

            this.DoubleBuffered = true;

            this.WindowState = FormWindowState.Maximized;
            this.InputBox.Font = new Font("Arial", this.ClientSize.Height / 32, FontStyle.Regular, GraphicsUnit.Pixel);
            this.InputBox.Size = new Size(this.ClientSize.Width * 31 / 32, 0);
            this.InputBox.Location = new Point(this.ClientSize.Width / 64, this.ClientSize.Height / 32);
            this.InputBox.BorderStyle = BorderStyle.None;
            this.InputBox.KeyUp += this.InputKeyUp;

            this.MainOutputLabel.Font = new Font("Sans Serif", this.ClientSize.Height / 64, FontStyle.Regular, GraphicsUnit.Pixel);
            Rectangle mainOutputRect = new Rectangle(0, this.ClientSize.Height * 3 / 32, this.ClientSize.Width * 3 / 4, this.ClientSize.Height - this.ClientSize.Height * 3 / 32);
            int widthBorder = mainOutputRect.Width / 64;
            int heightBorder = mainOutputRect.Height / 64;
            mainOutputRect.X += widthBorder;
            mainOutputRect.Width -= 2 * widthBorder;
            mainOutputRect.Y += heightBorder;
            mainOutputRect.Height -= 2 * heightBorder;
            this.MainOutputLabel.Location = mainOutputRect.Location;
            this.MainOutputLabel.Size = mainOutputRect.Size;

            this.PasswordBox.Font = new Font("Arial", this.ClientSize.Height / 32, FontStyle.Regular, GraphicsUnit.Pixel);
            this.PasswordBox.Size = new Size(this.ClientSize.Width * 3 / 4, this.ClientSize.Height / 32);
            this.PasswordBox.Location = new Point(this.ClientSize.Width / 8, this.ClientSize.Height * 3 / 4);
            this.PasswordBox.UseSystemPasswordChar = true;
            this.PasswordBox.KeyUp += this.PasswordBoxKeyUp;

            this.PasswordLabel.Font = new Font("Sans Serif", this.ClientSize.Height / 32, FontStyle.Regular, GraphicsUnit.Pixel);
            this.PasswordLabel.Size = new Size(this.ClientSize.Width - this.ClientSize.Width / 8, this.ClientSize.Height / 2);
            this.PasswordLabel.Location = new Point(this.ClientSize.Width / 16, this.ClientSize.Height / 4);
            this.PasswordLabel.BackColor = Color.Transparent;
            this.PasswordLabel.TextAlign = ContentAlignment.MiddleCenter;

            this.Speech = new SpeechSynthesizer();
            this.Speech.SpeakCompleted += this.SpeakingCompleted;

            this.Controls.Add(this.InputBox);
            this.Controls.Add(this.MainOutputLabel);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.PasswordLabel);

            if (Directory.Exists(RootDirectory))
            {
                this.WriteInPasswordAndSay("Enter the password please.");
                this.Locked = true;
            }
            else
            {
                this.WriteInPasswordAndSay("Enter the password you will use to unlock the system.");
                this.InputBox.Hide();
                this.MainOutputLabel.Hide();
                this.SettingPassword = true;
                this.PasswordBackground = (PasswordBackground)this.R.Next(Enum.GetNames(typeof(PasswordBackground)).Length);
            }
            this.Refresh();
        }

        private void UpdateComponentSizes()
        {
            if (this.Locked || this.SettingPassword)
            {
                this.PasswordBox.Font = new Font("Arial", this.ClientSize.Height / 32, FontStyle.Regular, GraphicsUnit.Pixel);
                this.PasswordBox.Size = new Size(this.ClientSize.Width * 3 / 4, this.ClientSize.Height / 32);
                this.PasswordBox.Location = new Point(this.ClientSize.Width / 8, this.ClientSize.Height * 3 / 4);

                this.PasswordLabel.Font = new Font("Sans Serif", this.ClientSize.Height / 32, FontStyle.Regular, GraphicsUnit.Pixel);
                this.PasswordLabel.Size = new Size(this.ClientSize.Width - this.ClientSize.Width / 8, this.ClientSize.Height / 2);
                this.PasswordLabel.Location = new Point(this.ClientSize.Width / 16, this.ClientSize.Height / 4);
            }
            else
            {
                this.InputBox.Font = new Font("Arial", this.ClientSize.Height / 32, FontStyle.Regular, GraphicsUnit.Pixel);
                this.InputBox.Size = new Size(this.ClientSize.Width * 31 / 32, 0);
                this.InputBox.Location = new Point(this.ClientSize.Width / 64, this.ClientSize.Height / 32);

                this.MainOutputLabel.Font = new Font("Sans Serif", this.ClientSize.Height / 64, FontStyle.Regular, GraphicsUnit.Pixel);
                Rectangle mainOutputRect = new Rectangle(0, this.ClientSize.Height * 3 / 32, this.ClientSize.Width * 3 / 4, this.ClientSize.Height - this.ClientSize.Height * 3 / 32);
                int widthBorder = mainOutputRect.Width / 64;
                int heightBorder = mainOutputRect.Height / 64;
                mainOutputRect.X += widthBorder;
                mainOutputRect.Width -= 2 * widthBorder;
                mainOutputRect.Y += heightBorder;
                mainOutputRect.Height -= 2 * heightBorder;
                this.MainOutputLabel.Location = mainOutputRect.Location;
                this.MainOutputLabel.Size = mainOutputRect.Size;
            }
        }

        private void SpeakingCompleted(object sender, EventArgs e)
        {
            if (this.OnSpeechCompleted != null)
            {
                this.OnSpeechCompleted();
                this.OnSpeechCompleted = null;
            }
        }

        private void InitializeNativeFunctions()
        {
            this.Functions.Add(new TimeFunction());
            this.Functions.Add(new CloseFunction());
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (this.Locked || this.SettingPassword)
            {
                switch (this.PasswordBackground)
                {
                    case PasswordBackground.Blocky:
                        for (int i = 0; i < 64; i++)
                        {
                            for (int j = 0; j < 32; j++)
                            {
                                Brush selectedBrush;
                                if ((j + i) % 2 == 0)
                                {
                                    selectedBrush = Brushes.LightGoldenrodYellow;
                                }
                                else
                                {
                                    selectedBrush = Brushes.Yellow;
                                }
                                e.Graphics.FillRectangle(selectedBrush, i * this.ClientSize.Width / 64, j * this.ClientSize.Height / 32, this.ClientSize.Width / 64, this.ClientSize.Height / 32);
                            }
                        }
                        break;
                    case PasswordBackground.Plain:
                        e.Graphics.FillRectangle(Brushes.Azure, this.ClientRectangle);
                        break;
                    case PasswordBackground.Ellipse:
                        e.Graphics.FillRectangle(Brushes.LightSalmon, this.ClientRectangle);
                        e.Graphics.FillEllipse(Brushes.LightGreen, this.ClientRectangle);
                        break;
                    case PasswordBackground.EqualSign:
                        int signWidth = this.ClientSize.Width / 2;
                        int signLineHeight = this.ClientSize.Height / 16;
                        e.Graphics.FillRectangle(Brushes.LightSteelBlue, this.ClientRectangle);
                        e.Graphics.FillRectangle(Brushes.LightSeaGreen, this.ClientSize.Width / 4, this.ClientSize.Height * 3 / 8 - signLineHeight / 2, signWidth, signLineHeight);
                        e.Graphics.FillRectangle(Brushes.LightSeaGreen, this.ClientSize.Width / 4, this.ClientSize.Height * 5 / 8 - signLineHeight / 2, signWidth, signLineHeight);
                        break;
                    case PasswordBackground.Hourglass:
                        e.Graphics.FillRectangle(Brushes.LemonChiffon, this.ClientRectangle);
                        Point centerPoint = new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
                        Point[] leftTriangle = new Point[] { new Point(0, 0), new Point(0, this.ClientSize.Height), centerPoint};
                        Point[] rightTriangle = new Point[] { new Point(this.ClientSize.Width, 0), centerPoint, new Point(this.ClientSize.Width, this.ClientSize.Height) };
                        e.Graphics.FillPolygon(Brushes.Bisque, leftTriangle);
                        e.Graphics.FillPolygon(Brushes.Bisque, rightTriangle);
                        break;
                }
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.Black, 0, 0, this.ClientSize.Width, this.ClientSize.Height * 3 / 32);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.UpdateComponentSizes();
            this.Refresh();
        }

        private void InputKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this.Process(this.InputBox.Text);
                    this.InputBox.Text = "";
                    break;
                case Keys.Up:
                    if (!this.IsBrowsingCommands)
                    {
                        if (this.LastCommands.Count != 0)
                        {
                            this.IsBrowsingCommands = true;
                            this.InputBox.Text = this.LastCommands[this.LastCommands.Count - 1];
                            this.InputBox.SelectionStart = this.InputBox.Text.Length;
                        }
                    }
                    else
                    {
                        if (this.CommandIndex != this.LastCommands.Count - 1)
                        {
                            this.CommandIndex++;
                        }
                        this.InputBox.Text = this.LastCommands[this.LastCommands.Count - 1 - this.CommandIndex];
                        this.InputBox.SelectionStart = this.InputBox.Text.Length;
                    }
                    break;
                case Keys.Down:
                    if (this.IsBrowsingCommands)
                    {
                        if (this.CommandIndex != 0)
                        {
                            this.CommandIndex--;
                            this.InputBox.Text = this.LastCommands[this.LastCommands.Count - 1 - this.CommandIndex];
                            this.InputBox.SelectionStart = this.InputBox.Text.Length;
                        }
                        else
                        {
                            this.InputBox.Text = this.LastCommands[this.LastCommands.Count - 1];
                            this.InputBox.SelectionStart = this.InputBox.Text.Length;
                        }
                    }
                    break;
            }
        }

        private void PasswordBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.Locked)
                {
                    if (!this.PasswordBox.Text.Equals(""))
                    {
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(this.PasswordBox.Text);
                        byte[] hash = HashAlgorithm.Create("SHA512").ComputeHash(passwordBytes);
                        byte[] realHash = File.ReadAllBytes(this.RootDirectory + "\\Hash.dat");
                        if (hash.EqualsArray(realHash))
                        {
                            this.MainOutputLabel.Text = "Welcome.";
                            this.OnSpeechCompleted += () =>
                            {
                                this.Locked = false;
                                this.InputBox.Focus();
                            };
                            this.WriteInPasswordAndSay("Welcome.");
                        }
                        else
                        {
                            this.OnSpeechCompleted += () => this.PasswordLabel.Text = "Enter the password please.";
                            this.WriteInPasswordAndSay("Ouch, it looks like this is not the right password.");
                        }
                    }
                    else
                    {
                        this.OnSpeechCompleted += () => this.PasswordLabel.Text = "Enter the password please.";
                        this.WriteInPasswordAndSay("Fill in the password please!");
                    }
                }
                else
                {
                    if (this.ConfirmingPassword)
                    {
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(this.PasswordBox.Text);
                        if (passwordBytes.EqualsArray(this.PasswordBytes))
                        {
                            byte[] hash = HashAlgorithm.Create("SHA512").ComputeHash(passwordBytes);
                            Directory.CreateDirectory(this.RootDirectory);
                            File.WriteAllBytes(this.RootDirectory + "\\Hash.dat", hash);
                            this.MainOutputLabel.Text = "Your password is now set.";
                            this.OnSpeechCompleted += () =>
                            {
                                this.SettingPassword = false;
                                this.ConfirmingPassword = false;
                                this.InputBox.Show();
                                this.MainOutputLabel.Show();
                                this.PasswordBox.Hide();
                                this.PasswordLabel.Hide();
                                this.InputBox.Focus();
                                this.Refresh();
                            };
                            this.WriteInPasswordAndSay("Your password is now set.");
                        }
                        else
                        {
                            this.OnSpeechCompleted += () =>
                            {
                                this.PasswordLabel.Text = "Enter the password you will use to unlock the system.";
                                this.ConfirmingPassword = false;
                                this.PasswordBytes = null;
                            };
                            this.WriteInPasswordAndSay("The passwords do not match!");
                        }
                    }
                    else
                    {
                        if (!this.PasswordBox.Text.Equals(""))
                        {
                            this.PasswordBytes = Encoding.UTF8.GetBytes(this.PasswordBox.Text);
                            this.ConfirmingPassword = true;
                            this.WriteInPasswordAndSay("Confirm your password please.");
                        }
                        else
                        {
                            this.OnSpeechCompleted += () => this.PasswordLabel.Text = "Enter the password you will use to unlock the system.";
                            this.WriteInPasswordAndSay("Fill in the password please!");
                        }
                    }
                }
                this.PasswordBox.Text = "";
            }
        }

        public void WriteAndSay(string text)
        {
            this.MainOutputLabel.Text = text;
            this.Speech.SpeakAsync(text);
        }

        private void WriteInPasswordAndSay(string text)
        {
            this.PasswordLabel.Text = text;
            this.Speech.SpeakAsync(text);
        }

        private void Process(string text)
        {
            if (!text.Equals(""))
            {
                this.IsBrowsingCommands = false;
                this.CommandIndex = 0;
                foreach (Function function in this.Functions)
                {
                    if (function.IsAsked(text))
                    {
                        function.Ask(this, text);
                        break;
                    }
                }
                this.LastCommands.Add(text);
            }
        }
    }
}
