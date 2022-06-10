namespace SymX_UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void AddFileButton_Click(object sender, EventArgs e)
        {
            AddFileForm aff = new AddFileForm();
            aff.Show();
        }
    }
}