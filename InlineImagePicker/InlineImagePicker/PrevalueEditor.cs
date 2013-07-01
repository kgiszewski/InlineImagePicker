using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;

using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;
using umbraco.interfaces;

using System.Web.Script.Serialization;

namespace InlineImagePicker
{
    public class PrevalueEditor : System.Web.UI.UpdatePanel, IDataPrevalue
    {
        // referenced datatype
        private umbraco.cms.businesslogic.datatype.BaseDataType _datatype;

        private TextBox saveBox;

        private JavaScriptSerializer jsonSerializer;
        private Options savedOptions;

        public PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType DataType)
        {
            _datatype = DataType;
            jsonSerializer = new JavaScriptSerializer();
            savedOptions = Configuration;
        }

        public Control Editor
        {
            get
            {
                return this;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            saveBox = new TextBox();
            saveBox.TextMode = TextBoxMode.MultiLine;
            saveBox.CssClass = "saveBox";
            ContentTemplateContainer.Controls.Add(saveBox);

            string css = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/App_Plugins/InlineImagePicker/Prevalue.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "InlineImagePickerPrevalueCSS", css, false);

            string js = string.Format("<script src=\"{0}\" ></script>", "/App_Plugins/InlineImagePicker/Prevalue.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "InlineImagePickerPrevalueJS", js, false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            Options renderingOptions;

            //test for postback, decide to use db or saveBox for rendering
            if (Page.IsPostBack)
            {
                //test for saveBox having a value, default if not
                if (saveBox.Text != "")
                {
                    renderingOptions = jsonSerializer.Deserialize<Options>(saveBox.Text);
                }
                else
                {
                    renderingOptions = new Options();
                }
            }
            else
            {
                renderingOptions = savedOptions;
            }


            createPrevalueTable(writer, renderingOptions);
        }

        protected void createPrevalueTable(HtmlTextWriter writer, Options renderingOptions)
        {
            HtmlGenericControl table = new HtmlGenericControl("table");

            HtmlGenericControl tbody = new HtmlGenericControl("tbody");
            table.Controls.Add(tbody);

            HtmlGenericControl tr = new HtmlGenericControl("tr");
            tbody.Controls.Add(tr);

            HtmlGenericControl th = new HtmlGenericControl("th");
            tr.Controls.Add(th);
            th.InnerHtml = "Media Tree Nodes IDs (CSV)";

            HtmlGenericControl td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            TextBox textbox = new TextBox();
            textbox.Attributes["class"] = "mediaNodes";
            td.Controls.Add(textbox);

            textbox.Text = renderingOptions.mediaIDs;

            table.RenderControl(writer);
        }

        public void Save()
        {
            _datatype.DBType = (umbraco.cms.businesslogic.datatype.DBTypes)Enum.Parse(typeof(umbraco.cms.businesslogic.datatype.DBTypes), DBTypes.Ntext.ToString(), true);

            SqlHelper.ExecuteNonQuery("delete from cmsDataTypePreValues where datatypenodeid = @dtdefid", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId));
            SqlHelper.ExecuteNonQuery("insert into cmsDataTypePreValues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId), SqlHelper.CreateParameter("@value", saveBox.Text));
        }

        public Options Configuration
        {
            get
            {
                string dbValue = "";
                try
                {
                    object conf = SqlHelper.ExecuteScalar<object>("select value from cmsDataTypePreValues where datatypenodeid = @datatypenodeid", SqlHelper.CreateParameter("@datatypenodeid", _datatype.DataTypeDefinitionId));
                    dbValue = conf.ToString();
                }
                catch (Exception e)
                {
                }

                if (dbValue.ToString() != "")
                {
                    return jsonSerializer.Deserialize<Options>(dbValue.ToString());
                }
                else
                {
                    return new Options();
                }
            }
        }

        public static ISqlHelper SqlHelper
        {
            get
            {
                return Application.SqlHelper;
            }
        }
    }
}