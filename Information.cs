using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WikiApplication
{
    [Serializable]
    public class Information : IComparable<Information>
    {
        
        public Information()
        {
            //throw new System.NotImplementedException();
        }

        private string _name;
        private string _category;
        private string _definition;
        private string _structure;
       
        public Information(string name, string category, string definition, string structure)
        {
            _name = name;
            _category = category;
            _definition = definition;
            _structure = structure;
        }
        public int CompareTo(Information other)
        {
            string thisName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this._name);
            string otherName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(other._name);
            return String.Compare(this._name, other._name, StringComparison.OrdinalIgnoreCase);
        }

     
        public string GetName()
        {
            //throw new System.NotImplementedException();
            return _name;
        }

        public string GetCategory()
        {
            //throw new System.NotImplementedException();
            return _category;
        /*   if (File.Exists(filePath))
            {
                List<string> categories = new List<string>();
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        categories.Add(line);
                    }
                }
                return string.Join(", ", categories);
            }
            else
            {
                //MessageBox.Show("Categories file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Categories file not found.";
            }*/
        }

        public string GetStructure()
        {
            return _structure;
        }

        public string GetDefinition()
        {
            return _definition;
        }

        public void SetName(string name)
        {
            _name = name;

        }

        public void SetCategory(string category)
        {
            _category = category;

        }

            public void SetStructure(string structure)
        {
            _structure = structure;
        }

        public void SetDefinition(string definition)
        {
            _definition = definition;
        }
    }
}