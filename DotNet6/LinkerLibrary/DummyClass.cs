using DummyCodeGenerator;

namespace LinkerLibrary
{
    public partial class DummyClass
    {
        [Generate(Length = 10000)]
        public partial void DoSomething();
    }
}
