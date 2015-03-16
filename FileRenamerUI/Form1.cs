using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using FileRenamer;
using System.Collections.Generic;
using System.Linq;

namespace FileRenamerUI
{
    
    public partial class Form1 : Form
    {
        private readonly ViewModel _viewModel;
        DataTable _table;

        public Form1()
        {
            InitializeComponent();
            
            _viewModel = new ViewModel();
            Bind(_viewModel);

            var bindingSource = new BindingSource();
            _table = new DataTable();
            bindingSource.DataSource = _table;
            RepresenterGrid.DataSource = bindingSource;
        }

        private void Bind(ViewModel viewModel)
        {
            viewModel.OnRootFolderChanged += value => RootFolderLabel.Text = value;
            viewModel.OnRootFolderChanged += LoadFolder;
            viewModel.OnPatternCollectionChanged += ChangePatterns;
        }

        private void ChangePatterns(IEnumerable<string> patterns)
        {
            throw new NotImplementedException();
        }

        private void LoadFolder(string value)
        {
            _viewModel.LoadFolder(_table, value);
            AlbumTextBox.Text = _viewModel.Album;
            ArtistTextBox.Text = _viewModel.Artist;
        }

        private void FolderSelectButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.ShowDialog();
            _viewModel.RootFolder = dialog.SelectedPath;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            _viewModel.Rename();
        }

        private void SelectPatternComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _viewModel.SelectPattern(PatternSelectionСomboBox.SelectedText);
        }
    }
}
