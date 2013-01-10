using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using AjaxControlToolkit;

namespace Weavver.Web
{
     [ToolboxData("<{0}:AutoCompleteTextBox runat=server></{0}:AutoCompleteTextBox>")]
     public class AutoCompleteTextBox : TextBox, IPostBackEventHandler
     {
          public delegate void ItemSelectedHandler(string eventArgument);
          public event ItemSelectedHandler ItemSelected;
          public AutoCompleteExtender _AutoComplete;
          public Button _Edit;
//-------------------------------------------------------------------------------------------
          public Button Edit
          {
               get
               {
                    EnsureChildControls();
                    return _Edit;
               }
               set
               {
                    _Edit = value;
               }
          }
//-------------------------------------------------------------------------------------------
          public AutoCompleteExtender AutoComplete
          {
               get
               {
                    EnsureChildControls();
                    return _AutoComplete;
               }
               set
               {
                    _AutoComplete = value;
               }
          }
//-------------------------------------------------------------------------------------------
          public string DataId
          {
               get
               {
                    if (ViewState["DataId"] != null)
                    {
                         return (string) ViewState["DataId"];
                    }
                    return null;
               }
               set
               {
                    ViewState["DataId"] = value;
               }
          }
//-------------------------------------------------------------------------------------------
          public AutoCompleteTextBox()
          {
               Init += new EventHandler(AutoCompleteTextBox_Init);
               Load += new EventHandler(AutoCompleteTextBox_Load);
          }
//-------------------------------------------------------------------------------------------
          void AutoCompleteTextBox_Init(object sender, EventArgs e)
          {
               // Page.RegisterRequiresRaiseEvent(this);
               
               // "Content_Content_AutoText_AutoCompleteId"
               //if (!Page.ClientScript.IsClientScriptBlockRegistered("Dbworks.Web.Controls.TabControl"))
               //     Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Web.Controls.TabControl", JavaScript());
          }
//-------------------------------------------------------------------------------------------
          void AutoCompleteTextBox_Load(object sender, EventArgs e)
          {
          }
//-------------------------------------------------------------------------------------------
          protected override void CreateChildControls()
          {
               Attributes.Add("autocomplete", "off");
               Attributes.Add("postbackid", UniqueID);

               AutoComplete = new AutoCompleteExtender();
               AutoComplete.ID = ClientID + "_AutoCompleteId";
               AutoComplete.TargetControlID = UniqueID;
               AutoComplete.ServicePath = "~/System/Tests/AutoComplete.asmx";
               AutoComplete.ServiceMethod = "GetOrganizationsCompletionList";
               AutoComplete.OnClientItemSelected = "AutoCompleteTextBox_itemSelected";
               AutoComplete.MinimumPrefixLength = 1;
               Controls.Add(AutoComplete);

               Edit = new Button();
               Edit.ID = ClientID + "_Edit";
               Edit.Text = "Edit";
               Controls.Add(Edit);
          }
//-------------------------------------------------------------------------------------------
          protected override void Render(System.Web.UI.HtmlTextWriter writer)
          {
               base.Render(writer);

               AutoComplete.RenderControl(writer);
               Edit.RenderControl(writer);
          }
//-------------------------------------------------------------------------------------------
          void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
          {
               OnItemSelected(eventArgument);
          }
//-------------------------------------------------------------------------------------------
          public virtual void OnItemSelected(string eventArgument)
          {
               if (ItemSelected != null)
               {
                    string data = Page.Request.Params["__EVENTARGUMENT"];
                    if (data != null && data != "")
                    {
                         DataId = data;
                         ItemSelected(data);
                    }
               }
          }
//-------------------------------------------------------------------------------------------
     }
}
