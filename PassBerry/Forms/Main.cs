namespace PassBerry.Forms
{
    using PassBerry.Handler;
    using PassBerry.Library;
    using PassBerry.Model;
    using PassBerry.Properties;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Reflection;
    using System.Security;
    using System.Windows.Forms;

    public partial class Main : Form
    {
        private static readonly string defalutPasswordPattern = "******";

        public Main()
        {
            this.InitializeComponent();
            this.InitializeMainForm();
            this.InitializeNotifyIcon();
        }

        private static List<RecordForDisplay> allDataCache;

        private void InitializeMainForm()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = $"PassBerry {version.Major}.{version.Minor} - Press Shift + F2 to paste username and password";
            this.MinimumSize = new Size(700, 500);

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

        private void InitializeNotifyIcon()
        {
            var notifyIcon = new NotifyIcon { Icon = this.Icon, Visible = true };
            this.Closing += delegate
            {
                HotKeyHelper.UnregisterHotKey(this.Handle, 100);
                HotKeyHelper.UnregisterHotKey(this.Handle, 101);
            };

            notifyIcon.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Activate();
                    this.Visible = true;
                }
                if (e.Button == MouseButtons.Right)
                {
                    if (MessageBox.Show("Are your sure to exit the PassBerry?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.Close();
                    }
                }
            };
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
                    item.PictureForLogo == null ? Resources.acorn_40px : item.PictureForLogo,
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

        #region Register Hotkey and reset Close and Min Window function

        private const int WmHotkey = 0x0312;
        private const int WmSysCommand = 0x0112;
        private const int ScMaxMize = 0xf030;
        private const int ScMinMize = 0xf020;
        private const int ScClose = 0xf060;

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case WmSysCommand:
                    var code = message.WParam.ToInt32();
                    if (code == ScClose || code == ScMinMize)
                    {
                        this.Visible = false;
                        // Must Prevent WndProc
                        return;
                    }
                    // Others, such as SC_MAXMIZE must in WndProc.
                    break;

                case WmHotkey:
                    switch (message.WParam.ToInt32())
                    {
                        // Shift + F2
                        case 100:
                            var text = this.GetCurrentPasteValue();
                            if (!string.IsNullOrEmpty(text))
                            {
                                Clipboard.SetText(text);
                                SendKeys.Send("^V");
                                // Clipboard.Clear();
                            }
                            break;

                        // Shift + F1
                        case 101:
                            this.Activate();
                            this.Visible = true;
                            break;
                    }
                    break;
            }
            base.WndProc(ref message);
        }

        private bool IsCurrentPasteUsername = true;

        private string GetCurrentPasteValue()
        {
            if (this.dataGridViewMain.SelectedRows.Count < 1) { return null; }
            var currentSelectedItem = (Guid)this.dataGridViewMain.SelectedRows[0].Cells["Id"].Value;
            var record = allDataCache.Find(i => i.Id == currentSelectedItem);
            if (IsCurrentPasteUsername)
            {
                this.IsCurrentPasteUsername = false;
                if (string.IsNullOrWhiteSpace(record.Username)) { return null; }
                return record.Username;
            }
            else
            {
                this.IsCurrentPasteUsername = true;
                if (record.Password == null) { return null; }
                return SecureStringHelper.GetStringFromSecureString(record.Password);
            }
        }

        private void CopyToClipBoard(object target)
        {
            if (target == null) return;
            Clipboard.SetDataObject(target, true);
        }

        #endregion Register Hotkey and reset Close and Min Window function

        #region Event Handler

        private void Main_Load(object sender, EventArgs e)
        {
            // Register Hotkey: Shift + F2
            // ID: 100
            HotKeyHelper.RegisterHotKey(this.Handle, 100, HotKeyHelper.KeyModifiers.Shift, Keys.F2);

            // Register Hotkey: Shift + F1
            // ID: 101
            HotKeyHelper.RegisterHotKey(this.Handle, 101, HotKeyHelper.KeyModifiers.Shift, Keys.F1);
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

        private void dataGridViewMain_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            this.IsCurrentPasteUsername = true;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var result = new Edit(null).ShowDialog();
            if (result == DialogResult.OK)
            {
                allDataCache = ResourceProcessor.GetInstance().GetAll();
                this.LoadData(allDataCache);
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewMain.SelectedRows.Count < 1) { return; }
            var currentSelectedItem = (Guid)this.dataGridViewMain.SelectedRows[0].Cells["Id"].Value;
            var record = allDataCache.Find(i => i.Id == currentSelectedItem);
            var result = new Edit(record).ShowDialog();
            if (result == DialogResult.OK)
            {
                allDataCache = ResourceProcessor.GetInstance().GetAll();
                this.LoadData(allDataCache);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewMain.SelectedRows.Count < 1) { return; }
            var currentSelectedItem = (Guid)this.dataGridViewMain.SelectedRows[0].Cells["Id"].Value;
            var confirmResult = MessageBox.Show("Are you sure to delete the current item?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (confirmResult == DialogResult.Yes)
            {
                ResourceProcessor.GetInstance().Delete(currentSelectedItem);
                allDataCache = ResourceProcessor.GetInstance().GetAll();
                this.LoadData(allDataCache);
            }
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        #endregion Event Handler
    }
}