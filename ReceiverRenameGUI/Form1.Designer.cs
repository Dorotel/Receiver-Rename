namespace ReceiverRenameGUI;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        textBoxFolder = new TextBox();
        buttonBrowse = new Button();
        textBoxFormat = new TextBox();
        buttonProcess = new Button();
        progressBar = new ProgressBar();
        listBoxMessages = new ListBox();
        labelFolder = new Label();
        labelFormat = new Label();
        tableLayoutPanel1 = new TableLayoutPanel();
        helpButton = new Button();
        tableLayoutPanel1.SuspendLayout();
        SuspendLayout();
        // 
        // textBoxFolder
        // 
        textBoxFolder.Dock = DockStyle.Fill;
        textBoxFolder.Location = new Point(129, 3);
        textBoxFolder.Name = "textBoxFolder";
        textBoxFolder.Size = new Size(492, 23);
        textBoxFolder.TabIndex = 1;
        // 
        // buttonBrowse
        // 
        buttonBrowse.Location = new Point(627, 3);
        buttonBrowse.Name = "buttonBrowse";
        buttonBrowse.Size = new Size(75, 25);
        buttonBrowse.TabIndex = 2;
        buttonBrowse.Text = "Browse";
        buttonBrowse.UseVisualStyleBackColor = true;
        buttonBrowse.Click += buttonBrowse_Click;
        // 
        // textBoxFormat
        // 
        textBoxFormat.Dock = DockStyle.Fill;
        textBoxFormat.Location = new Point(129, 34);
        textBoxFormat.Name = "textBoxFormat";
        textBoxFormat.Size = new Size(492, 23);
        textBoxFormat.TabIndex = 4;
        textBoxFormat.Text = "{DATE} {PO} {RECEIVER}";
        // 
        // buttonProcess
        // 
        buttonProcess.Dock = DockStyle.Fill;
        buttonProcess.Location = new Point(3, 65);
        buttonProcess.Name = "buttonProcess";
        buttonProcess.Size = new Size(120, 30);
        buttonProcess.TabIndex = 5;
        buttonProcess.Text = "Process Files";
        buttonProcess.UseVisualStyleBackColor = true;
        buttonProcess.Click += buttonProcess_Click;
        // 
        // progressBar
        // 
        tableLayoutPanel1.SetColumnSpan(progressBar, 2);
        progressBar.Dock = DockStyle.Fill;
        progressBar.Location = new Point(129, 65);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(573, 30);
        progressBar.TabIndex = 6;
        // 
        // listBoxMessages
        // 
        tableLayoutPanel1.SetColumnSpan(listBoxMessages, 3);
        listBoxMessages.Dock = DockStyle.Fill;
        listBoxMessages.FormattingEnabled = true;
        listBoxMessages.ItemHeight = 15;
        listBoxMessages.Location = new Point(3, 101);
        listBoxMessages.Name = "listBoxMessages";
        listBoxMessages.Size = new Size(699, 300);
        listBoxMessages.TabIndex = 7;
        // 
        // labelFolder
        // 
        labelFolder.AutoSize = true;
        labelFolder.Dock = DockStyle.Fill;
        labelFolder.Location = new Point(3, 0);
        labelFolder.Name = "labelFolder";
        labelFolder.Size = new Size(120, 31);
        labelFolder.TabIndex = 0;
        labelFolder.Text = "Scans Folder:";
        labelFolder.TextAlign = ContentAlignment.MiddleRight;
        // 
        // labelFormat
        // 
        labelFormat.AutoSize = true;
        labelFormat.Dock = DockStyle.Fill;
        labelFormat.Location = new Point(3, 31);
        labelFormat.Name = "labelFormat";
        labelFormat.Size = new Size(120, 31);
        labelFormat.TabIndex = 3;
        labelFormat.Text = "File Format:";
        labelFormat.TextAlign = ContentAlignment.MiddleRight;
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 3;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel1.Controls.Add(helpButton, 2, 1);
        tableLayoutPanel1.Controls.Add(buttonBrowse, 2, 0);
        tableLayoutPanel1.Controls.Add(listBoxMessages, 0, 3);
        tableLayoutPanel1.Controls.Add(labelFormat, 0, 1);
        tableLayoutPanel1.Controls.Add(labelFolder, 0, 0);
        tableLayoutPanel1.Controls.Add(buttonProcess, 0, 2);
        tableLayoutPanel1.Controls.Add(textBoxFormat, 1, 1);
        tableLayoutPanel1.Controls.Add(progressBar, 1, 2);
        tableLayoutPanel1.Controls.Add(textBoxFolder, 1, 0);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 4;
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.Size = new Size(705, 404);
        tableLayoutPanel1.TabIndex = 8;
        // 
        // helpButton
        // 
        helpButton.Location = new Point(627, 34);
        helpButton.Name = "helpButton";
        helpButton.Size = new Size(75, 25);
        helpButton.TabIndex = 8;
        helpButton.Text = "Help";
        helpButton.UseVisualStyleBackColor = true;
        helpButton.Click += helpButton_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(705, 404);
        Controls.Add(tableLayoutPanel1);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "Form1";
        Text = "PDF Receiver Rename Tool";
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TextBox textBoxFolder;
    private System.Windows.Forms.Button buttonBrowse;
    private System.Windows.Forms.TextBox textBoxFormat;
    private System.Windows.Forms.Button buttonProcess;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.ListBox listBoxMessages;
    private System.Windows.Forms.Label labelFolder;
    private System.Windows.Forms.Label labelFormat;
    private TableLayoutPanel tableLayoutPanel1;
    private Button helpButton;
}
