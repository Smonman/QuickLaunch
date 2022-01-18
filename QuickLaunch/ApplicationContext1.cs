using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QuickLaunch
{
    internal partial class ApplicationContext1 : ApplicationContext
    {
        private NotifyIcon notifyIcon;
        private FormConfig formConfig;
        private ContextMenuStrip cms;

        public ApplicationContext1()
        {
            InitResourceFolder();
            Init();
        }

        private void Init()
        {
            List<Executable> executables = LoadResourceStrings();

            formConfig = new FormConfig(executables);
            formConfig.UpdateListBox();
            formConfig.Hide();

            cms = GenerateContextMenuStrip(executables);

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Resources.Icons.Icon;
            notifyIcon.Text = "QuickLaunch";
            notifyIcon.ContextMenuStrip = cms;
            notifyIcon.Visible = true;
        }

        private List<Executable> LoadResourceStrings()
        {
            string p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuickLaunch", "items.txt");

            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(p));

            if (di.Exists == false || File.Exists(p) == false)
            {
                MessageBox.Show("Could not load saved items.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            string[] lines = File.ReadAllLines(p, System.Text.Encoding.UTF8);

            List<Executable> executables = new List<Executable>();
            foreach (string line in lines)
            {
                executables.Add(Executable.FromSaveString(line));
            }

            return executables;
        }

        private void SaveResourceStrings(List<Executable> executables)
        {
            string p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuickLaunch", "items.txt");

            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(p));

            if (di.Exists == false)
            {
                di.Create();
            }

            if (File.Exists(p) == false)
            {
                string tempFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "items.txt");
                File.Create(tempFile).Close();
                File.Copy(tempFile, p, true);
                File.Delete(tempFile);
            }

            string[] lines = new string[executables.Count];

            for(int i = 0; i < lines.Length; i++)
            {
                lines[i] = executables[i].SaveString();
            }

            File.WriteAllLines(p, lines);
        }

        private void InitResourceFolder()
        {
            string p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuickLaunch", "items.txt");

            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(p));

            if (di.Exists == false)
            {
                di.Create();
            }

            if (File.Exists(p) == false)
            {
                string tempFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "items.txt");
                File.Create(tempFile).Close();
                File.Copy(tempFile, p, true);
                File.Delete(tempFile);
            }
        }

        private ToolStripMenuItem[] GenerateMenuItems(List<Executable> executables)
        {
            List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
            foreach (Executable executable in executables)
            {
                ToolStripMenuItem m = new ToolStripMenuItem();
                m.Text = executable.Title;
                m.Tag = executable.Path;
                m.Click += StartExecutable;

                try
                {
                    Icon icon = Icon.ExtractAssociatedIcon(executable.Path);
                    if (icon != null)
                    {
                        m.Image = (Image)icon.ToBitmap();
                    }
                }
                catch (FileNotFoundException e)
                { 
                    MessageBox.Show("Could not extract the icon from this process:\n" + e.Message + "\nNo icon will be added.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    m.Image = null;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                items.Add(m);
            }

            return items.ToArray();
        }

        private ContextMenuStrip GenerateContextMenuStrip(List<Executable> executables)
        {
            ContextMenuStrip temp = new ContextMenuStrip();
            temp.Items.AddRange(GenerateMenuItems(executables));

            temp.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem menuItemQuit = new ToolStripMenuItem();
            menuItemQuit.Text = "&Quit";
            menuItemQuit.Click += Quit;

            ToolStripMenuItem menuItemConfig = new ToolStripMenuItem();
            menuItemConfig.Text = "&Config";
            menuItemConfig.Click += Config;

            temp.Items.Add(menuItemConfig);
            temp.Items.Add(menuItemQuit);

            return temp;
        }

        private void UpdateContextMenuStrip(List<Executable> executables)
        {
            int c = cms.Items.Count - 3;
            for (int i = 0; i < c; i++)
            {
                cms.Items.RemoveAt(0);
            }

            ToolStripMenuItem[] items = GenerateMenuItems(executables);

            for (int i = 0; i < items.Length; i++)
            {
                cms.Items.Insert(i, items[i]);
            }
        }

        private void Quit(Object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void Config(Object sender, EventArgs e)
        {
            if (formConfig.Visible)
            {
                formConfig.Activate();
            }
            else
            {
                // when the user closes the config form
                if (formConfig.ShowDialog() == DialogResult.Cancel)
                {
                    SaveResourceStrings(formConfig.executables);
                    UpdateContextMenuStrip(LoadResourceStrings());
                }
            }
        }

        private void StartExecutable(Object sender, EventArgs e)
        {
            ToolStripMenuItem m = (ToolStripMenuItem)sender;
            try
            {
                Process.Start((string)m.Tag);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
