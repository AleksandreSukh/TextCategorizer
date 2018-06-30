using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Microsoft.Win32;

namespace MarkovChainGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextMarkovChains.MultiDeepMarkovChain multi = new TextMarkovChains.MultiDeepMarkovChain(4);
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEntry_Click(object sender, RoutedEventArgs e)
        {
            multi.Feed(txtEntry.Text);
            txtEntry.Text = string.Empty;
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if(multi.ReadyToGenerate())
                txtOutput.Text = multi.GenerateSentence();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "xml";
            Nullable<bool> saved = sfd.ShowDialog();
            if (saved == true)
            {
                multi.Save(sfd.FileName);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            Nullable<bool> selected = ofd.ShowDialog();
            if (selected == true)
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(ofd.FileName);
                multi.Feed(xd);
            }
        }

        private void btnTypingTest_Click(object sender, RoutedEventArgs e)
        {
            predictiveText();
        }

        private void predictiveText()
        {
            List<string> test = multi.GetNextLikelyWord(txtTypingTest.Text.Trim());
            StringBuilder sb = new StringBuilder();
            foreach (string s in test)
                sb.Append(s).Append(" ");
            txtTypingTestOutput.Text = sb.ToString();
        }

        private void txtTypingTest_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                predictiveText();
        }
    }
}
