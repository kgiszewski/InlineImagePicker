using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Xml;

using umbraco.interfaces;
using umbraco.NodeFactory;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.media;

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
        private HtmlGenericControl wrapperDiv;

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

            string css = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/App_Plugins/InlineImagePicker/InlineImagePicker.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "InlineImagePickerCSS", css, false);

            string js = string.Format("<script src=\"{0}\" ></script>", "/App_Plugins/InlineImagePicker/InlineImagePicker.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "InlineImagePickerJS", js, false);

            string js2 = string.Format("<script src=\"{0}\" ></script>", "/App_Plugins/InlineImagePicker/sort.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "InlineImagePickerSortJS", js2, false);

            saveBox = new TextBox();
            saveBox.TextMode = TextBoxMode.MultiLine;
            saveBox.CssClass = "InlineImagePickerSaveBox";
            ContentTemplateContainer.Controls.Add(saveBox);

            HtmlGenericControl toolbarDiv = new HtmlGenericControl("div");
            ContentTemplateContainer.Controls.Add(toolbarDiv);
            toolbarDiv.Attributes["class"] = "InlineImageToolbar";

            string toolbarHtml = "";
            toolbarHtml += "<img title='Sort Ascending' class='InlineImagePickerSortA' src='/App_Plugins/InlineImagePicker/images/a.png'/>";
            toolbarHtml += "<img title='Sort Descending' class='InlineImagePickerSortZ' src='/App_Plugins/InlineImagePicker/images/z.png'/>";
            toolbarHtml += "<img title='Sort Newest' class='InlineImagePickerNew' src='/App_Plugins/InlineImagePicker/images/new.png'/>";
            toolbarHtml += "<img title='Sort Oldest' class='InlineImagePickerOld' src='/App_Plugins/InlineImagePicker/images/old.png'/>";

            toolbarDiv.InnerHtml = toolbarHtml;

            wrapperDiv = new HtmlGenericControl("div");
            wrapperDiv.Attributes["class"] = "InlineImagePickerWrapperDiv";
            ContentTemplateContainer.Controls.Add(wrapperDiv);
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

            XmlNode selectedDataXML = xd.SelectSingleNode("//image");


                        
            //Log.Add(LogTypes.Custom, 0, "xd=>"+xd.OuterXml);
            //Log.Add(LogTypes.Custom, 0, "selectedXML=>" + selectedDataXML.OuterXml);
            //Log.Add(LogTypes.Custom, 0, "selectedXMLinner=>" + selectedDataXML.InnerText);
            //Log.Add(LogTypes.Custom, 0, "prevalue=>" + savedOptions.mediaIDs);            

            foreach(string mediaID in savedOptions.mediaIDs.Split(',')){
                Media media = new Media(Convert.ToInt32(mediaID));

                foreach (Media thisMedia in media.Children)
                {
                    try
                    {
                        HtmlGenericControl div = new HtmlGenericControl("div");
                        wrapperDiv.Controls.Add(div);

                        //add classes
                        div.Attributes["class"] = "InlineImageWrapper";
                        if(selectedDataXML!=null && thisMedia.Id.ToString()==selectedDataXML.InnerText){
                            div.Attributes["class"]+= " InlineImageSelected";
                        }

                        //data for client
                        div.Attributes["data-mediaID"] = thisMedia.Id.ToString();
                        div.Attributes["data-unixTime"] = ToUnixTime(thisMedia.CreateDateTime).ToString();

                        umbraco.cms.businesslogic.property.Property umbracoFile = thisMedia.getProperty("umbracoFile");

                        string thumbUrl=umbracoFile.Value.ToString();

                        if(thumbUrl.Contains(".jpg")){
                            thumbUrl = thumbUrl.Replace(".jpg", "_thumb.jpg");
                        }

                        if (thumbUrl.Contains(".gif"))
                        {
                            thumbUrl = thumbUrl.Replace(".gif", "_thumb.jpg");
                        }

                        if (thumbUrl.Contains(".png"))
                        {
                            thumbUrl = thumbUrl.Replace(".png", "_thumb.jpg");
                        }

                        div.InnerHtml = "<img src='" + thumbUrl+ "'/><div class='InlineImageDetails'>" + thisMedia.Text + "</div>";
                        
                        //Log.Add(LogTypes.Custom, 0, "uf=>" + umbracoFile.Value);
                    }
                    catch { }
                }
            }           
        }

        public long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }

        public void Save()
        {
            savedData.Value = saveBox.Text;
        }

    }
}