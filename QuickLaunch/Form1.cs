using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace QuickLaunch
{
    public partial class FormConfig : Form
    {
        public List<Executable> executables;
        
        public FormConfig(List<Executable> executables)
        {
            InitializeComponent();
            this.executables = executables;
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {
            buttonSave.Enabled = false;
            buttonDelete.Enabled = false;
        }

        public void UpdateListBox()
        { 
            listBox_items.Items.Clear();
            foreach (var item in executables)
            {
                listBox_items.Items.Add(item.Title);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string title = textBoxTitle.Text;
            string path = textBoxPath.Text;

            title = title.Trim();
            path = path.Trim();

            title.Replace(";", "");
            path.Replace(";", "");

            path = Path.GetFullPath(path);

            if (title.Length <= 0 || path.Length <= 0)
            {
                MessageBox.Show("You must fill in both title and path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            executables[listBox_items.SelectedIndex] = new Executable(title, path);
            UpdateListBox();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string title = textBoxTitle.Text;
            string path = textBoxPath.Text;

            title = title.Trim();
            path = path.Trim();

            title.Replace(";", "");
            path.Replace(";", "");

            path = Path.GetFullPath(path);

            if (title.Length <= 0 || path.Length <= 0)
            {
                MessageBox.Show("You must fill in both title and path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!executables.Contains(new Executable(title, path)))
            {
                executables.Add(new Executable(title, path));
                UpdateListBox();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int index = listBox_items.SelectedIndex;
            executables.RemoveAt(index);
            listBox_items.Items.RemoveAt(index);
        }

        private void listBox_items_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_items.SelectedIndex >= 0)
            {
                textBoxTitle.Text = executables[listBox_items.SelectedIndex].Title;
                textBoxPath.Text = executables[listBox_items.SelectedIndex].Path;
                buttonDelete.Enabled = true;
                buttonSave.Enabled = true;
            }
            else
            {
                textBoxTitle.Text = "";
                textBoxPath.Text = "";
                buttonDelete.Enabled = false;
                buttonSave.Enabled = false;
            }
        }
    }
}
