namespace SymX_UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddFileForm aff = new AddFileForm();
            aff.Activate();
        }
    }
}