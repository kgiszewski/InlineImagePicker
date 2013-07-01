using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Xml;

using umbraco.interfaces;
using umbraco.NodeFactory;
using umbraco.BusinessLogic;

namespace InlineImagePicker
{
    /// <summary>
    /// This class is used for the actual datatype dataeditor, i.e. the control you will get in the content section of umbraco. 
    /// </summary>
    public class DataEditor : System.Web.UI.UpdatePanel, umbraco.interfaces.IDataEditor
    {

        private umbraco.interfaces.IData savedData;
        private Options savedOptions;
        private XmlDocument savedXML = new XmlDocument();
        private TextBox saveBox;
        private HtmlGenericControl wrapperDiv = new HtmlGenericControl();

        public DataEditor(umbraco.interfaces.IData Data, Options Configuration)
        {
            //load the prevalues
            savedOptions = Configuration;

            //ini the savedData object
            savedData = Data;
        }

        public virtual bool TreatAsRichTextEditor
        {
            get { return false; }
        }

        public bool ShowLabel
        {
            get { return true; }
        }

        public Control Editor { get { return this; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string css = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/umbraco/plugins/InlineImagePicker/InlineImagePicker.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "InlineImagePickerCSS", css, false);

            string js = string.Format("<script src=\"{0}\" ></script>", "/umbraco/plugins/InlineImagePicker/DataType.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "InlineImagePickerJS", js, false);

            saveBox = new TextBox();
            saveBox.TextMode = TextBoxMode.MultiLine;
            saveBox.CssClass = "InlineImagePickerSaveBox";
            ContentTemplateContainer.Controls.Add(saveBox);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            buildControls();
        }

        protected void buildControls()
        {
            string data;

            //get the data based on action
            if (Page.IsPostBack)
            {
                data = saveBox.Text;
            }
            else
            {
                data = savedData.Value.ToString();
            }

            //load the data into an xml doc
            XmlDocument xd = new XmlDocument();

            try
            {
                xd.LoadXml(data);
            }
            catch (Exception e)
            {
                xd.LoadXml(DefaultData.defaultXML);
            }
            XmlNodeList dataXML = xd.SelectNodes("widgets/widget");

            /*
            //loop thru each widget
            foreach (XmlNode widgetNode in dataXML)
            {

                bool startCollapsed = Convert.ToBoolean(widgetNode.Attributes["isCollapsed"].InnerText);

                HtmlGenericControl widgetFadeWrapper = new HtmlGenericControl();
                widgetFadeWrapper.TagName = "div";
                widgetFadeWrapper.Attributes["class"] = "widgetFadeWrapper";

                HtmlGenericControl widgetWrapper = new HtmlGenericControl();
                widgetWrapper.TagName = "div";
                widgetWrapper.Attributes["class"] = "widgetWrapper";

                if (startCollapsed)
                {
                    widgetWrapper.Attributes["class"] += " widgetStartCollapsed";
                }

                widgetFadeWrapper.Controls.Add(widgetWrapper);

                //add buttons if more than 1 is allowed
                if (savedOptions.maxWidgets > 1)
                {
                    addButtons(widgetWrapper);
                }

                //loop through the schema
                foreach (WidgetElement schemaElement in savedOptions.elements)
                {
                    switch (schemaElement.type)
                    {
                        case "textbox":
                            renderTextbox(widgetWrapper, schemaElement, widgetNode);
                            break;

                        case "textarea":
                            renderTextarea(widgetWrapper, schemaElement, widgetNode);
                            break;

                        case "spreadsheet":
                            renderSpreadsheet(widgetWrapper, schemaElement, widgetNode);
                            break;

                        case "mediapicker":
                            renderMediapicker(widgetWrapper, schemaElement, widgetNode);
                            break;

                        case "damp":
                            renderDamp(widgetWrapper, schemaElement, widgetNode);
                            break;

                        case "list":
                            renderList(widgetWrapper, schemaElement, widgetNode);
                            break;

                        case "dropdown":
                            renderDropdown(widgetWrapper, schemaElement, widgetNode);
                            break;

                        case "checkradio":
                            renderCheckRadio(widgetWrapper, schemaElement, widgetNode);
                            break;
                    }
                }

                wrapperDiv.Controls.Add(widgetFadeWrapper);
            }
             */
        }

        public void Save()
        {
            savedData.Value = saveBox.Text;
        }

    }
}