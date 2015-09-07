import clr
clr.AddReference("mscorlib")
clr.AddReference("PresentationFramework")

from System.Windows import Application
from System.IO import StreamReader
from System.Threading import Thread
from System.Windows.Markup import XamlReader
from System.Reflection import Assembly
from System import Action

class ViewModel:
  numberOfSpeakers = 0
  def __init__(self, speakers):
    self.numberOfSpeakers = speakers

def getNumberOfSpeakers():
  vm = ViewModel(Application.Current.MainWindow.DataContext.Speakers.Length)
  stream = Application.Current.GetType().Assembly.GetManifestResourceStream(
    "IronPython.UI.Scripts.ResultWindow.xaml")
  reader = StreamReader(stream)
  window = XamlReader.Parse(reader.ReadToEnd())
  reader.Close()
  stream.Close()
  window.DataContext = vm
  window.FindName("CloseButton").Click += lambda s, e: window.Close()
  window.Show()

Application.Current.Dispatcher.BeginInvoke(Action(lambda: getNumberOfSpeakers()))

for i in range(0, 10):
  print str(i+1)
  Thread.Sleep(500)

print "Done!"