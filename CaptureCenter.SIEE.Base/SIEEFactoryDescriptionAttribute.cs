using System;

namespace ExportExtensionCommon
{
    public class SIEEFactoryDescriptionAttribute : System.Attribute
    {
        private string label = "";
        private string role = "";
        private string description = "";
        private string manufacturer = "";

        /// <summary>
        /// Manufacturer
        /// </summary>
        public string Manufacturer
        {
            get { return manufacturer; }
            set { manufacturer = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SIEEFactoryDescriptionAttribute(string label)
            : this(label, string.Empty, string.Empty, string.Empty)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public SIEEFactoryDescriptionAttribute(string label, string role)
            : this(label, role, string.Empty, string.Empty)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public SIEEFactoryDescriptionAttribute(string label, string role, string description)
            : this(label, role, description, string.Empty)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public SIEEFactoryDescriptionAttribute(string label, string role, string description, string manufacturer)
        {
            this.label = label;
            this.role = role;
            this.description = description;
            this.manufacturer = manufacturer;
        }

        /// <summary>
        /// Label
        /// </summary>
        public string Label
        {
            get { return this.label; }
        }

        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get { return this.description; }
        }

        /// <summary>
        /// Role
        /// </summary>
        public string Role
        {
            get { return this.role; }
        }
    }
}
