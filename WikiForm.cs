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
using System.Diagnostics;

//Assessment Task two
// Mehraneh Hamedani - 30062786 - 04/10/2023
namespace WikiApplication
{
    public partial class WikiForm : Form
    {
       // Create a list of (information) class
        List<Information> information = new List<Information>();
         
        public WikiForm()
        {// Call the method
            InitializeComponent();  
        }
        #region Initialize the list view and combo box when form is loaded
        // Make a method to display by default the title of the list view.
        private void InitializeListView()
        {
            listViewData.View = View.Details;
            listViewData.Columns.Add("Name");
            listViewData.Columns.Add("Category");
        }
        // When form load load the data of combobox and call the method InitializeListView()
        private void WikiForm_Load(object sender, EventArgs e)
        {
            string filePath = "categories.dat";
            ReadCategories(filePath);
            InitializeListView();
        }
        #endregion
        #region Display list view and clear text boxes
        // Clear the list view and if all properties are filled by the user. the data of them are showed in the list view.
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

        // All of the input tools clear and the cursor will be on name textbox.
        private void ClearTextBox()
        {
            txtName.Clear();
            txtDefinition.Clear();
            radioButtonLinear.Checked = false;
            radioButtonNonLinear.Checked = false;
            comboBoxCategory.Text = "";
            txtName.Focus();
        }
        #endregion
        #region Load combobox data from a file
        // Read the combobox from the file and check if there is any problem or the file does not exist.
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
     
        #endregion
        #region radio buttons for structure
        // In this method fill the text of the radio button in related to which radio button is checked.
        private string GetRadioButton()
        {
            string radioText = "";
            if (radioButtonLinear.Checked)
                radioText = radioButtonLinear.Text;
            else if (radioButtonNonLinear.Checked)
                radioText = radioButtonNonLinear.Text;
            return radioText;
        }
        // It depends on the data which saved for the (structure) property the appropriate radio button is selected.
        private void SetRadioButton(int index)
        {
            if (information[index].GetStructure() == "Linear")
                radioButtonLinear.Checked = true;
            else if (information[index].GetStructure() == "Non-Linear")
                radioButtonNonLinear.Checked = true;
        }
   
        #endregion
        #region Add
        // Add data with a unique name to the list but before that check if all input properties are filled or not.
        // Then Display the appropriate message.
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            //StreamWriter TextWriter;
            //TextWriter = new StreamWriter("DebugCompare.txt", true);
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
            Trace.TraceInformation("Compare the name: {0} with existing data.", name);
            //TextWriter.WriteLine("Compare the name: {0}", name);

            if (information.Any(info => info.GetName() == name))
            {
                StatusStripDataStr.Items.Add("Duplicate Name. Please enter a unique name.");
                Trace.TraceInformation("Duplicate Name");
                //TextWriter.WriteLine("Duplicate Name");
            }
            else
            {
                //TextWriter.Close();
                Information newInformation = new Information();
                newInformation.SetName(name);
                newInformation.SetCategory(category);
                newInformation.SetDefinition(definition);
                newInformation.SetStructure(structure);
                information.Add(newInformation);
                information.Sort();
                ClearTextBox();
                DisplayListViewData();
                StatusStripDataStr.Items.Add("The new data structure is added.");
            }            

        }

     
        #endregion
        #region Load
        // By click on this button the data of binary file loaded and display in the list view.
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
        // By click on this button all data in the list will be saved in the binary file which is called definitions.
        //By using try and catch control if there is any problem.
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
        // By click oe Edit button check if one item is selected in the list view and all the input properties are filled then 
        // allow to edit that item.
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
                    StatusStripDataStr.Items.Add("The selected row is edited.");
                }
            }
            else
            {
                StatusStripDataStr.Items.Add("Please select a row to edit.");
            }
            
            ClearTextBox();
            DisplayListViewData();
        }
        #endregion
        #region Delete and double click on text box name and list view
        // By click on delete button pop up a window to ask about deleting and if select an item in the ist view, it allows to delete.
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
                    StatusStripDataStr.Items.Add("The selected item is deleted.");
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
        // First sort the file then check if the search text box is filled then browsing the item if the name
        // of that item exists in the file then fill the input properties with the appropriate data of that item.
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
        // To capitalize the initial of the name.
        private void txtName_Leave(object sender, EventArgs e)
        {
            txtName.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtName.Text);
        }
        // By double click on the name textbox all input properties will be cleared.
        private void txtName_DoubleClick_1(object sender, EventArgs e)
        {
            ClearTextBox();
            txtName.Focus();
        }


        #endregion
        #region Select index change in the list view
        // By select an item in the list view all data of that item display in the input properties.
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
        #endregion
    }
}
