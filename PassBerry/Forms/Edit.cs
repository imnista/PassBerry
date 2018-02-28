using PassBerry.Library;
using PassBerry.Model;
using System.Windows.Forms;

namespace PassBerry.Forms
{
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
                this.textBoxLogoFilePath.Text = string.Empty;
                this.textBoxUsername.Text = this.record.Username;
                this.textBoxPassword.Text = SecureStringHelper.GetStringFromSecureString(this.record.Password);
                this.textBoxKeywords.Text = this.record.Keywords;
                this.textBoxRemarks.Text = this.record.Remarks;
            }
        }
    }
}