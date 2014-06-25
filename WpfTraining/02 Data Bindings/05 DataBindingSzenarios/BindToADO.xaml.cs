using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;

namespace Samples
{
	public partial class BindToADO : Page
	{
		public BindToADO()
		{
			InitializeComponent();
			((SqlConnection)FindResource("Connection")).Open();

			DataSet result = (DataSet)FindResource("Data");
			((SqlDataAdapter)FindResource("DataAdapter")).Fill(result, "Person");

			DataList.DataContext = result;
		}
	}
}