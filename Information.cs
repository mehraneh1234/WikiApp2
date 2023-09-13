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
        }

        private string _name;
        private string _category;
        private string _definition;
        private string _structure;
       
      
        public int CompareTo(Information other)
        {
            string thisName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this._name);
            string otherName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(other._name);
            return String.Compare(this._name, other._name, StringComparison.OrdinalIgnoreCase);
        }

     
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