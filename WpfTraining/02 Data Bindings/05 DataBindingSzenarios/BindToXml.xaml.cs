using System;
using System.Windows.Controls;
using System.Xml;
using System.Windows;
using System.Windows.Data;

namespace Samples
{
	public partial class BindToXml : Page
	{
		public BindToXml()
		{
			InitializeComponent();
			DataContext = this;
		}

		public String StockSymbol
		{
			get { return (String)GetValue(StockSymbolProperty); }
			set { SetValue(StockSymbolProperty, value); }
		}
		public static readonly DependencyProperty StockSymbolProperty =
			DependencyProperty.Register("StockSymbol", typeof(String), typeof(BindToXml),
			new PropertyMetadata("", new PropertyChangedCallback(OnStockSymbolChanged)));
		private static void OnStockSymbolChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			BindToXml d = sender as BindToXml;
			if (d.StockSymbol.Length > 0)
			{
				StockQuoteService.StockQuote service = new Samples.StockQuoteService.StockQuote();
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(service.GetQuote(d.StockSymbol));
				((XmlDataProvider)d.FindResource("MarketDataXml")).Document = doc;
			}
		}

	}
}