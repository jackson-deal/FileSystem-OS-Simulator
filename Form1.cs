// Form1.cs
using System;
using System.IO;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace CS3502_P3_FileSystem_JacksonDeal
{
    public partial class Form1 : Form
    {
        private FileService _fileService;
        private bool _isDirty = false;
        private string _currentFilePath = null;

        public Form1()
        {
            InitializeComponent();
            _fileService = new FileService();

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            rtFileContent.TextChanged += (s, e) => _isDirty = true;
            lstContents.SelectedIndexChanged += LstContents_SelectedIndexChanged;

            string startPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            txtPath.Text = startPath;
            LoadDirectory(startPath);

            btnGo.Click += (s, e) => LoadDirectory(txtPath.Text);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) btnRename.PerformClick();
            else if (e.KeyCode == Keys.Delete) btnDelete.PerformClick();
            else if (e.Control && e.KeyCode == Keys.S) btnUpdate.PerformClick();
            else if (e.KeyCode == Keys.Back && !rtFileContent.Focused) btnBack.PerformClick();
        }

        private bool PromptUnsavedChanges()
        {
            if (!_isDirty) return true;

            var result = MessageBox.Show("You have unsaved changes. Discard them?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                _isDirty = false;
                return true;
            }
            return false;
        }

        private void LoadDirectory(string path)
        {
            if (!PromptUnsavedChanges()) return;

            try
            {
                if (!Directory.Exists(path))
                {
                    statusLabel.Text = "● Error: Path not found (ENOENT)";
                    return;
                }

                lstContents.Items.Clear();

                var items = _fileService.ListDirectory(path);
                foreach (var item in items)
                {
                    lstContents.Items.Add(item);
                }

                rtFileContent.Clear();
                lblMetadata.Text = "Metadata: None";
                _currentFilePath = null;
                _isDirty = false;

                statusLabel.Text = "● Current Status: Ready";
                statusItems.Text = $"{lstContents.Items.Count} Items";
            }
            catch (UnauthorizedAccessException)
            {
                statusLabel.Text = "● Error: Permission Denied (EACCES)";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"● Error: {ex.Message}";
            }
        }

        private void LstContents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstContents.SelectedItem is FileSystemEntry entry)
            {
                try
                {
                    lblMetadata.Text = "Metadata: " + _fileService.GetMetadata(entry.FullPath, entry.IsDirectory);
                }
                catch
                {
                    lblMetadata.Text = "Metadata: Unavailable";
                }
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (lstContents.SelectedItem is not FileSystemEntry entry)
            {
                statusLabel.Text = "● Please select a file or directory.";
                return;
            }

            if (!PromptUnsavedChanges()) return;

            try
            {
                if (entry.IsDirectory)
                {
                    // Clear and lock the editor when entering a directory
                    rtFileContent.Clear();
                    rtFileContent.ReadOnly = true;
                    _isDirty = false;
                    _currentFilePath = null;

                    txtPath.Text = entry.FullPath;
                    LoadDirectory(entry.FullPath);
                    statusLabel.Text = "● Navigated into: " + entry.Name;
                }
                else
                {
                    // Existing file logic
                    rtFileContent.Text = _fileService.ReadFile(entry.FullPath);
                    rtFileContent.ReadOnly = false; // Enable editing for files
                    _currentFilePath = entry.FullPath;
                    _isDirty = false;
                    statusLabel.Text = $"● Read: {entry.Name}";
                }
            }
            catch (UnauthorizedAccessException)
            {
                statusLabel.Text = "● Error: Permission Denied (EACCES)";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"● Read Error: {ex.Message}";
            }
        }

        private void btnNewFolder_Click(object sender, EventArgs e) => CreateItem(true);
        private void btnNewFile_Click(object sender, EventArgs e) => CreateItem(false);

        private void CreateItem(bool isDirectory)
        {
            string promptTitle = isDirectory ? "Create Directory" : "Create File";
            string defaultName = isDirectory ? "NewFolder" : "newfile.txt";

            string itemName = Microsoft.VisualBasic.Interaction.InputBox($"Enter new {(isDirectory ? "directory" : "file")} name:",
                promptTitle, defaultName);

            if (string.IsNullOrWhiteSpace(itemName)) return;

            string fullPath = Path.Combine(txtPath.Text, itemName);

            try
            {
                if (isDirectory)
                {
                    if (Directory.Exists(fullPath))
                    {
                        statusLabel.Text = "● Error: Directory already exists (EEXIST)";
                        return;
                    }
                    _fileService.CreateDirectory(fullPath);
                    statusLabel.Text = $"● Directory Created: {itemName}";
                }
                else
                {
                    if (File.Exists(fullPath))
                    {
                        statusLabel.Text = "● Error: File already exists (EEXIST)";
                        return;
                    }
                    _fileService.CreateFile(fullPath);
                    statusLabel.Text = $"● File Created: {itemName}";
                }

                LoadDirectory(txtPath.Text);
            }
            catch (UnauthorizedAccessException)
            {
                statusLabel.Text = "● Error: Permission Denied (EACCES)";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"● Create Error: {ex.Message}";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstContents.SelectedItem is not FileSystemEntry entry)
            {
                statusLabel.Text = "● Please select an item to delete.";
                return;
            }

            DialogResult confirm = MessageBox.Show($"Are you sure you want to delete {entry.Name}?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _fileService.DeleteItem(entry.FullPath, entry.IsDirectory);
                statusLabel.Text = $"● Deleted: {entry.Name}";

                if (_currentFilePath == entry.FullPath)
                {
                    rtFileContent.Clear();
                    _currentFilePath = null;
                    _isDirty = false;
                }

                LoadDirectory(txtPath.Text);
            }
            catch (UnauthorizedAccessException)
            {
                statusLabel.Text = "● Error: Permission Denied (EACCES)";
            }
            catch (IOException)
            {
                statusLabel.Text = "● Error: Item in use or directory not empty (EBUSY)";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"● Delete Error: {ex.Message}";
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (lstContents.SelectedItem is not FileSystemEntry entry)
            {
                statusLabel.Text = "● Please select an item to rename.";
                return;
            }

            string newName = Microsoft.VisualBasic.Interaction.InputBox($"Enter new name for {entry.Name}:", "Rename", entry.Name);

            if (string.IsNullOrWhiteSpace(newName) || newName == entry.Name) return;

            string newPath = Path.Combine(txtPath.Text, newName);

            try
            {
                _fileService.RenameItem(entry.FullPath, newPath, entry.IsDirectory);
                statusLabel.Text = $"● Renamed: {entry.Name} -> {newName}";

                if (_currentFilePath == entry.FullPath) _currentFilePath = newPath;

                LoadDirectory(txtPath.Text);
            }
            catch (IOException ex) when (ex.Message.Contains("already exists"))
            {
                statusLabel.Text = "● Error: Target name already exists (EEXIST)";
            }
            catch (UnauthorizedAccessException)
            {
                statusLabel.Text = "● Error: Permission Denied (EACCES)";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"● Rename Error: {ex.Message}";
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                statusLabel.Text = "● Error: No active file to save.";
                return;
            }

            try
            {
                _fileService.UpdateFile(_currentFilePath, rtFileContent.Text);
                statusLabel.Text = $"● Updated: {Path.GetFileName(_currentFilePath)} successfully.";
                _isDirty = false;

                btnUpdate.Enabled = false;
                Timer t = new Timer { Interval = 1000 };
                t.Tick += (s, ev) => { btnUpdate.Enabled = true; t.Stop(); };
                t.Start();
            }
            catch (UnauthorizedAccessException)
            {
                statusLabel.Text = "● Error: Access Denied (EACCES) - File might be Read-Only.";
            }
            catch (IOException)
            {
                statusLabel.Text = "● Error: Disk full or file locked (EIO/EBUSY).";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"● Update Error: {ex.Message}";
            }
        }

        private void lstContents_MouseDoubleClick(object sender, MouseEventArgs e) => btnRead_Click(sender, e);

        private void btnBack_Click(object sender, EventArgs e)
        {
            DirectoryInfo parentDir = Directory.GetParent(txtPath.Text);

            if (parentDir != null)
            {
                if (!PromptUnsavedChanges()) return;

                txtPath.Text = parentDir.FullName;
                LoadDirectory(parentDir.FullName);
                statusLabel.Text = "● Moved up to: " + parentDir.Name;
            }
            else
            {
                statusLabel.Text = "● Error: Already at the root directory.";
            }
        }
    }
}