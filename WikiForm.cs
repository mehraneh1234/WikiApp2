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
        Information informationInstance = new Information();
        List<Information> information = new List<Information>();
        private TextBox[] textBoxValues;
        private string selectedStructure = "Linear"; // Default value
        string filePath = "categories.dat";
        string[] categories = new string[]
        {
            "Array",
            "List",
            "Tree",
            "Graphs",
            "Abstract",
            "Hash"
        };

        public WikiForm()
        {
            InitializeComponent();
            //SaveCategories(filePath, categories);
            ReadCategories(filePath);
            InitializeListView();
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
            foreach (TextBox textBox in textBoxValues)
            {
                textBox.Clear();
            }
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
            
            // Information.SetCategory(comboBoxCategory, "categories.txt");
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
            string category = comboBoxCategory.SelectedItem as string;
            string structure = selectedStructure;
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

            Information newInformation = new Information(name, category, definition, structure);
            information.Add(newInformation);

          /*  informationInstance.SetName(name);
            informationInstance.SetCategory(category);
            informationInstance.SetDefinition(definition);
            informationInstance.SetStructure(structure);
            AddData(informationInstance);*/

            //ClearTextBox();
            txtName.Clear();
            txtDefinition.Clear();
            selectedStructure = "Linear";
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
        private void groupBoxStructure_Enter(object sender, EventArgs e)
        {
            if (radioButtonLinear.Checked)
            {
                selectedStructure = "Linear";
            }
            else if (radioButtonNonLinear.Checked)
            {
                selectedStructure = "Non-Linear";
            }
        }
        #endregion
        #region Load
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            StatusStripDataStr.Items.Clear();
            listViewData.Items.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary Files|*.dat|All Files|*.*";
            openFileDialog.FileName = "definitions.dat";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                try
                {
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        int loadedRowCount = binaryReader.ReadInt32();
                        information.Clear();
                        for (int i =0; i < loadedRowCount; i++)
                        {
                            Information newInfo = new Information();
                            newInfo.SetName(binaryReader.ReadString());
                            newInfo.SetCategory(binaryReader.ReadString());
                            newInfo.SetStructure(binaryReader.ReadString());
                            newInfo.SetDefinition(binaryReader.ReadString());

                            information.Add(newInfo);
                        }
                        DisplayListViewData();
                        StatusStripDataStr.Items.Add("Data loaded successfully.");
                        //buttonSave.Enabled = true;
                    }
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
            saveFileDialog.Filter = "Binary Files|*.dat|All Files|*.*";
            saveFileDialog.FileName = "definitions.dat";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    using (FileStream filestream = new FileStream(filename, FileMode.Create))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(filestream, information);
                        StatusStripDataStr.Items.Add("Data saved successfully.");
                    }
                }
                catch (Exception ex)
                {
                    StatusStripDataStr.Items.Add("Error saving data: " + ex.Message);
                }
            }
        }
    }
}
