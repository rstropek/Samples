using System;
using System.Windows.Markup;
using System.Xml;

namespace Samples
{
    [MarkupExtensionReturnType(typeof(XmlDocument))]
	public class ImdbInfoExtension : MarkupExtension
	{
		public ImdbInfoExtension(string motionPictureID)
		{
			this.MotionPictureID = motionPictureID;
		}

        public string MotionPictureID { get; } = string.Empty;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
            /*
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.trynt.com/movie-imdb-api/v2/?i=" + this.motionPictureID);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			Stream stream = response.GetResponseStream();
			StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
			string content = reader.ReadToEnd();
			reader.Close();
			stream.Close();
			response.Close();
            */
            const string content = "<?xml version='1.0' encoding='utf-8'?><trynt><movie-imdb><title>TitlePlaceholder</title><aka></aka><year>2004</year></movie-imdb></trynt>";
			var ImdbInfo = new XmlDocument();
			ImdbInfo.LoadXml(content);
			return ImdbInfo;
		}
	}
}
