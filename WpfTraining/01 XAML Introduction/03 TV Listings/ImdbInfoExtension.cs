using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Markup;
using System.Xml;

namespace Samples
{
	[MarkupExtensionReturnType(typeof(XmlDocument))]
	public class ImdbInfoExtension : MarkupExtension
	{
		private string motionPictureID = "";

		public ImdbInfoExtension(string motionPictureID)
		{
			this.motionPictureID = motionPictureID;
		}

		public string MotionPictureID
		{
			get
			{
				return motionPictureID;
			}
			set
			{
				motionPictureID = value;
			}
		}

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
            string content = "<?xml version='1.0' encoding='utf-8'?><trynt><movie-imdb><title>TitlePlaceholder</title><aka></aka><year>2004</year></movie-imdb></trynt>";
			XmlDocument ImdbInfo = new XmlDocument();
			ImdbInfo.LoadXml(content);
			return ImdbInfo;
		}
	}
}
