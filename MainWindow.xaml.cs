using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WordProcessor
{
    public partial class MainWindow : Window
    {
        private string currentFilePath;

        public MainWindow()
        {
            InitializeComponent();
            LoadFontFamilies();
        }

        private void LoadFontFamilies()
        {
            foreach (var fontFamily in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
            {
                FontFamilyComboBox.Items.Add(new ComboBoxItem { Content = fontFamily.Source });
                FontFamilyComboBoxToolbar.Items.Add(new ComboBoxItem { Content = fontFamily.Source });
            }
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab("New Document", true);
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName;
                string fileContent = File.ReadAllText(currentFilePath);
                AddNewTab(Path.GetFileName(currentFilePath), true, fileContent);
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem tabItem && tabItem.Content is RichTextBox richTextBox)
            {
                if (currentFilePath == null)
                {
                    SaveAsFile_Click(sender, e);
                }
                else
                {
                    SaveToFile(currentFilePath, richTextBox);
                }
            }
        }

        private void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem tabItem && tabItem.Content is RichTextBox richTextBox)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|Word documents (*.docx)|*.docx|PDF files (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    currentFilePath = saveFileDialog.FileName;
                    SaveToFile(currentFilePath, richTextBox);
                    tabItem.Header = Path.GetFileName(currentFilePath);
                }
            }
        }

        private void SaveToFile(string filePath, RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            string fileExtension = Path.GetExtension(filePath);

            switch (fileExtension.ToLower())
            {
                case ".txt":
                    File.WriteAllText(filePath, textRange.Text);
                    break;
                case ".docx":
                    // Implement DOCX saving logic here
                    break;
                case ".pdf":
                    // Implement PDF saving logic here
                    break;
                default:
                    File.WriteAllText(filePath, textRange.Text);
                    break;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            ToggleFontWeight();
        }

        private void Italic_Click(object sender, RoutedEventArgs e)
        {
            ToggleFontStyle();
        }

        private void Underline_Click(object sender, RoutedEventArgs e)
        {
            ToggleTextDecoration();
        }

        private void ToggleFontWeight()
        {
            if (MainTabControl.SelectedItem is TabItem tabItem && tabItem.Content is RichTextBox richTextBox)
            {
                if (richTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty) is FontWeight fontWeight && fontWeight == FontWeights.Bold)
                {
                    richTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);
                }
                else
                {
                    richTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
                }
            }
        }

        private void ToggleFontStyle()
        {
            if (MainTabControl.SelectedItem is TabItem tabItem && tabItem.Content is RichTextBox richTextBox)
            {
                if (richTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty) is FontStyle fontStyle && fontStyle == FontStyles.Italic)
                {
                    richTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
                }
                else
                {
                    richTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
                }
            }
        }

        private void ToggleTextDecoration()
        {
            if (MainTabControl.SelectedItem is TabItem tabItem && tabItem.Content is RichTextBox richTextBox)
            {
                if (richTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) == TextDecorations.Underline)
                {
                    richTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                }
                else
                {
                    richTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
                }
            }
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem tabItem && tabItem.Content is RichTextBox richTextBox)
            {
                string fontFamily = null;

                if (FontFamilyComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    fontFamily = selectedItem.Content.ToString();
                }
                else if (FontFamilyComboBoxToolbar.SelectedItem is ComboBoxItem selectedItemToolbar)
                {
                    fontFamily = selectedItemToolbar.Content.ToString();
                }

                if (fontFamily != null)
                {
                    richTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, new FontFamily(fontFamily));
                }
            }
        }

       
        private void IncreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            AdjustFontSize(2);
        }

        private void DecreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            AdjustFontSize(-2);
        }

        private void AdjustFontSize(double delta)
        {
            if (MainTabControl.SelectedItem is TabItem tabItem && tabItem.Content is RichTextBox richTextBox)
            {
                if (richTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty) is double currentSize)
                {
                    double newSize = currentSize + delta;
                    richTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, newSize);

                   
                }
            }
        }

      
        private void AddNewTab(string header, bool canClose, string content = "")
        {
            TabItem newTab = new TabItem();
            StackPanel headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            headerPanel.Children.Add(new TextBlock { Text = header });

            if (canClose)
            {
                Button closeButton = new Button { Content = "x", Width = 20, Height = 20 };
                closeButton.Click += (s, e) => CloseTab(newTab);
                headerPanel.Children.Add(closeButton);
            }

            newTab.Header = headerPanel;
            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(content)));
            newTab.Content = richTextBox;

            MainTabControl.Items.Insert(MainTabControl.Items.Count - 1, newTab);
            MainTabControl.SelectedItem = newTab;
        }

        private void CloseTab(TabItem tabItem)
        {
            if (MessageBox.Show("Are you sure you want to close this tab?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MainTabControl.Items.Remove(tabItem);
            }
        }

        private void AddNewTab_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab("New Document", true);
        }
    }
}