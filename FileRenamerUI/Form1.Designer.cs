namespace FileRenamerUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.RootFolderLabel = new System.Windows.Forms.Label();
            this.FolderSelectButton = new System.Windows.Forms.Button();
            this.RepresenterGrid = new System.Windows.Forms.DataGridView();
            this.dataSourceForRepresenterBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.AlbumLabel = new System.Windows.Forms.Label();
            this.AlbumTextBox = new System.Windows.Forms.TextBox();
            this.ArtistTextBox = new System.Windows.Forms.TextBox();
            this.ArtistLabel = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.PatternSelectionLabel = new System.Windows.Forms.Label();
            this.PatternSelectionСomboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.RepresenterGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSourceForRepresenterBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // RootFolderLabel
            // 
            this.RootFolderLabel.AutoSize = true;
            this.RootFolderLabel.Location = new System.Drawing.Point(94, 7);
            this.RootFolderLabel.Name = "RootFolderLabel";
            this.RootFolderLabel.Size = new System.Drawing.Size(0, 13);
            this.RootFolderLabel.TabIndex = 0;
            // 
            // FolderSelectButton
            // 
            this.FolderSelectButton.Location = new System.Drawing.Point(13, 2);
            this.FolderSelectButton.Name = "FolderSelectButton";
            this.FolderSelectButton.Size = new System.Drawing.Size(75, 23);
            this.FolderSelectButton.TabIndex = 1;
            this.FolderSelectButton.Text = "Select folder";
            this.FolderSelectButton.UseVisualStyleBackColor = true;
            this.FolderSelectButton.Click += new System.EventHandler(this.FolderSelectButton_Click);
            // 
            // RepresenterGrid
            // 
            this.RepresenterGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RepresenterGrid.Location = new System.Drawing.Point(13, 31);
            this.RepresenterGrid.Name = "RepresenterGrid";
            this.RepresenterGrid.Size = new System.Drawing.Size(868, 562);
            this.RepresenterGrid.TabIndex = 2;
            // 
            // AlbumLabel
            // 
            this.AlbumLabel.AutoSize = true;
            this.AlbumLabel.Location = new System.Drawing.Point(917, 47);
            this.AlbumLabel.Name = "AlbumLabel";
            this.AlbumLabel.Size = new System.Drawing.Size(39, 13);
            this.AlbumLabel.TabIndex = 3;
            this.AlbumLabel.Text = "Album:";
            // 
            // AlbumTextBox
            // 
            this.AlbumTextBox.Location = new System.Drawing.Point(920, 63);
            this.AlbumTextBox.Name = "AlbumTextBox";
            this.AlbumTextBox.Size = new System.Drawing.Size(170, 20);
            this.AlbumTextBox.TabIndex = 4;
            // 
            // ArtistTextBox
            // 
            this.ArtistTextBox.Location = new System.Drawing.Point(920, 136);
            this.ArtistTextBox.Name = "ArtistTextBox";
            this.ArtistTextBox.Size = new System.Drawing.Size(170, 20);
            this.ArtistTextBox.TabIndex = 6;
            // 
            // ArtistLabel
            // 
            this.ArtistLabel.AutoSize = true;
            this.ArtistLabel.Location = new System.Drawing.Point(917, 120);
            this.ArtistLabel.Name = "ArtistLabel";
            this.ArtistLabel.Size = new System.Drawing.Size(33, 13);
            this.ArtistLabel.TabIndex = 5;
            this.ArtistLabel.Text = "Artist:";
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(920, 569);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(170, 23);
            this.StartButton.TabIndex = 7;
            this.StartButton.Text = "Rename";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // PatternSelectionLabel
            // 
            this.PatternSelectionLabel.AutoSize = true;
            this.PatternSelectionLabel.Location = new System.Drawing.Point(920, 191);
            this.PatternSelectionLabel.Name = "PatternSelectionLabel";
            this.PatternSelectionLabel.Size = new System.Drawing.Size(76, 13);
            this.PatternSelectionLabel.TabIndex = 8;
            this.PatternSelectionLabel.Text = "Select pattern:";
            // 
            // PatternSelectionСomboBox
            // 
            this.PatternSelectionСomboBox.FormattingEnabled = true;
            this.PatternSelectionСomboBox.Location = new System.Drawing.Point(920, 208);
            this.PatternSelectionСomboBox.Name = "PatternSelectionСomboBox";
            this.PatternSelectionСomboBox.Size = new System.Drawing.Size(170, 21);
            this.PatternSelectionСomboBox.TabIndex = 9;
            this.PatternSelectionСomboBox.SelectedIndexChanged += new System.EventHandler(this.SelectPatternComboBox_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1120, 605);
            this.Controls.Add(this.PatternSelectionСomboBox);
            this.Controls.Add(this.PatternSelectionLabel);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.ArtistTextBox);
            this.Controls.Add(this.ArtistLabel);
            this.Controls.Add(this.AlbumTextBox);
            this.Controls.Add(this.AlbumLabel);
            this.Controls.Add(this.RepresenterGrid);
            this.Controls.Add(this.FolderSelectButton);
            this.Controls.Add(this.RootFolderLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.RepresenterGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSourceForRepresenterBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label RootFolderLabel;
        private System.Windows.Forms.Button FolderSelectButton;
        private System.Windows.Forms.DataGridView RepresenterGrid;
        private System.Windows.Forms.BindingSource dataSourceForRepresenterBindingSource;
        private System.Windows.Forms.Label AlbumLabel;
        private System.Windows.Forms.TextBox AlbumTextBox;
        private System.Windows.Forms.TextBox ArtistTextBox;
        private System.Windows.Forms.Label ArtistLabel;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Label PatternSelectionLabel;
        private System.Windows.Forms.ComboBox PatternSelectionСomboBox;

    }
}

