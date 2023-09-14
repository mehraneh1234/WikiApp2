using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace WikiApplication
{
    public partial class WikiForm : Form
    {
       
        List<Information> information = new List<Information>();
        
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

        private void WikiForm_Load(object sender, EventArgs e)
        {
            string filePath = "categories.dat";
            ReadCategories(filePath);
            InitializeListView();
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
        private void ReadCategories(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    comboBoxCategory.Items.AddRange(System.IO.File.ReadAllLines(filePath));
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
        }
      

   /*     private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
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
        }*/
        #endregion
        #region radio buttons for structure

        private string GetRadioButton()
        {
            string radioText = "";
            if (radioButtonLinear.Checked)
                radioText = radioButtonLinear.Text;
            else if (radioButtonNonLinear.Checked)
                radioText = radioButtonNonLinear.Text;
            return radioText;
        }

        private void SetRadioButton(int index)
        {
            if (information[index].GetStructure() == "Linear")
                radioButtonLinear.Checked = true;
            else if (information[index].GetStructure() == "Non-Linear")
                radioButtonNonLinear.Checked = true;
        }
   
        #endregion
        #region Add
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            StatusStripDataStr.Items.Clear();
            string name = txtName.Text.Trim();
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

            ClearTextBox();
            DisplayListViewData();
            StatusStripDataStr.Items.Add("The new data structure is added.");

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
        #region Save button and close form to save
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

        private void WikiForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            buttonSave_Click(this, e);
        }

        #endregion
        #region Edit

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            StatusStripDataStr.Items.Clear();
            Information newInfo = new Information();
            if (listViewData.SelectedItems.Count > 0)
            {
                int selectedRowIndex = listViewData.SelectedIndices[0];
                string name = txtName.Text.Trim();
                string category = comboBoxCategory.Text.Trim();
                string structure = GetRadioButton();
                string definition = txtDefinition.Text.Trim();

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(category) &&
                    !string.IsNullOrEmpty(structure) && !string.IsNullOrEmpty(definition))
                {
                    information[selectedRowIndex].SetName(name);
                    information[selectedRowIndex].SetCategory(category);
                    information[selectedRowIndex].SetStructure(structure);
                    information[selectedRowIndex].SetDefinition(definition);
                }
            }
            else
            {
                StatusStripDataStr.Items.Add("Please select a row to edit.");
            }
            StatusStripDataStr.Items.Add("The selected row is edited.");
            ClearTextBox();
            DisplayListViewData();
        }
        #endregion
        #region Delete and double click on text box name and list view
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            StatusStripDataStr.Items.Clear();
            if (listViewData.SelectedItems.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure to delete this data?", "Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int selectedIndex = listViewData.SelectedItems[0].Index;
                    information.RemoveAt(selectedIndex);
                    DisplayListViewData();
                }
            }
            else
            {
                StatusStripDataStr.Items.Add("Please select a row to delete.");
            }
        }

        private void txtName_DoubleClick(object sender, EventArgs e)
        {
            ClearTextBox();
            txtName.Focus();
        }

        private void listViewData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewData.SelectedItems.Count > 0)
            {
                buttonDelete_Click(this, e);
            }
        }

        #endregion
        #region Search
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            listViewData.SelectedItems.Clear();
            StatusStripDataStr.Items.Clear();
            if (!String.IsNullOrEmpty(txtSearch.Text))
            {
                information.Sort();
                Information result = information.FirstOrDefault(ds => 
                    ds.GetName().Equals(txtSearch.Text, StringComparison.OrdinalIgnoreCase));
                if (result != null)
                {
                    int index = information.FindIndex(ds => 
                        ds.GetName().Equals(txtSearch.Text, StringComparison.OrdinalIgnoreCase));
                    if (index != -1)
                    {
                        SetRadioButton(index);
                        txtName.Text = result.GetName();
                        comboBoxCategory.Text = result.GetCategory();
                        txtDefinition.Text = result.GetDefinition();
                        foreach (ListViewItem item in listViewData.Items)
                        {
                            if (item.Text == result.GetName())
                            {
                                item.Selected = true;
                                break;
                            }
                        }
                        StatusStripDataStr.Items.Add("Target found.");
                        txtSearch.Clear();
                        txtSearch.Focus();
                    }

                    else
                    {
                        StatusStripDataStr.Items.Add("Please write the correct target!");
                    }

                    
                }
                else
                {
                    StatusStripDataStr.Items.Add("Target not found.");
                }

            }
            else
            {
                StatusStripDataStr.Items.Add("Please write the target to search.");
            }
            

        }
        #endregion
        #region Capitalize initial name and double click on txtName to clear all
        private void txtName_Leave(object sender, EventArgs e)
        {
            txtName.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtName.Text);
        }

        private void txtName_DoubleClick_1(object sender, EventArgs e)
        {
            ClearTextBox();
            txtName.Focus();
        }


        #endregion

        private void listViewData_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatusStripDataStr.Items.Clear();
           
            if (listViewData.SelectedItems.Count > 0)
            {
                int index = listViewData.SelectedIndices[0];
                txtName.Text = information[index].GetName();
                comboBoxCategory.Text = information[index].GetCategory();
                SetRadioButton(index);
                
                txtDefinition.Text = information[index].GetDefinition();
            }
        }
    }
}
