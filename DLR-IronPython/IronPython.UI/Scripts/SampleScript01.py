import clr
clr.AddReference("mscorlib")

from System.Threading import Thread

for i in range(0, 10):
  print str(i+1)
  Thread.Sleep(500)

print "Done!"