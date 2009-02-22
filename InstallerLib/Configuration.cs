using System;
using System.Xml;
using System.ComponentModel;
using System.Collections.Generic;

namespace InstallerLib
{
    /// <summary>
    /// A configuration.
    /// </summary>
    [XmlChild(typeof(EmbedFile))]
    [XmlChild(typeof(Component))]
    [XmlChild(typeof(DownloadDialog))]
    public abstract class Configuration : XmlClassImpl
    {
        public Configuration(string p_type)
        {
            m_type = p_type;
        }

        #region Configuration Prop
        private string m_lcid = string.Empty;
        [Description("A filter to install this configuration only on all operating system language equals or not equals than the LCID specified (see Help->LCID Table). Separate multiple LCID with comma (',') and use not symbol ('!') for NOT logic (es. '1044,1033,!1038' ). You can also filter a specified component. (OPTIONAL)")]
        public string lcid
        {
            get { return m_lcid; }
            set { m_lcid = value; OnLCIDChanged(); }
        }

        private string m_type;
        [Description("Type of the configuration. Can be 'install' or 'reference'. (REQUIRED)")]
        public string type
        {
            get { return m_type; }
        }

        #endregion

        public override string ToString()
        {
            return m_type + ":" + m_lcid;
        }

        protected void OnLCIDChanged()
        {
            if (LCIDChanged != null)
                LCIDChanged(this, EventArgs.Empty);
        }

        public event EventHandler LCIDChanged;

        #region IXmlClass Members

        public override string XmlTag
        {
            get { return "configuration"; }
        }

        protected override void OnXmlReadTag(XmlElementEventArgs e)
        {
            if (e.XmlElement.Attributes["lcid"] != null)
                lcid = e.XmlElement.Attributes["lcid"].InnerText;
        }

        protected override void OnXmlWriteTag(XmlWriterEventArgs e)
        {
            e.XmlWriter.WriteAttributeString("type", m_type);
            e.XmlWriter.WriteAttributeString("lcid", m_lcid);
            base.OnXmlWriteTag(e);
        }

        public static Configuration CreateFromXml(XmlElement element)
        {
            Configuration l_Config;
            string xmltype = element.Attributes["type"].InnerText;
            if (xmltype == "install")
                l_Config = new SetupConfiguration();
            else if (xmltype == "reference")
                l_Config = new WebConfiguration();
            else
                throw new Exception(string.Format("Invalid configuration type: {0}", xmltype));
            
            l_Config.FromXml(element);
            return l_Config;
        }

        #endregion
    }
}