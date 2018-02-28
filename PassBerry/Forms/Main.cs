using PassBerry.Handler;
using PassBerry.Library;
using PassBerry.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Security;
using System.Windows.Forms;

namespace PassBerry.Forms
{
    public partial class Main : Form
    {
        private static readonly string defalutPasswordPattern = "******";

        public Main()
        {
            this.InitializeComponent();
            this.InitializeMainForm();
        }

        private static List<RecordForDisplay> allDataCache;

        private void InitializeMainForm()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = $"PassBerry {version.Major}.{version.Minor}";

            this.MinimumSize = new Size(700, 500);
            //this.buttonAdd.Image = Properties.;

            var dataGrid = this.dataGridViewMain;
            dataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid.BackgroundColor = Color.White;
            dataGrid.BorderStyle = BorderStyle.None;
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGrid.AllowDrop = false;
            dataGrid.AllowUserToResizeColumns = false;
            dataGrid.AllowUserToResizeRows = false;
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGrid.MultiSelect = false;
            dataGrid.RowHeadersVisible = false;
            dataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGrid.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGrid.DefaultCellStyle.SelectionBackColor = Color.Black;
            dataGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGrid.RowTemplate.Height = 50;
            dataGrid.RowTemplate.DefaultCellStyle.Padding = new Padding(10);

            allDataCache = ResourceProcessor.GetInstance().GetAll();
            this.LoadData(allDataCache);
        }

        private void LoadData(List<RecordForDisplay> data)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("PictureForLogo", typeof(Image));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Remarks", typeof(string));
            dataTable.Columns.Add("Username", typeof(string));
            dataTable.Columns.Add("PasswordForDisplay", typeof(string));
            dataTable.Columns.Add("Password", typeof(SecureString));
            dataTable.Columns.Add("Keywords", typeof(string));
            dataTable.Columns.Add("Id", typeof(Guid));

            foreach (var item in data)
            {
                dataTable.Rows.Add(
                    item.PictureForLogo,
                    item.Name,
                    item.Remarks,
                    item.Username,
                    defalutPasswordPattern,
                    item.Password,
                    item.Keywords,
                    item.Id);
            }

            var dataGrid = this.dataGridViewMain;
            dataGrid.DataSource = dataTable;

            dataGrid.Columns["Id"].Visible = false;
            dataGrid.Columns["Password"].Visible = false;
            dataGrid.Columns["PasswordForDisplay"].HeaderText = "Password";

            var imageColumn = dataGrid.Columns["PictureForLogo"] as DataGridViewImageColumn;
            imageColumn.HeaderText = "";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imageColumn.Width = 100;

            dataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGrid.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGrid.Columns["Remarks"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGrid.Columns["Keywords"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGrid.Columns["PasswordForDisplay"].CellTemplate.Style.BackColor = Color.LightYellow;
            dataGrid.Columns["PasswordForDisplay"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGrid.Columns["PasswordForDisplay"].CellTemplate.Style.Font = new Font("", 16, FontStyle.Bold);
            dataGrid.Columns["PasswordForDisplay"].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private List<RecordForDisplay> Search(string searchText)
        {
            var result = new List<RecordForDisplay>();
            foreach (var item in allDataCache)
            {
                var bySearch = item.Name + item.Keywords + item.Remarks;
                if (bySearch.ToLowerInvariant().Contains(searchText.ToLowerInvariant()))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private void CopyToClipBoard(object target)
        {
            if (target == null) return;
            Clipboard.SetDataObject(target, true);
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            var searchText = this.textBoxSearch.Text.Trim();
            if (searchText == string.Empty)
            {
                this.LoadData(allDataCache);
            }
            else
            {
                this.LoadData(this.Search(searchText));
            }
        }

        private void dataGridViewMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataGrid = this.dataGridViewMain;
            var targetCell = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            
            // If it is password field, show the characters
            if (e.ColumnIndex == dataGrid.Columns["PasswordForDisplay"].Index)
            {
                if (targetCell.Value.ToString() == defalutPasswordPattern)
                {
                    var passwordIndex = dataGrid.Columns["Password"].Index;
                    targetCell.Value =
                        SecureStringHelper.GetStringFromSecureString(
                            dataGrid.Rows[e.RowIndex].Cells[passwordIndex].Value as SecureString);
                }
                else
                {
                    targetCell.Value = defalutPasswordPattern;
                }
            }

            // Copy current cell's value to clipboard
            this.CopyToClipBoard(targetCell.Value);
        }

        private void tableLayoutPanelFunction_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {

        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {

        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }
    }
}