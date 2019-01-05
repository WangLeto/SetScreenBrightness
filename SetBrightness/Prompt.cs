using System.Windows.Forms;

namespace SetBrightness
{
    public static class Prompt
    {
        private static Form _prompt;

        public static string ShowDialog(string text, string caption)
        {
            TextBox textBox;
            if (_prompt != null)
            {
                textBox = (TextBox) _prompt.Controls.Find("textBox", false)[0];
            }
            else
            {
                _prompt = new Form()
                {
                    Width = 500,
                    Height = 170,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = "重命名显示器",
                    StartPosition = FormStartPosition.CenterScreen
                };
                var textLabel = new Label {Left = 50, Top = 20, Text = text};
                textBox = new TextBox
                {
                    Left = 50,
                    Top = 50,
                    Width = 400,
                    Name = "textBox",
                    Text = caption
                };
                var confirmation = new Button
                    {Text = "Ok", Left = 350, Width = 100, Top = 80, DialogResult = DialogResult.OK};
                confirmation.Click += (sender, e) => { _prompt.Close(); };
                _prompt.Controls.Add(textBox);
                _prompt.Controls.Add(confirmation);
                _prompt.Controls.Add(textLabel);
                _prompt.AcceptButton = confirmation;
                _prompt.ShowInTaskbar = false;
                _prompt.MaximizeBox = _prompt.MinimizeBox = false;
            }

            textBox.Text = caption;
            textBox.SelectAll();
            _prompt.TopMost = true;
            _prompt.Activate();

            var input = _prompt.ShowDialog() == DialogResult.OK ? _prompt.Controls["textBox"].Text : "";
            return input;
        }
    }
}