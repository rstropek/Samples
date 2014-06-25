using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Xml;

namespace Samples
{
		public class MotionPicture : Broadcast
		{
			// Dependency Property ImdbInfo
			public XmlDocument ImdbInfo
			{
				get { return (XmlDocument)GetValue(ImdbInfoProperty); }
				set { SetValue(ImdbInfoProperty, value); }
			}

			public static readonly DependencyProperty ImdbInfoProperty =
				DependencyProperty.Register("ImdbInfo", typeof(XmlDocument), typeof(MotionPicture), new UIPropertyMetadata(null));
		}
}
