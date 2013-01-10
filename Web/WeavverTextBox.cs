using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using Weavver.Data;

namespace Weavver.Web
{
	public class WeavverTextBox : TextBox
	{
		string			oldtext		= "";
		bool			changed		= false;
//--------------------------------------------------------------------------------------------
		public bool Changed
		{
			get {return changed;}
		}
//--------------------------------------------------------------------------------------------
		public string OldText
		{
			get {return oldtext;}
		}
//--------------------------------------------------------------------------------------------
		public void Reset()
		{
			base.Text	= "";
			oldtext		= "";
			changed		= false;
		}
//--------------------------------------------------------------------------------------------
          //public override string Text
          //{
          //     get
          //     {
          //          WeavverDataManager dm = DataManager;
          //          Weavver.Data.WeavverDataTable dt = DataManager.DataTable;

          //          if (DataColumn == "")
          //          {
          //               return "Error: The datatable or datamanager property is set incorrectly.";
          //          }
          //          else if (DataManager.Mode == WeavverDataManagerMode.New)
          //          {
          //               return base.Text;
          //          }
          //          else if (DataManager.Mode == WeavverDataManagerMode.Update)
          //          {
          //               if (Changed)
          //               {
          //                    return base.Text;
          //               }
          //               else
          //               {
          //                    return DataManager.DataTable.SelectedRow[DataColumn].ToString();
          //               }
          //          }
          //          else
          //          {
          //               return "null";
          //          }
          //     }
          //     set
          //     {
          //          if (value != Text)
          //          {
          //               oldtext = Text;
          //               base.Text = value;
          //               changed = true;
          //          }
          //     }
          //}
//--------------------------------------------------------------------------------------------
	}
}
