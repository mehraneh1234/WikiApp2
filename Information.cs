using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;

namespace WikiApplication
{// To save the data in a file it is eligible to write this code.
    [Serializable]
    // Define the class which is inherited by IComparable. 
    public class Information : IComparable<Information>
    {
        // Construct the class
        public Information()
        {
        }
        // Define all properties in private 
        private string _name;
        private string _category;
        private string _definition;
        private string _structure;
       
        // This method should be defined because of using IComparable. It compares the current name with the new one.  
        public int CompareTo(Information other)
        {
            string thisName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this._name);
            string otherName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(other._name);
            Trace.TraceInformation("Compare the name: {0}", thisName);
            return String.Compare(this._name, other._name, StringComparison.OrdinalIgnoreCase);
        }

        // Define getter methods of the properties.
        public string GetName()
        {
            return _name;
        }

        public string GetCategory()
        {
            return _category;
        }

        public string GetStructure()
        {
            return _structure;
        }

        public string GetDefinition()
        {
            return _definition;
        }
        // Define setter methods of the properties
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