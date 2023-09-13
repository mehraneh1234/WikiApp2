using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WikiApplication
{
    public partial class WikiForm : Form
    {
       
        List<Information> information = new List<Information>();
        
       // private string selectedStructure = "Linear"; // Default value
        
        public WikiForm()
        {
            InitializeComponent();
            
        }

        private void InitializeListView()
        {
            listViewData.View = View.Details;
            listViewData.Columns.Add("Name");
            listViewData.Columns.Add("Category");
        }

        private void DisplayListViewData()
        {
            listViewData.Items.Clear();
            foreach (Information info in information)
            {
                if (!string.IsNullOrEmpty(info.GetName()) &&
                    !string.IsNullOrEmpty(info.GetCategory()) &&
                    !string.IsNullOrEmpty(info.GetStructure()) &&
                    !string.IsNullOrEmpty(info.GetDefinition()))
                {
                    ListViewItem item = new ListViewItem(info.GetName());
                    item.SubItems.Add(info.GetCategory());
                    item.SubItems.Add(info.GetStructure());
                    item.SubItems.Add(info.GetDefinition());
                    
                    // Add the row to the ListView
                    listViewData.Items.Add(item);
                }
            }
        }

        private void ClearTextBox()
        {
            txtName.Clear();
            txtDefinition.Clear();
            radioButtonLinear.Checked = false;
            radioButtonNonLinear.Checked = false;
            comboBoxCategory.Text = "";
            txtName.Focus();
        }
        #region Category read, save, selected index change
        private List<string> ReadCategories(string filePath)
        {
            List<string> categories = new List<string>();

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    categories.AddRange(lines);
                    comboBoxCategory.DataSource = categories;
                   // comboBoxCategory.DisplayMember = "CategoryName";
                    comboBoxCategory.Refresh();
                }
                else
                {
                    MessageBox.Show($"Categories file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return categories;
        }
       
        private void SaveCategories(string filePath, string[] categories)
        {
            try
            {
                File.WriteAllLines(filePath, categories);
                StatusStripDataStr.Items.Add("Categories saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
      

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            
            if (listViewData.SelectedItems.Count > 0)
            {
                Information selectedInformation = GetSelectedInformation();
                selectedInformation.SetCategory(comboBoxCategory.SelectedItem.ToString());
                DisplayListViewData();
            }
           
        }

        private Information GetSelectedInformation()
        {
            if (listViewData.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listViewData.SelectedItems[0];
                string selectedName = selectedItem.Text;
                return information.FirstOrDefault(ds => ds.GetName() == selectedName);
            }
            return null;
        }
        #endregion
        #region Add
        private void buttonAdd_Click(object sender, EventArgs e)
        {        
            string name = txtName.Text.Trim();
            //string category = comboBoxCategory.SelectedItem as string;
            string category = comboBoxCategory.Text.Trim();
            string structure = GetRadioButton();
            string definition = txtDefinition.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category) || 
                string.IsNullOrEmpty(structure) || string.IsNullOrEmpty(definition))
            {
                StatusStripDataStr.Items.Add("Please fill in all fields.");
                return;
            }

            if (information.Any(info => info.GetName() == name))
            {
                StatusStripDataStr.Items.Add("Duplicate Name. Please enter a unique name.");
            }

            
            Information newInformation = new Information();
            newInformation.SetName(name);
            newInformation.SetCategory(category);
            newInformation.SetDefinition(definition);
            newInformation.SetStructure(structure);
            information.Add(newInformation);

          /*  informationInstance.SetName(name);
            informationInstance.SetCategory(category);
            informationInstance.SetDefinition(definition);
            informationInstance.SetStructure(structure);
            AddData(informationInstance);*/

            ClearTextBox();
            
            DisplayListViewData();

        }

      /*  private void AddData(Information newInformation)
        {
            if (!information.Any(ds => ds.GetName() == newInformation.GetName()))
            {
                information.Add(newInformation);

            }
            else
            {
                StatusStripDataStr.Items.Add("Duplicate Name. Please enter a unique name.");
            }
        }*/
        #endregion
        #region group box radio button for structure

        private string GetRadioButton()
        {
            string radioText = "";
            if (radioButtonLinear.Checked)
                radioText = radioButtonLinear.Text;
            else if (radioButtonNonLinear.Checked)
                radioText = radioButtonNonLinear.Text;
            return radioText;
        }

        private void SetRadioButton(int item)
        {
            if (information[item].GetStructure() == "Linear")
                radioButtonLinear.Checked = true;
            else if (information[item].GetStructure() == "Non-Linear")
                radioButtonNonLinear.Checked = true;
        }
   
        #endregion
        #region Load
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            StatusStripDataStr.Items.Clear();
            listViewData.Items.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary Files|*.bin|All Files|*.*";
            openFileDialog.FileName = "definitions.bin";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                try
                {
                    information.Clear();
                    using (Stream stream = File.Open(fileName, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                        {
                            while (stream.Position < stream.Length)
                            {
                                Information newInfo = new Information();
                                newInfo.SetName(reader.ReadString());
                                newInfo.SetCategory(reader.ReadString());
                                newInfo.SetStructure(reader.ReadString());
                                newInfo.SetDefinition(reader.ReadString());

                                information.Add(newInfo);  
                            }
                        }
                    }
                    DisplayListViewData();
                    StatusStripDataStr.Items.Add("Data loaded successfully.");
                }
                catch (Exception ex)
                {
                    StatusStripDataStr.Items.Add("Error loading data: " + ex.Message);
                }
            }
        }

        #endregion

        private void buttonSave_Click(object sender, EventArgs e)
        {
            StatusStripDataStr.Items.Clear();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Binary Files|*.bin|All Files|*.*";
            saveFileDialog.FileName = "definitions.bin";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    using (FileStream filestream = new FileStream(filename, FileMode.Create))
                    
                    {
                        using (var writer = new BinaryWriter(filestream, Encoding.UTF8, false))
                        {
                           
                            foreach (var info in information)
                            {
                                writer.Write(info.GetName());
                                writer.Write(info.GetCategory());
                                writer.Write(info.GetStructure());
                                writer.Write(info.GetDefinition());
                            }
                        }
                    
                            StatusStripDataStr.Items.Add("Data saved successfully.");
                    }
                }
                catch (Exception ex)
                {
                    StatusStripDataStr.Items.Add("Error saving data: " + ex.Message);
                }
            }
        }

        private void WikiForm_Load(object sender, EventArgs e)
        {
            string filePath = "categories.dat";
            string[] categories = new string[]
            {
                "",
            "Array",
            "List",
            "Tree",
            "Graphs",
            "Abstract",
            "Hash"
            };
            //SaveCategories(filePath, categories);
            ReadCategories(filePath);
            InitializeListView();
        }
    }
}
