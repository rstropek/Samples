using System;
using System.IO;
using System.Text;

static class Start
{
	static void Main()
	{
		const string TargetText = "This is the target text! äüö";

		using (var bw = new BinaryWriter(new FileStream("AsciiText.txt", FileMode.Create)))
		{
			var ascii = new ASCIIEncoding();
			var asciiEncodedText = ascii.GetBytes(TargetText);
			bw.Write(asciiEncodedText);
			bw.Close();
		}

		using (BinaryWriter bw = new BinaryWriter(new FileStream("UTF8Text.txt", FileMode.Create)))
		{
			var utf8 = new UTF8Encoding();
			var utf8EncodedText = utf8.GetBytes(TargetText);
			bw.Write(utf8EncodedText);
			bw.Close();
		}
	}
}