namespace PassBerry.Forms
{
    using PassBerry.Handler;
    using PassBerry.Library;
    using PassBerry.Model;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class Edit : Form
    {
        private RecordForDisplay record;

        public Edit(RecordForDisplay record)
        {
            this.record = record;
            this.InitializeComponent();
            this.InitializeForm();
        }

        private void InitializeForm()
        {
            if (this.record == null)
            {
                this.Text = "Create";
            }
            else
            {
                this.Text = $"Edit - {this.record.Name}";
                this.textBoxName.Text = this.record.Name;
                this.pictureBoxLogo.Image = this.record.PictureForLogo;
                this.textBoxLogoFilePath.Text = string.Empty;
                this.textBoxUsername.Text = this.record.Username;
                this.textBoxPassword.Text = SecureStringHelper.GetStringFromSecureString(this.record.Password);
                this.textBoxKeywords.Text = this.record.Keywords;
                this.textBoxRemarks.Text = this.record.Remarks;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxName.Text))
            {
                MessageBox.Show("Please select a valid name for current item.", "Save Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Image logo = null;
            var logoFilePath = this.textBoxLogoFilePath.Text;
            if (!string.IsNullOrWhiteSpace(logoFilePath))
            {
                try
                {
                    logo = Image.FromFile(logoFilePath);
                    logo = ImageHelper.ZoomPicture(logo, 100, 50);
                }
                catch
                {
                    MessageBox.Show("Please select a valid image for Logo.", "Save Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (this.record == null)
            {
                this.record = new RecordForDisplay { Id = Guid.NewGuid() };
            }

            this.record.Name = this.textBoxName.Text;
            var image = this.textBoxLogoFilePath.Text;
            this.record.PictureForLogo = logo;
            this.record.Username = this.textBoxUsername.Text;
            this.record.Password = SecureStringHelper.GetSecureStringFromString(this.textBoxPassword.Text);
            this.record.Keywords = this.textBoxKeywords.Text;
            this.record.Remarks = this.textBoxRemarks.Text;

            ResourceProcessor.GetInstance().Save(this.record);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonSelectLogo_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "Please select an image file"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = dialog.FileName;
                this.textBoxLogoFilePath.Text = filePath;
                this.pictureBoxLogo.Image = Image.FromFile(filePath);
            }
        }
    }
}