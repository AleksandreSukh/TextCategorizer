using System;
using System.Windows.Forms;
using TextAnalyser;

namespace TextCategoryEvaluatorGui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static TextCategoryEvaluatorEntry evaluatorEntry = new TextCategoryEvaluatorEntry();

        private void Form1_Load(object sender, EventArgs e)
        {
            evaluatorEntry.LoadXmlsIntoCategorisedSampleChainsList();
        }

        private void btnEvaluate_Click(object sender, EventArgs e)
        {
            var text = richTextBox1.Text;
            try
            {
                var result = evaluatorEntry.EvaluateTextCategory(text);
                MessageBox.Show($"I think text category is:{result}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Category could not be evaluated:{ex}");
            }
        }
    }
}
